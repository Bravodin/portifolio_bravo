using Trace.Domain.Content;

namespace Trace.Domain.Tests.Content;

public sealed class SiteRoutesTests
{
    [Theory]
    [InlineData(PageKey.Home, "pt-BR", "/")]
    [InlineData(PageKey.Home, "en", "/en/")]
    [InlineData(PageKey.Recruiter, "pt-BR", "/recrutador/")]
    [InlineData(PageKey.Recruiter, "en", "/en/recruiter/")]
    [InlineData(PageKey.Recruiter, "es", "/es/reclutador/")]
    [InlineData(PageKey.Colophon, "es", "/es/como-se-hizo/")]
    [InlineData(PageKey.Garage, "pt-BR", "/garagem/")]
    [InlineData(PageKey.Garage, "es", "/es/garaje/")]
    public void PathFor_UsesLocalizedSlugs(PageKey page, string locale, string expected) =>
        Assert.Equal(expected, SiteRoutes.PathFor(page, locale));

    [Fact]
    public void PathFor_UnknownLocale_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SiteRoutes.PathFor(PageKey.Home, "fr"));

    [Theory]
    [InlineData("/en/recruiter", PageKey.Recruiter, "en")]
    [InlineData("en/recruiter/?utm=x#top", PageKey.Recruiter, "en")]
    [InlineData("/", PageKey.Home, "pt-BR")]
    [InlineData("", PageKey.Home, "pt-BR")]
    [InlineData("/como-foi-feito", PageKey.Colophon, "pt-BR")]
    public void TryResolve_FindsPageAndLocale(string path, PageKey page, string locale)
    {
        Assert.True(SiteRoutes.TryResolve(path, out var foundPage, out var foundLocale));
        Assert.Equal(page, foundPage);
        Assert.Equal(locale, foundLocale);
    }

    [Fact]
    public void TryResolve_UnknownPath_ReturnsFalseWithDefaultLocale()
    {
        Assert.False(SiteRoutes.TryResolve("/en/nope/", out _, out var locale));
        Assert.Equal(Locales.Default, locale);
    }

    [Theory]
    [InlineData("/en/anything/", "en")]
    [InlineData("/es/", "es")]
    [InlineData("/not-found", "pt-BR")]
    [InlineData("/english/", "pt-BR")]
    public void LocaleOf_UsesFirstSegment(string path, string expected) =>
        Assert.Equal(expected, SiteRoutes.LocaleOf(path));

    [Fact]
    public void Alternates_ListEveryLocaleInSwitcherOrder_EnEsPt()
    {
        var alternates = SiteRoutes.Alternates(PageKey.Recruiter);

        Assert.Equal(["en", "es", "pt-BR"], alternates.Keys);
        Assert.Equal("/es/reclutador/", alternates["es"]);
    }

    [Fact]
    public void AllPaths_AreUniqueAndCanonical()
    {
        var paths = SiteRoutes.AllPaths.ToList();

        Assert.Equal(12, paths.Count);
        Assert.Equal(paths.Count, paths.Distinct(StringComparer.Ordinal).Count());
        Assert.All(paths, p => Assert.True(p.StartsWith('/') && p.EndsWith('/')));
    }

    [Theory]
    [InlineData("pt-BR", true)]
    [InlineData("es", true)]
    [InlineData("fr", false)]
    [InlineData(null, false)]
    public void IsSupported_KnowsTheThreeLocales(string? locale, bool expected) =>
        Assert.Equal(expected, Locales.IsSupported(locale));
}
