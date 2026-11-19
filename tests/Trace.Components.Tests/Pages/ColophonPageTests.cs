using Bunit;
using Trace.Web.Components.Pages;
using Trace.Web.Site;

namespace Trace.Components.Tests.Pages;

public sealed class ColophonPageTests : BunitContext
{
    [Fact]
    public void AiSection_ShowsRolesReviewsAndBothSetsOfNumbers()
    {
        this.AddSite("/como-foi-feito/");

        var cut = Render<Colophon>();

        Assert.Equal("Construído com IA, dirigido por humano", cut.Find("#ai-title").TextContent);
        Assert.Equal(["Manuel", "Claude", "Juntos"], cut.FindAll(".roles-list dt").Select(d => d.TextContent));
        Assert.Equal(4, cut.FindAll(".reviews li").Count);
        var figures = cut.FindAll(".figures strong").Select(s => s.TextContent).ToList();
        Assert.Equal(["42", "10", "136", "146,6 milhões", "455 mil", "US$ 43,93"], figures);
        Assert.Equal("https://github.com/Bravodin/portifolio_bravo/commits/main", cut.Find(".ai-build a").GetAttribute("href"));
    }

    [Fact]
    public void WithoutGit_TheCommitCountIsHidden()
    {
        this.AddSite("/en/colophon/", new RepoStats(Commits: null, Adrs: 10, TestMethods: 136));

        var cut = Render<Colophon>();

        Assert.DoesNotContain(cut.FindAll(".figures span"), s => s.TextContent == "commits");
    }

    [Fact]
    public void Measure_CountsThisRepository()
    {
        var stats = RepoStats.Measure();

        Assert.Equal(10, stats.Adrs);
        Assert.True(stats.TestMethods > 100);
        Assert.True(stats.Commits is null or > 0);
    }
}
