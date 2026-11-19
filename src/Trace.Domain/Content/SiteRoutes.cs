namespace Trace.Domain.Content;

/// <summary>Pages that exist in every locale.</summary>
public enum PageKey
{
    Home,
    Recruiter,
    Colophon,
    Garage,
}

/// <summary>
/// Localized URLs (docs/ux/sitemap.md, ADR-0011): English at the root, Spanish under <c>/es/</c>, Portuguese under
/// <c>/pt/</c>, each with its own slug. Paths are canonical: leading and trailing slash.
/// </summary>
public static class SiteRoutes
{
    private static readonly Dictionary<(PageKey Page, string Locale), string> Paths = new()
    {
        [(PageKey.Home, Locales.English)] = "/",
        [(PageKey.Home, Locales.Spanish)] = "/es/",
        [(PageKey.Home, Locales.Portuguese)] = "/pt/",
        [(PageKey.Recruiter, Locales.English)] = "/recruiter/",
        [(PageKey.Recruiter, Locales.Spanish)] = "/es/reclutador/",
        [(PageKey.Recruiter, Locales.Portuguese)] = "/pt/recrutador/",
        [(PageKey.Colophon, Locales.English)] = "/colophon/",
        [(PageKey.Colophon, Locales.Spanish)] = "/es/como-se-hizo/",
        [(PageKey.Colophon, Locales.Portuguese)] = "/pt/como-foi-feito/",
        [(PageKey.Garage, Locales.English)] = "/garage/",
        [(PageKey.Garage, Locales.Spanish)] = "/es/garaje/",
        [(PageKey.Garage, Locales.Portuguese)] = "/pt/garagem/",
    };

    /// <summary>URL prefix of each non-default locale (English has none: it's at the root, ADR-0011).</summary>
    private static readonly Dictionary<string, string> Prefixes = new(StringComparer.Ordinal)
    {
        [Locales.Spanish] = "/es/",
        [Locales.Portuguese] = "/pt/",
    };

    private static readonly Dictionary<string, (PageKey Page, string Locale)> ByPath =
        Paths.ToDictionary(entry => entry.Value, entry => entry.Key, StringComparer.Ordinal);

    public static IEnumerable<string> AllPaths => Paths.Values;

    public static string PathFor(PageKey page, string locale) =>
        Paths.TryGetValue((page, locale), out var path)
            ? path
            : throw new ArgumentOutOfRangeException(nameof(locale), locale, $"No route for {page} in '{locale}'.");

    /// <summary>Finds the page and locale for a URL path such as <c>/pt/recrutador</c> or <c>pt/recrutador/?x=1</c>.</summary>
    public static bool TryResolve(string path, out PageKey page, out string locale)
    {
        if (ByPath.TryGetValue(Canonical(path), out var match))
        {
            (page, locale) = match;
            return true;
        }

        (page, locale) = (default, Locales.Default);
        return false;
    }

    /// <summary>
    /// The locale a path belongs to, from its first segment. Unknown paths (the 404 page, assets) fall back
    /// to the default locale.
    /// </summary>
    public static string LocaleOf(string path)
    {
        var canonical = Canonical(path);
        foreach (var (locale, prefix) in Prefixes)
        {
            if (canonical.StartsWith(prefix, StringComparison.Ordinal))
            {
                return locale;
            }
        }

        return Locales.Default;
    }

    /// <summary>The same page in every locale, for the language switcher and <c>hreflang</c> links.</summary>
    public static IReadOnlyDictionary<string, string> Alternates(PageKey page) =>
        Locales.All.ToDictionary(locale => locale, locale => PathFor(page, locale), StringComparer.Ordinal);

    internal static string Canonical(string path)
    {
        var clean = path.Split('?', '#')[0];
        clean = clean.StartsWith('/') ? clean : "/" + clean;
        return clean.EndsWith('/') ? clean : clean + "/";
    }
}
