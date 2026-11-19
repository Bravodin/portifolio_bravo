using System.Net;

namespace Trace.Prerender;

internal sealed record RenderedPage(string Path, string Html, PageAnalysis Analysis);

/// <summary>
/// Requests pages from the in-process host, starting at the seed routes and following every same-site link.
/// A link that doesn't render (non-200 or not HTML) is a build error, so broken links never ship.
/// </summary>
internal sealed class SiteCrawler(HttpClient client)
{
    public List<string> Errors { get; } = [];

    public async Task<IReadOnlyList<RenderedPage>> CrawlAsync(
        IEnumerable<string> seeds,
        IReadOnlySet<string> excluded,
        CancellationToken cancellationToken)
    {
        var pages = new List<RenderedPage>();
        var queue = new Queue<(string Path, string From)>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var seed in seeds)
        {
            Enqueue(SitePaths.CanonicalPage(seed), "route table");
        }

        while (queue.TryDequeue(out var next))
        {
            var page = await RenderAsync(next.Path, next.From, cancellationToken);
            if (page is null)
            {
                continue;
            }

            pages.Add(page);
            foreach (var link in page.Analysis.PageLinks)
            {
                Enqueue(link, page.Path);
            }
        }

        return [.. pages.OrderBy(p => p.Path, StringComparer.Ordinal)];

        void Enqueue(string path, string from)
        {
            if (!excluded.Contains(path) && seen.Add(path))
            {
                queue.Enqueue((path, from));
            }
        }
    }

    /// <summary>Renders one page; records an error and returns null when it doesn't come back as HTML 200.</summary>
    public async Task<RenderedPage?> RenderAsync(string path, string from, CancellationToken cancellationToken)
    {
        using var response = await client.GetAsync(new Uri(path, UriKind.Relative), cancellationToken);
        var mediaType = response.Content.Headers.ContentType?.MediaType;
        if (response.StatusCode != HttpStatusCode.OK || mediaType != "text/html")
        {
            Errors.Add($"GET {path} returned {(int)response.StatusCode} {mediaType} (linked from {from})");
            return null;
        }

        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        return new RenderedPage(path, html, HtmlAnalyzer.Analyze(html, new Uri(client.BaseAddress!, path)));
    }
}
