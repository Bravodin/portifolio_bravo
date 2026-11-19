using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Trace.Web.Components.Pages;

namespace Trace.Components.Tests.Pages;

public sealed class HomePageTests : BunitContext
{
    public HomePageTests() => Services.AddSingleton<TimeProvider>(new FakeTimeProvider());

    private IRenderedComponent<Home> RenderHome(string path)
    {
        this.AddSite(path);
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));
        return Render<Home>();
    }

    [Fact]
    public void Hero_ShowsThePortrait_AboveTheFold()
    {
        var img = RenderHome("/en/").Find(".hero picture.hero-portrait img");

        Assert.Equal(("160", "eager"), (img.GetAttribute("width"), img.GetAttribute("loading")));
        Assert.StartsWith("Portrait of Manuel Bravo", img.GetAttribute("alt"), StringComparison.Ordinal);
    }

    [Fact]
    public void About_ComesRightAfterTheHero_WithHowIWork()
    {
        var cut = RenderHome("/");

        Assert.Equal("Sobre mim", cut.Find("#about h2").TextContent);
        Assert.Equal(6, cut.FindAll("#about .about-principles li").Count);
        Assert.Equal("Foco no que posso mudar", cut.FindAll("#about .about-principles strong")[4].TextContent);
        Assert.Equal("Simples primeiro", cut.FindAll("#about .about-principles strong")[2].TextContent);
        Assert.Equal("about", cut.Find("#start + section").Id);
    }

    [Fact]
    public void Trace_HasTheSevenStagesInOrder()
    {
        var cut = RenderHome("/en/");

        Assert.Equal(["client", "edge", "gateway", "services", "cache", "database", "pit"], cut.FindAll(".stage").Select(s => s.Id));
    }

    [Fact]
    public void EachStage_ShowsItsMeasuredMetric()
    {
        var cut = RenderHome("/en/");

        Assert.Equal("−40 %", cut.Find("#edge .metric-after").TextContent);
        Assert.Equal("−30 %", cut.Find("#gateway .metric-delta").TextContent);
        Assert.Contains("metric-after-long", cut.Find("#services .metric-after").ClassName, StringComparison.Ordinal);
        Assert.Equal("4 h", cut.Find("#cache .metric-after").TextContent);
    }

    [Fact]
    public void Case_IsANativeDetailsWithProblemActionResult()
    {
        var cut = RenderHome("/");

        var details = cut.Find("#gateway details.case");
        Assert.Null(details.GetAttribute("open"));
        Assert.Equal("Abrir caso", details.QuerySelector("summary")!.TextContent);
        Assert.Contains("Migrei o gateway para a Sensedia.", details.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void StageOne_HoldsTheBenchmarkStoryAndTheDyno()
    {
        var cut = RenderHome("/es/");

        Assert.Contains("Mi gerente pidió un benchmark", cut.Find("#client .stage-story").TextContent, StringComparison.Ordinal);
        Assert.NotNull(cut.Find("#client section.dyno"));
    }

    [Fact]
    public void ServiceLog_ListsEveryRevisionDownTo2006()
    {
        var cut = RenderHome("/en/");

        var entries = cut.FindAll(".log-entry");
        Assert.Equal(10, entries.Count);
        Assert.Equal("2006 → 2008", entries[^1].QuerySelector(".log-odometer")!.TextContent);
    }

    [Fact]
    public void Toolbox_HasSevenOpenDrawers()
    {
        var cut = RenderHome("/en/");

        var drawers = cut.FindAll("details.drawer");
        Assert.Equal(7, drawers.Count);
        Assert.All(drawers, d => Assert.NotNull(d.GetAttribute("open")));
    }

    [Fact]
    public void Garage_HasThreeCards_ImagesWhereTheyExist_LineArtForGames()
    {
        var cut = RenderHome("/en/");

        Assert.NotNull(cut.Find("#garage-motorcycles picture img"));
        Assert.StartsWith("Illustration of Manuel", cut.Find("#garage-mechanics picture img").GetAttribute("alt"), StringComparison.Ordinal);
        Assert.Single(cut.FindAll(".garage-card svg.line-art"));
        Assert.NotNull(cut.Find("#garage-games svg.line-art"));
        Assert.Equal("/en/garage/", cut.Find("#garage a.btn").GetAttribute("href"));
    }

    [Fact]
    public void PitStop_HasALazyRoundAvatar_InTheVisitorsLanguage()
    {
        var img = RenderHome("/es/").Find("#pit-stop picture.avatar img");

        Assert.Equal(("64", "lazy"), (img.GetAttribute("width"), img.GetAttribute("loading")));
        Assert.Null(img.GetAttribute("fetchpriority"));
        Assert.StartsWith("Retrato de Manuel Bravo con blazer", img.GetAttribute("alt"), StringComparison.Ordinal);
    }

    [Fact]
    public void PitStop_OffersLinkedInAndCv_Only()
    {
        var cut = RenderHome("/en/");

        var pitStop = cut.Find("#pit-stop");
        Assert.Equal("https://www.linkedin.com/in/manuelbravoc/", pitStop.QuerySelector("a.btn-primary")!.GetAttribute("href"));
        Assert.Equal("/cv/manuel-bravo-cv.pdf", pitStop.QuerySelector("a[download]")!.GetAttribute("href"));
        Assert.Empty(cut.FindAll("a[href^='mailto:'], a[href^='tel:']"));
    }
}
