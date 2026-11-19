using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Trace.Prerender;

internal sealed record PrerenderOptions(string PublishDirectory, string OutputDirectory, Uri BaseUrl, string SwaConfigTemplate, bool IsCiBuild = false);

internal sealed record PrerenderResult(int Pages, int Files, long Bytes, IReadOnlyList<string> Errors, IReadOnlyList<string> Warnings);

/// <summary>Runs the whole prerender: crawl → write pages → ship assets → site files → validate (ADR-0003).</summary>
internal sealed class Prerenderer(PrerenderOptions options)
{
    /// <summary>Routes that exist in the app but are not published as pages.</summary>
    private static readonly HashSet<string> ExcludedPages = new(StringComparer.Ordinal) { "/error/", "/not-found/" };

    private const string NotFoundRoute = "/not-found";

    private readonly List<string> _errors = [];
    private readonly List<string> _warnings = [];

    public async Task<PrerenderResult> RunAsync(CancellationToken cancellationToken)
    {
        await using var host = new PrerenderHost(options.PublishDirectory);
        using var client = host.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var crawler = new SiteCrawler(client);
        var pages = await crawler.CrawlAsync(DiscoverRoutes(), ExcludedPages, cancellationToken);
        var notFound = await crawler.RenderAsync(NotFoundRoute, "404 page", cancellationToken);
        _errors.AddRange(crawler.Errors);

        PrepareOutputDirectory();

        foreach (var page in pages)
        {
            await WriteAsync(SitePaths.PageFile(page.Path), page.Html, cancellationToken);
        }

        if (notFound is not null)
        {
            await WriteAsync("404.html", notFound.Html, cancellationToken);
        }

        var rendered = notFound is null ? pages : [.. pages, notFound];
        var endpoints = StaticAssetManifest.Load(host.ManifestPath);
        CopyStaticAssets(endpoints, host.WebRoot);
        var generatedImmutable = await MaterializeMissingAssetsAsync(client, rendered, cancellationToken);
        ValidatePages(rendered);
        _errors.AddRange(HreflangValidator.Validate(pages, options.BaseUrl, Trace.Domain.Content.Locales.All));
        CheckPdfs(rendered);

        await WriteAsync("sitemap.xml", SiteFiles.Sitemap(options.BaseUrl, pages.Select(p => p.Path)), cancellationToken);
        await WriteAsync("robots.txt", SiteFiles.Robots(options.BaseUrl), cancellationToken);
        await WriteAsync(
            "staticwebapp.config.json",
            SiteFiles.StaticWebAppConfig(
                await File.ReadAllTextAsync(options.SwaConfigTemplate, cancellationToken),
                rendered.SelectMany(p => p.Analysis.InlineScripts).Select(CspHashes.ForInlineScript),
                endpoints.Where(e => e.IsImmutable).Select(e => e.Route).Concat(generatedImmutable)),
            cancellationToken);

        var files = new DirectoryInfo(options.OutputDirectory).GetFiles("*", SearchOption.AllDirectories);
        return new PrerenderResult(pages.Count, files.Length, files.Sum(f => f.Length), _errors, _warnings);
    }

    /// <summary>Every <c>@page</c> route without parameters, from the host and the islands assembly.</summary>
    private static IEnumerable<string> DiscoverRoutes() =>
        new[] { typeof(Program).Assembly, typeof(Trace.Client._Imports).Assembly }
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetCustomAttributes<RouteAttribute>())
            .Select(route => route.Template)
            .Where(template => !template.Contains('{', StringComparison.Ordinal))
            .Select(template => SitePaths.CanonicalPage(template).ToLowerInvariant())
            .Distinct(StringComparer.Ordinal);

    private void PrepareOutputDirectory()
    {
        var output = Path.GetFullPath(options.OutputDirectory);
        if (Directory.Exists(Path.Combine(output, ".git")) || File.Exists(Path.Combine(output, "Trace.slnx")))
        {
            throw new InvalidOperationException($"Refusing to clear {output}: it looks like the repository root.");
        }

        if (Directory.Exists(output))
        {
            Directory.Delete(output, recursive: true);
        }

        _ = Directory.CreateDirectory(output);
    }

    /// <summary>Writes every identity route from the publish manifest under its URL (spike finding 1).</summary>
    private void CopyStaticAssets(IEnumerable<StaticAssetEndpoint> endpoints, string webRoot)
    {
        foreach (var endpoint in endpoints)
        {
            var source = Path.Combine(webRoot, endpoint.AssetFile.Replace('/', Path.DirectorySeparatorChar));
            var target = SitePaths.ToDistPath(options.OutputDirectory, endpoint.Route);
            _ = Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(source, target, overwrite: true);
        }
    }

    /// <summary>
    /// Some URLs the pages reference are generated in memory (e.g. <c>_framework/resource-collection.{hash}.js</c>,
    /// spike finding 2). Fetch each one that isn't a file yet; a reference the host can't serve is an error.
    /// Returns the routes the host marked <c>immutable</c>, so they get the same long-lived caching on SWA.
    /// </summary>
    private async Task<List<string>> MaterializeMissingAssetsAsync(HttpClient client, IEnumerable<RenderedPage> pages, CancellationToken cancellationToken)
    {
        var immutable = new List<string>();
        var references = pages.SelectMany(p => p.Analysis.AssetReferences).Distinct(StringComparer.Ordinal);
        foreach (var reference in references)
        {
            var target = SitePaths.ToDistPath(options.OutputDirectory, reference);
            if (File.Exists(target))
            {
                continue;
            }

            using var response = await client.GetAsync(new Uri(reference, UriKind.Relative), cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var from = pages.First(p => p.Analysis.AssetReferences.Contains(reference)).Path;
                _errors.Add($"Missing asset {reference} (referenced by {from}, host returned {(int)response.StatusCode})");
                continue;
            }

            _ = Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            await File.WriteAllBytesAsync(target, await response.Content.ReadAsByteArrayAsync(cancellationToken), cancellationToken);
            if (response.Headers.CacheControl?.Extensions.Any(e => e.Name.Equals("immutable", StringComparison.OrdinalIgnoreCase)) == true)
            {
                immutable.Add(reference);
            }
        }

        return immutable;
    }

    /// <summary>
    /// Linked PDFs (the CV) must be real files. A placeholder is a warning locally, so the site can still be
    /// previewed, and an error in Azure Pipelines (TF_BUILD), so it can never be deployed.
    /// </summary>
    private void CheckPdfs(IEnumerable<RenderedPage> pages)
    {
        var pdfs = pages.SelectMany(p => p.Analysis.AssetReferences)
            .Where(r => r.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.Ordinal);

        foreach (var pdf in pdfs)
        {
            if (PdfCheck.Problem(SitePaths.ToDistPath(options.OutputDirectory, pdf)) is { } problem)
            {
                (options.IsCiBuild ? _errors : _warnings).Add($"{pdf} {problem}: replace content/cv/ with the real CV (no phone or e-mail)");
            }
        }
    }

    private void ValidatePages(IEnumerable<RenderedPage> pages)
    {
        foreach (var page in pages.Where(p => p.Analysis.Lang is null))
        {
            _errors.Add($"{page.Path} has no <html lang>");
        }
    }

    private Task WriteAsync(string relativePath, string content, CancellationToken cancellationToken) =>
        SiteFiles.WriteAsync(SitePaths.ToDistPath(options.OutputDirectory, relativePath), content, cancellationToken);
}
