using Bunit;
using Trace.Web.Components.Pages;
using Trace.Web.Site;

namespace Trace.Components.Tests.Pages;

public sealed class ColophonPageTests : BunitContext
{
    [Fact]
    public void AiSection_ShowsRolesReviewsAndBothSetsOfNumbers()
    {
        this.AddSite("/pt/como-foi-feito/");

        var cut = Render<Colophon>();

        Assert.Equal("Construído com IA, dirigido por humano", cut.Find("#ai-title").TextContent);
        Assert.Equal(["Manuel", "Claude", "Juntos"], cut.FindAll(".roles-list dt").Select(d => d.TextContent));
        Assert.Equal(4, cut.FindAll(".reviews li").Count);
        var figures = cut.FindAll(".figures strong").Select(s => s.TextContent).ToList();
        Assert.Equal(["42", "10", "136", "248,3 milhões", "608 mil", "US$ 69,38", "≈ 6 h"], figures);
        Assert.Equal("https://github.com/Bravodin/portifolio_bravo/commits/main", cut.Find(".ai-build a").GetAttribute("href"));
    }

    [Fact]
    public void Architecture_IsASixStepFlow_WithAdrLinks()
    {
        this.AddSite("/colophon/");

        var cut = Render<Colophon>();

        Assert.Equal(["Manuel", "GitHub", "Azure Pipelines", "Azure Static Web Apps", "Cloudflare + registro.br", "Browser"], cut.FindAll(".arch-node h3").Select(h => h.TextContent));
        var adr9 = cut.FindAll(".arch-adrs a").First(a => a.TextContent == "ADR-0009");
        Assert.Equal("https://github.com/Bravodin/portifolio_bravo/blob/main/docs/architecture/adr/0009-azure-pipelines-and-static-web-apps.md", adr9.GetAttribute("href"));
        var unknown = cut.FindAll(".arch-adrs a").First(a => a.TextContent == "ADR-0003");
        Assert.EndsWith("/docs/architecture/adr/README.md", unknown.GetAttribute("href"), StringComparison.Ordinal);
    }

    [Fact]
    public void LatestBuild_ShowsTheRunHistoryScreenshot_LinkedToTheLargeVersion()
    {
        this.AddSite("/es/como-se-hizo/");

        var figure = Render<Colophon>().Find(".build-panel figure.build-shot");

        Assert.StartsWith("Pantalla de Azure DevOps", figure.QuerySelector("img")!.GetAttribute("alt"), StringComparison.Ordinal);
        Assert.Matches(@"^/media/colophon-pipeline-1600\.[0-9a-f]{10}\.jpg$", figure.QuerySelector("a.photo-link")!.GetAttribute("href"));
        Assert.StartsWith("El pipeline en Azure DevOps", figure.QuerySelector("figcaption")!.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void LatestBuild_ShowsThePipelineRun()
    {
        var build = new BuildInfo("20261119.5", "439189f9abcdef", "main", new DateTimeOffset(2026, 11, 19, 20, 30, 0, TimeSpan.Zero),
            new Uri("https://dev.azure.com/org/TRACE/_build/results?buildId=42"), 186, 186, 1.0);
        this.AddSite("/colophon/", build: build);

        var panel = Render<Colophon>().Find(".build-panel");

        Assert.Contains("20261119.5", panel.TextContent, StringComparison.Ordinal);
        Assert.Equal("https://github.com/Bravodin/portifolio_bravo/commit/439189f9abcdef", panel.QuerySelector(".build-facts a")!.GetAttribute("href"));
        Assert.Contains("186 / 186", panel.TextContent, StringComparison.Ordinal);
        Assert.Contains("100%", panel.TextContent, StringComparison.Ordinal);
        Assert.Contains("2026-11-19 20:30 UTC", panel.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void LatestBuild_OnALocalBuild_SaysSo()
    {
        this.AddSite("/pt/como-foi-feito/");

        var panel = Render<Colophon>().Find(".build-panel");

        Assert.Empty(panel.QuerySelectorAll(".build-facts"));
        Assert.Contains("gerada localmente", panel.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void Architecture_HasBothViews_DiagramChosenByDefault()
    {
        this.AddSite("/pt/como-foi-feito/");

        var cut = Render<Colophon>();

        Assert.NotNull(cut.Find("#arch-view-graph").GetAttribute("checked"));
        Assert.Null(cut.Find("#arch-view-list").GetAttribute("checked"));
        Assert.Equal(["Gráfica", "Detalhada"], cut.FindAll(".arch-toggle label").Select(l => l.TextContent));
        Assert.Equal(["Autor", "Código", "CI/CD", "Hospedagem", "DNS", "Cliente"], cut.FindAll(".lane-label").Select(l => l.TextContent));
        Assert.Equal(3, cut.FindAll(".lane-publish").Count);
        Assert.Equal(2, cut.FindAll(".lane-access").Count);
        Assert.Equal("Azure Static Web Apps", cut.Find(".lane-both strong").TextContent);
        Assert.Equal(6, cut.FindAll(".arch-flow > .arch-node").Count);
    }

    [Fact]
    public void WithoutGit_TheCommitCountIsHidden()
    {
        this.AddSite("/colophon/", new RepoStats(Commits: null, Adrs: 10, TestMethods: 136));

        var cut = Render<Colophon>();

        Assert.DoesNotContain(cut.FindAll(".figures span"), s => s.TextContent == "commits");
    }

    // The repository root: two levels above content/media.
    private static string MediaRootParent() => Path.GetFullPath(Path.Combine(MediaCatalog.FindRoot(null), "..", ".."));

    [Fact]
    public void Measure_CountsThisRepository()
    {
        var stats = RepoStats.Measure();

        Assert.Equal(Directory.GetFiles(Path.Combine(MediaRootParent(), "docs", "architecture", "adr"), "0*.md").Length, stats.Adrs);
        Assert.Equal("0010-video-youtube-facade.md", stats.AdrFiles["0010"]);
        Assert.True(stats.TestMethods > 100);
        Assert.True(stats.Commits is null or > 0);
    }
}
