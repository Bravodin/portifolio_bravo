using System.Text.Json;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Trace.Prerender;

/// <summary>What the prerender needs to know about one rendered page.</summary>
/// <param name="Lang">The <c>&lt;html lang&gt;</c> value, or null when missing.</param>
/// <param name="PageLinks">Same-site links to other pages, as canonical paths (<c>/en/colophon/</c>).</param>
/// <param name="AssetReferences">Same-site files the page loads or links to (<c>/app.{hash}.css</c>).</param>
/// <param name="InlineScripts">Text of every inline script the CSP must allow (import map included).</param>
internal sealed record PageAnalysis(
    string? Lang,
    IReadOnlyList<string> PageLinks,
    IReadOnlyList<string> AssetReferences,
    IReadOnlyList<string> InlineScripts);

internal static class HtmlAnalyzer
{
    // Inline script types the browser executes or interprets under script-src.
    private static readonly HashSet<string> CspScriptTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        string.Empty, "text/javascript", "application/javascript", "module", "importmap",
    };

    public static PageAnalysis Analyze(string html, Uri pageUrl)
    {
        var document = new HtmlParser().ParseDocument(html);
        var baseUrl = document.QuerySelector("base[href]")?.GetAttribute("href") is { } baseHref
            ? new Uri(pageUrl, baseHref)
            : pageUrl;

        var pages = new SortedSet<string>(StringComparer.Ordinal);
        var assets = new SortedSet<string>(StringComparer.Ordinal);
        var inline = new List<string>();

        foreach (var anchor in document.QuerySelectorAll("a[href]"))
        {
            if (SameSitePath(pageUrl, anchor.GetAttribute("href")!) is { } path)
            {
                _ = SitePaths.LooksLikePage(path) ? pages.Add(SitePaths.CanonicalPage(path)) : assets.Add(path);
            }
        }

        AddAll(assets, baseUrl, document.QuerySelectorAll("link[href]").Select(e => e.GetAttribute("href")!));
        AddAll(assets, baseUrl, document.QuerySelectorAll("script[src], img[src], source[src]").Select(e => e.GetAttribute("src")!));
        AddAll(assets, baseUrl, document.QuerySelectorAll("img[srcset], source[srcset]").SelectMany(e => SrcsetUrls(e.GetAttribute("srcset")!)));

        foreach (var script in document.QuerySelectorAll("script:not([src])"))
        {
            var type = script.GetAttribute("type")?.Trim() ?? string.Empty;
            if (!CspScriptTypes.Contains(type))
            {
                continue; // e.g. application/ld+json is data, not script.
            }

            inline.Add(script.TextContent);
            if (type.Equals("importmap", StringComparison.OrdinalIgnoreCase))
            {
                AddAll(assets, baseUrl, ImportMapUrls(script));
            }
        }

        var lang = document.DocumentElement.GetAttribute("lang");
        return new PageAnalysis(string.IsNullOrWhiteSpace(lang) ? null : lang, [.. pages], [.. assets], inline);
    }

    private static void AddAll(SortedSet<string> target, Uri baseUrl, IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            if (SameSitePath(baseUrl, url) is { } path && !StaticAssetManifest.IsCompressedRoute(path))
            {
                _ = target.Add(path);
            }
        }
    }

    /// <summary>The URL path when <paramref name="href"/> stays on this site; null for external or non-HTTP links.</summary>
    private static string? SameSitePath(Uri baseUrl, string href)
    {
        if (string.IsNullOrWhiteSpace(href) || href.StartsWith('#'))
        {
            return null;
        }

        var resolved = new Uri(baseUrl, href);
        return resolved.Scheme is "http" or "https" && resolved.Authority == baseUrl.Authority
            ? Uri.UnescapeDataString(resolved.AbsolutePath)
            : null;
    }

    private static IEnumerable<string> SrcsetUrls(string srcset) =>
        srcset.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(candidate => candidate.Split(' ', 2)[0]);

    private static IEnumerable<string> ImportMapUrls(IElement script)
    {
        using var json = JsonDocument.Parse(script.TextContent);
        if (!json.RootElement.TryGetProperty("imports", out var imports))
        {
            return [];
        }

        return [.. imports.EnumerateObject().Select(entry => entry.Value.GetString()!)];
    }
}
