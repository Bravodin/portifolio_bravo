namespace Trace.Domain.Content;

/// <summary>Pages that exist in every locale.</summary>
public enum PageKey
{
    Home,
    Recruiter,
    Colophon,
}

/// <summary>
/// Localized URLs (docs/ux/sitemap.md): PT-BR at the root, EN under <c>/en/</c>, ES under <c>/es/</c>,
/// each with its own slug. Paths are canonical: leading and trailing slash.
/// </summary>
public static class SiteRoutes
{
    private static readonly Dictionary<(PageKey Page, string Locale), string> Paths = new()
    {
        [(PageKey.Home, Locales.Portuguese)] = "/",
        [(PageKey.Home, Locales.English)] = "/en/",
        [(PageKey.Home, Locales.Spanish)] = "/es/",
        [(PageKey.Recruiter, Locales.Portuguese)] = "/recrutador/",
        [(PageKey.Recruiter, Locales.English)] = "/en/recruiter/",
        [(PageKey.Recruiter, Locales.Spanish)] = "/es/reclutador/",
        [(PageKey.Colophon, Locales.Portuguese)] = "/como-foi-feito/",
        [(PageKey.Colophon, Locales.English)] = "/en/colophon/",
        [(PageKey.Colophon, Locales.Spanish)] = "/es/como-se-hizo/",
    };

    private static readonly Dictionary<string, (PageKey Page, string Locale)> ByPath =
        Paths.ToDictionary(entry => entry.Value, entry => entry.Key, StringComparer.Ordinal);

    public static IEnumerable<string> AllPaths => Paths.Values;

    public static string PathFor(PageKey page, string locale) =>
        Paths.TryGetValue((page, locale), out var path)
            ? path
            : throw new ArgumentOutOfRangeException(nameof(locale), locale, $"No route for {page} in '{locale}'.");

    /// <summary>Finds the page and locale for a URL path such as <c>/en/recruiter</c> or <c>en/recruiter/?x=1</c>.</summary>
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
        foreach (var locale in Locales.All.Where(l => l != Locales.Default))
        {
            if (canonical.StartsWith($"/{locale}/", StringComparison.Ordinal))
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
