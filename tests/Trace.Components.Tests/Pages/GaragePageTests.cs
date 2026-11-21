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
        Assert.Equal("Player de vídeo", article.GetAttribute("data-lightbox-video-label"));
        Assert.Equal("Próximo vídeo", article.GetAttribute("data-lightbox-video-next"));
    }

    [Fact]
    public void Photos_CreditTheirPhotographers()
    {
        this.AddSite("/pt/garagem/");

        var credits = Render<Garage>().FindAll(".photo-grid figcaption small").Select(c => c.TextContent).ToList();

        Assert.Equal(3, credits.Count(c => c == "Foto: @mustelasportmedia"));
        Assert.Contains("Foto: B.S Racing · MotoGP Cerezer", credits);
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
    public void Videos_AreFacadesThatWorkWithoutJs()
    {
        this.AddSite("/es/garaje/");

        var facades = Render<Garage>().FindAll("a.video-facade");

        Assert.Equal(4, facades.Count);
        Assert.Matches(@"^/media/short-01-roll-out\.[0-9a-f]{10}\.mp4$", facades[0].GetAttribute("href"));
        Assert.True(facades[0].HasAttribute("data-video"));
        Assert.All(facades, a => Assert.Equal("false", a.GetAttribute("data-enhance-nav")));
        Assert.Equal("Reproducir video: Saliendo a la pista", facades[0].GetAttribute("aria-label"));
    }

    [Fact]
    public void Videos_ShowOurOwnPoster_AndNothingFromYouTube()
    {
        this.AddSite("/garage/");

        var cut = Render<Garage>();

        Assert.Equal(4, cut.FindAll(".video-facade picture.video-poster img").Count);
        Assert.DoesNotContain("youtube", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ytimg", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("<iframe", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void Videos_DescribeWhatHappens_ForPeopleWhoCantWatch()
    {
        this.AddSite("/garage/");

        var caption = Render<Garage>().Find(".video figcaption").TextContent;

        Assert.Contains("No sound.", caption, StringComparison.Ordinal);
        Assert.Contains("Video: @mustelasportmedia", caption, StringComparison.Ordinal);
    }
}
