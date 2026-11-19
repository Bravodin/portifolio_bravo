using Bunit;
using Trace.Web.Components.Pages;

namespace Trace.Components.Tests.Pages;

public sealed class GaragePageTests : BunitContext
{
    [Fact]
    public void Photos_AreTheNineTrackDays_EachLinkingToItsLargeVersion()
    {
        this.AddSite("/en/garage/");

        var links = Render<Garage>().FindAll("section[aria-labelledby=garage-track] a.photo-link");

        Assert.Equal(9, links.Count);
        Assert.All(links, a => Assert.Equal("false", a.GetAttribute("data-enhance-nav")));
        Assert.All(links, a => Assert.Matches(@"^/media/track-0\d-1600\.[0-9a-f]{10}\.jpg$", a.GetAttribute("href")));
    }

    [Fact]
    public void Lightbox_GetsItsLabelsInThePagesLanguage()
    {
        this.AddSite("/garagem/");

        var article = Render<Garage>().Find("article.garage-page");

        Assert.Equal("Fechar", article.GetAttribute("data-lightbox-close"));
        Assert.Equal("Próxima foto", article.GetAttribute("data-lightbox-next"));
    }

    [Fact]
    public void Photos_CreditTheirPhotographers()
    {
        this.AddSite("/garagem/");

        var credits = Render<Garage>().FindAll(".photo-grid figcaption small").Select(c => c.TextContent).ToList();

        Assert.Equal(8, credits.Count(c => c == "Foto: @mustelasportmedia"));
        Assert.Contains("Foto: B.S Racing · MotoGP Cerezer", credits);
    }

    [Fact]
    public void Workshop_ShowsTheIllustration_LabelledAsAiGenerated()
    {
        this.AddSite("/garagem/");

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
    public void Videos_AreFacadesThatWorkWithoutJs()
    {
        this.AddSite("/es/garaje/");

        var facades = Render<Garage>().FindAll("a.video-facade");

        Assert.Equal(4, facades.Count);
        Assert.Equal("https://www.youtube.com/shorts/xEh1hYPC3lM", facades[0].GetAttribute("href"));
        Assert.Equal("xEh1hYPC3lM", facades[0].GetAttribute("data-youtube"));
        Assert.Equal("Reproducir video: Saliendo a la pista", facades[0].GetAttribute("aria-label"));
    }

    [Fact]
    public void Videos_WithoutADownloadedPoster_ShowAPlainCard_AndNoYouTubeImage()
    {
        this.AddSite("/en/garage/");

        var cut = Render<Garage>();

        Assert.Equal(4, cut.FindAll(".video-placeholder").Count);
        Assert.DoesNotContain("ytimg", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("<iframe", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Videos_DescribeWhatHappens_ForPeopleWhoCantWatch()
    {
        this.AddSite("/en/garage/");

        var caption = Render<Garage>().Find(".video figcaption").TextContent;

        Assert.Contains("Engine sound only.", caption, StringComparison.Ordinal);
        Assert.Contains("Video: @mustelasportmedia", caption, StringComparison.Ordinal);
    }
}
