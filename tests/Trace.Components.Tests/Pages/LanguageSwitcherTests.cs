using Bunit;
using Trace.Web.Components.Shared;

namespace Trace.Components.Tests.Pages;

public sealed class LanguageSwitcherTests : BunitContext
{
    [Fact]
    public void LinksToTheSamePageInEachLanguage_AndMarksTheCurrentOne()
    {
        this.AddSite("/en/recruiter/");

        var links = Render<LanguageSwitcher>().FindAll("a");

        Assert.Equal(["/recrutador/", "/en/recruiter/", "/es/reclutador/"], links.Select(a => a.GetAttribute("href")));
        Assert.Equal(["PT", "EN", "ES"], links.Select(a => a.TextContent));
        Assert.Equal("true", links[1].GetAttribute("aria-current"));
        Assert.Null(links[0].GetAttribute("aria-current"));
    }

    [Fact]
    public void ForcesFullReload_SoHtmlLangChangesToo()
    {
        this.AddSite("/");

        var links = Render<LanguageSwitcher>().FindAll("a");

        Assert.All(links, a => Assert.Equal("false", a.GetAttribute("data-enhance-nav")));
        Assert.Equal(["pt-BR", "en", "es"], links.Select(a => a.GetAttribute("lang")));
    }

    [Fact]
    public void OnAPageWithoutTranslations_FallsBackToEachHome()
    {
        this.AddSite("/not-found");

        var links = Render<LanguageSwitcher>().FindAll("a");

        Assert.Equal(["/", "/en/", "/es/"], links.Select(a => a.GetAttribute("href")));
    }
}
