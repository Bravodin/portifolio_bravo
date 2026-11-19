using Microsoft.AspNetCore.Components;
using Trace.Domain.Content;

namespace Trace.Web.Site;

/// <summary>Settings that don't change per page.</summary>
/// <param name="SiteUrl">Canonical origin, used for <c>canonical</c> and <c>hreflang</c> links (ADR-0009: the www host).</param>
public sealed record SiteOptions(Uri SiteUrl);

/// <summary>
/// What the current request is: which page, in which locale, with which content. Derived from the URL,
/// so the same component renders all three locales (ADR-0004).
/// </summary>
public sealed class PageContext(NavigationManager navigation, ContentStore store, SiteOptions options)
{
    private string Path => "/" + navigation.ToBaseRelativePath(navigation.Uri);

    public string Locale => SiteRoutes.LocaleOf(Path);

    public SiteContent Content => store.For(Locale);

    public RecruiterSelection Recruiter => store.Recruiter;

    /// <summary>The page being rendered, or null for pages that exist in one form only (404).</summary>
    public PageKey? Page => SiteRoutes.TryResolve(Path, out var page, out _) ? page : null;

    public string T(string key) => Content.T(key);

    public string PathFor(PageKey page) => SiteRoutes.PathFor(page, Locale);

    public string Absolute(string path) => new Uri(options.SiteUrl, path).AbsoluteUri;
}
