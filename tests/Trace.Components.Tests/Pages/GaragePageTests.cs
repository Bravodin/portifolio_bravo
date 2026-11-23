using Bunit;
using Trace.Web.Components.Pages;

namespace Trace.Components.Tests.Pages;

public sealed class GaragePageTests : BunitContext
{
    [Fact]
    public void Photos_AreTheFourChosenTrackDays_EachLinkingToItsLargeVersion()
    {
        this.AddSite("/garage/");

        var links = Render<Garage>().FindAll("section[aria-labelledby=garage-track] a.photo-link");

        Assert.Equal(4, links.Count);
        Assert.All(links, a => Assert.Equal("false", a.GetAttribute("data-enhance-nav")));
        Assert.All(links, a => Assert.Matches(@"^/media/track-0\d-1600\.[0-9a-f]{10}\.jpg$", a.GetAttribute("href")));
    }

    [Fact]
    public void Lightbox_GetsItsLabelsInThePagesLanguage()
    {
        this.AddSite("/pt/garagem/");

        var article = Render<Garage>().Find("article.garage-page");

        Assert.Equal("Fechar", article.GetAttribute("data-lightbox-close"));
        Assert.Equal("Próxima foto", article.GetAttribute("data-lightbox-next"));
        Assert.False(article.HasAttribute("data-lightbox-video-label"));
    }

    [Fact]
    public void Photos_CreditTheirPhotographers()
    {
        this.AddSite("/pt/garagem/");

        var cut = Render<Garage>();
        var credits = cut.FindAll(".photo-grid figcaption small").Select(c => c.TextContent).ToList();

        Assert.Equal(3, credits.Count(c => c == "Foto: @mustelasportmedia"));
        Assert.Contains("Foto: B.S Racing · MotoGP Cerezer", credits);
        Assert.Empty(cut.FindAll(".photo-grid figcaption a"));
    }

    [Fact]
    public void Workshop_ShowsTheIllustration_LabelledAsAiGenerated()
    {
        this.AddSite("/pt/garagem/");

        var workshop = Render<Garage>().Find("section[aria-labelledby=garage-workbench]");

        Assert.Equal("Na oficina", workshop.QuerySelector("h2")!.TextContent);
        Assert.Single(workshop.QuerySelectorAll("a.photo-link"));
        Assert.Equal("Ilustração gerada com IA", workshop.QuerySelector(".ai-label")!.TextContent);
        Assert.StartsWith("Ilustração de Manuel", workshop.QuerySelector("img")!.GetAttribute("alt"), StringComparison.Ordinal);
    }

    [Fact]
    public void Games_HasItsOwnSection()
    {
        this.AddSite("/es/garaje/");

        var section = Render<Garage>().Find("section[aria-labelledby=garage-games]");

        Assert.Equal("Videojuegos", section.QuerySelector("h2")!.TextContent);
        Assert.Single(section.QuerySelectorAll("a.photo-link"));
    }

    [Fact]
    public void Garage_IsPhotosOnly_WithNoVideoOrThirdPartyPlayer()
    {
        // ADR-0014: no video, so a visit can't cost more bandwidth than a page of images.
        this.AddSite("/garage/");

        var markup = Render<Garage>().Markup;

        Assert.DoesNotContain("<video", markup, StringComparison.Ordinal);
        Assert.DoesNotContain(".mp4", markup, StringComparison.Ordinal);
        Assert.DoesNotContain("<iframe", markup, StringComparison.Ordinal);
        Assert.DoesNotContain("youtube", markup, StringComparison.OrdinalIgnoreCase);
    }
}
