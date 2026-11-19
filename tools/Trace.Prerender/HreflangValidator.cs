namespace Trace.Prerender;

/// <summary>
/// Quality gate (ADR-0007): every translated page lists all its translations plus <c>x-default</c>, each one
/// points at a page that was rendered, and the links are reciprocal (EN → PT implies PT → EN).
/// Search engines ignore <c>hreflang</c> sets that break any of these rules.
/// </summary>
internal static class HreflangValidator
{
    public static IEnumerable<string> Validate(IReadOnlyList<RenderedPage> pages, Uri siteUrl, IReadOnlyCollection<string> locales)
    {
        var byUrl = pages.ToDictionary(p => new Uri(siteUrl, p.Path).AbsoluteUri, StringComparer.Ordinal);

        foreach (var page in pages.Where(p => p.Analysis.Alternates.Count > 0))
        {
            var self = new Uri(siteUrl, page.Path).AbsoluteUri;
            var alternates = page.Analysis.Alternates;

            foreach (var locale in locales.Append("x-default").Where(l => !alternates.ContainsKey(l)))
            {
                yield return $"{page.Path} has no hreflang=\"{locale}\" alternate";
            }

            if (page.Analysis.Lang is { } lang && alternates.TryGetValue(lang, out var own) && own != self)
            {
                yield return $"{page.Path} is <html lang=\"{lang}\"> but its hreflang=\"{lang}\" points at {own}";
            }

            foreach (var (hreflang, url) in alternates)
            {
                if (!byUrl.TryGetValue(url, out var target))
                {
                    yield return $"{page.Path} hreflang=\"{hreflang}\" points at {url}, which wasn't rendered";
                    continue;
                }

                if (hreflang != "x-default" && page.Analysis.Lang is { } pageLang
                    && (!target.Analysis.Alternates.TryGetValue(pageLang, out var back) || back != self))
                {
                    yield return $"{target.Path} doesn't link back to {page.Path} as hreflang=\"{pageLang}\"";
                }
            }
        }
    }
}
