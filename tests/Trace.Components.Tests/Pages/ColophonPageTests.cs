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
        Assert.Equal(["42", "10", "136", "146,6 milhões", "455 mil", "US$ 43,93"], figures);
        Assert.Equal("https://github.com/Bravodin/portifolio_bravo/commits/main", cut.Find(".ai-build a").GetAttribute("href"));
    }

    [Fact]
    public void Architecture_IsAFiveStepFlow_WithAdrLinks()
    {
        this.AddSite("/colophon/");

        var cut = Render<Colophon>();

        Assert.Equal(["Manuel", "GitHub", "Azure Pipelines", "Azure Static Web Apps", "Browser"], cut.FindAll(".arch-node h3").Select(h => h.TextContent));
        var adr9 = cut.FindAll(".arch-adrs a").First(a => a.TextContent == "ADR-0009");
        Assert.Equal("https://github.com/Bravodin/portifolio_bravo/blob/main/docs/architecture/adr/0009-azure-pipelines-and-static-web-apps.md", adr9.GetAttribute("href"));
        var unknown = cut.FindAll(".arch-adrs a").First(a => a.TextContent == "ADR-0003");
        Assert.EndsWith("/docs/architecture/adr/README.md", unknown.GetAttribute("href"), StringComparison.Ordinal);
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
