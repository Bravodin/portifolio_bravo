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
        var img = RenderHome("/").Find(".hero picture.hero-portrait img");

        Assert.Equal(("160", "eager"), (img.GetAttribute("width"), img.GetAttribute("loading")));
        Assert.StartsWith("Portrait of Manuel Bravo", img.GetAttribute("alt"), StringComparison.Ordinal);
    }

    [Fact]
    public void Hero_LinksToGitHub_NextToRecruiterMode()
    {
        var actions = RenderHome("/").FindAll(".hero-actions a");

        Assert.Equal("https://github.com/Bravodin", actions[2].GetAttribute("href"));
        Assert.Equal(("noopener", "_blank"), (actions[2].GetAttribute("rel"), actions[2].GetAttribute("target")));
        Assert.Contains("GitHub", actions[2].TextContent, StringComparison.Ordinal);
        Assert.NotNull(actions[2].QuerySelector("svg[aria-hidden=true]"));
    }

    [Fact]
    public void Hero_OffersTheCv_AsAPrimaryButton_RightAfterRecruiterMode()
    {
        var cv = RenderHome("/pt/").FindAll(".hero-actions a")[1];

        Assert.Equal("/cv/manuel-bravo-resume.pdf", cv.GetAttribute("href"));
        Assert.True(cv.HasAttribute("download"));
        Assert.Contains("btn-primary", cv.ClassList);
        Assert.Equal("Baixar CV (PDF)", cv.TextContent);
        Assert.NotNull(cv.QuerySelector("svg[aria-hidden=true]"));
    }

    [Theory]
    [InlineData("/", "/#trace")]
    [InlineData("/pt/", "/pt/#trace")]
    [InlineData("/es/", "/es/#trace")]
    public void Hero_JumpLinks_StayInThePageLanguage(string path, string trace)
    {
        // <base href="/"> would turn a bare "#trace" into "/#trace", the English home page.
        var actions = RenderHome(path).FindAll(".hero-actions a");

        Assert.Equal(trace, actions[3].GetAttribute("href"));
    }

    [Fact]
    public void WhatIBring_SitsBetweenAboutAndTheTrace_WithThreePoints()
    {
        var cut = RenderHome("/pt/");

        Assert.Equal("bring", cut.Find("#about + section").Id);
        Assert.Equal("trace", cut.Find("#bring + section").Id);
        Assert.Equal("O que eu trago para o seu time", cut.Find("#bring-title").TextContent);
        Assert.Equal(
            ["Modernização de plataformas críticas", "Liderança técnica e desenvolvimento de equipes", "Redução de custos, riscos e tempo operacional"],
            cut.FindAll("#bring ol.bring-list > li").Select(li => li.TextContent));
    }

    [Fact]
    public void About_ComesRightAfterTheHero_WithHowIWork()
    {
        var cut = RenderHome("/pt/");

        Assert.Equal("Sobre mim", cut.Find("#about h2").TextContent);
        Assert.Equal(6, cut.FindAll("#about .about-principles li").Count);
        Assert.Equal("Foco no que posso mudar", cut.FindAll("#about .about-principles strong")[4].TextContent);
        Assert.Equal("Simples primeiro", cut.FindAll("#about .about-principles strong")[2].TextContent);
        Assert.Equal("about", cut.Find("#start + section").Id);
    }

    [Fact]
    public void Trace_HasTheSevenStagesInOrder()
    {
        var cut = RenderHome("/");

        Assert.Equal(["client", "edge", "gateway", "services", "cache", "database", "pit"], cut.FindAll(".stage").Select(s => s.Id));
    }

    [Fact]
    public void EachStage_ShowsItsMeasuredMetric()
    {
        var cut = RenderHome("/");

        Assert.Equal("−40 %", cut.Find("#edge .metric-after").TextContent);
        Assert.Equal("−30 %", cut.Find("#gateway .metric-delta").TextContent);
        Assert.Contains("metric-after-long", cut.Find("#services .metric-after").ClassName, StringComparison.Ordinal);
        Assert.Equal("4 h", cut.Find("#cache .metric-after").TextContent);
    }

    [Fact]
    public void Stage7_SplitsQualityObservabilityAndAi_EachWithItsOwnStory()
    {
        var stage = RenderHome("/").Find("#pit");

        Assert.Equal("Quality, Observability & AI", stage.QuerySelector("h3")!.TextContent);
        var parts = stage.QuerySelectorAll(".stage-part");
        Assert.Equal(["Quality", "Observability", "AI productivity"], parts.Select(p => p.QuerySelector("h4")!.TextContent));
        Assert.Equal(["M8", null, "M7"], parts.Select(p => p.QuerySelector(".stage-metric")?.GetAttribute("data-metric")));
        Assert.Equal(["Problem", "Action"], parts[1].QuerySelectorAll("dt").Select(dt => dt.TextContent));
        Assert.Equal(["Action", "Result"], parts[2].QuerySelectorAll("dt").Select(dt => dt.TextContent));
        Assert.Null(stage.QuerySelector("details.case"));
    }

    [Fact]
    public void Case_IsANativeDetailsWithProblemActionResult()
    {
        var cut = RenderHome("/pt/");

        var details = cut.Find("#gateway details.case");
        Assert.Null(details.GetAttribute("open"));
        Assert.Equal("Abrir caso", details.QuerySelector("summary")!.TextContent);
        Assert.Contains("Migrei o gateway para a Sensedia.", details.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void DeepCase_ShowsContextDecisionsAndBeforeAfter_InsideTheDetails()
    {
        var cut = RenderHome("/pt/");

        var deep = cut.Find("#client details.case .case-deep");
        Assert.Equal(["Contexto", "Decisões", "Arquitetura", "O mais difícil"], deep.QuerySelectorAll("h4").Select(h => h.TextContent));
        Assert.Equal(3, deep.QuerySelectorAll(".case-decisions li").Length);
        Assert.Equal(["Ionic 3 · 2016"], deep.QuerySelectorAll(".case-before .case-lane-title").Select(t => t.TextContent));
        Assert.Equal(["iOS", "Android"], deep.QuerySelectorAll(".case-after .case-lane-title").Select(t => t.TextContent));
        Assert.Contains("30 de novembro de 2026", deep.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void DeepCase_WithoutAHardPart_LeavesThatHeadingOut()
    {
        var home = RenderHome("/");
        var deep = home.Find("#database .case-deep");

        Assert.Equal(["Context", "Decisions", "Architecture"], deep.QuerySelectorAll("h4").Select(h => h.TextContent));
        Assert.Contains("Indexes and keys", deep.QuerySelector(".case-after")!.TextContent, StringComparison.Ordinal);
        Assert.Contains("planned for 2027", home.Find("#services .case-deep").TextContent, StringComparison.Ordinal);
        Assert.Empty(home.FindAll("#gateway .case-deep"));
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
        var cut = RenderHome("/");

        var entries = cut.FindAll(".log-entry");
        Assert.Equal(10, entries.Count);
        Assert.Equal("2006 → 2008", entries[^1].QuerySelector(".log-odometer")!.TextContent);
    }

    [Fact]
    public void LeadershipAndScope_SitsUnderWhatIBring_NotInTheServiceLog()
    {
        var cut = RenderHome("/pt/");

        var block = cut.Find("#bring .bring-scope");
        Assert.Equal("Liderança e escopo", block.QuerySelector("h3")!.TextContent);
        Assert.Equal("Porto · Lead Engineer · jul/2022 → hoje", block.QuerySelector(".bring-scope-role")!.TextContent);
        var scope = block.QuerySelectorAll("li").Select(li => li.TextContent).ToList();
        Assert.Equal(5, scope.Count);
        Assert.Equal("Lidero um time de 12 pessoas", scope[0]);
        Assert.Contains("Decido a arquitetura, que passa pela validação de Segurança da Informação", scope);
        Assert.Empty(cut.FindAll("#service-log ul"));
    }

    [Fact]
    public void Toolbox_LeadsWithCoreExpertise_AndFoldsAwayPreviousTechnologies()
    {
        var cut = RenderHome("/");

        var drawers = cut.FindAll("details.drawer");
        Assert.Equal(["Core expertise", "Architecture & Cloud", "Engineering practices", "Previous technologies"], drawers.Select(d => d.QuerySelector("summary")!.TextContent));
        Assert.All(drawers.Take(3), d => Assert.True(d.HasAttribute("open")));
        Assert.False(drawers[3].HasAttribute("open"));
    }

    [Fact]
    public void Garage_HasThreeCards_AllWithImages_GamesListsFavourites()
    {
        var cut = RenderHome("/");

        Assert.NotNull(cut.Find("#garage-motorcycles picture img"));
        Assert.StartsWith("Illustration of Manuel", cut.Find("#garage-mechanics picture img").GetAttribute("alt"), StringComparison.Ordinal);
        Assert.Empty(cut.FindAll(".garage-card svg.line-art"));
        Assert.Contains("blurred", cut.Find("#garage-games picture img").GetAttribute("alt"), StringComparison.Ordinal);
        Assert.Equal(8, cut.FindAll("#garage-games .garage-favorites li").Count);
        Assert.Equal("Retro", cut.Find("#garage-games .garage-favorites-label").TextContent);
        Assert.Equal("/garage/", cut.Find("#garage a.btn").GetAttribute("href"));
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
    public void PitStop_OffersLinkedInEmailAndCv_NoPhone()
    {
        var cut = RenderHome("/");

        var pitStop = cut.Find("#pit-stop");
        Assert.Equal("https://www.linkedin.com/in/manuelbravoc/", pitStop.QuerySelector("a.btn-primary")!.GetAttribute("href"));
        Assert.Equal("mailto:manuelbravoc@gmail.com", pitStop.QuerySelector("a[href^='mailto:']")!.GetAttribute("href"));
        Assert.Equal("manuelbravoc@gmail.com", pitStop.QuerySelector("a[href^='mailto:']")!.TextContent);
        Assert.Equal("/cv/manuel-bravo-resume.pdf", pitStop.QuerySelector("a[download]")!.GetAttribute("href"));
        Assert.Single(cut.FindAll("a[href^='mailto:']"));
        Assert.Empty(cut.FindAll("a[href^='tel:']"));
    }
}
