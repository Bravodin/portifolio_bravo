using Bunit;
using Microsoft.AspNetCore.Components;
using Trace.Web.Components.Layout;
using Trace.Web.Site;

namespace Trace.Components.Tests.Pages;

public sealed class MainLayoutTests : BunitContext
{
    [Fact]
    public void ErrorBar_KeepsItsWordsOutOfThePageText_InThePagesLanguage()
    {
        this.AddSite("/pt/");

        var cut = Render<MainLayout>(p => p.Add(l => l.Body, (RenderFragment)(b => b.AddContent(0, "body"))));

        var bar = cut.Find("#blazor-error-ui");
        Assert.Equal(string.Empty, bar.TextContent.Trim());
        Assert.Equal("Algo falhou numa parte interativa desta página.", bar.GetAttribute("data-message"));
        Assert.Equal("Recarregar", bar.QuerySelector(".reload")!.GetAttribute("data-label"));
        Assert.Equal("Fechar", bar.QuerySelector(".dismiss")!.GetAttribute("aria-label"));
        Assert.DoesNotContain("unhandled error", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Footer_InThePipeline_ShowsTheVersionAndTheBuild_WithoutLinkingThePrivateRun()
    {
        var build = new BuildInfo("20261122.3", "439189f9abcdef", "main", DateTimeOffset.UnixEpoch, new Uri("https://dev.azure.com/org/TRACE/_build/results?buildId=42"), null, null, null);
        this.AddSite("/pt/", build: build);

        var cut = Render<MainLayout>(p => p.Add(l => l.Body, (RenderFragment)(b => b.AddContent(0, "body"))));

        var version = cut.Find(".site-version");
        Assert.Equal($"v{SiteVersion.Current} · build 20261122.3", Normalize(version.TextContent));
        Assert.Null(version.QuerySelector("a"));
    }

    [Fact]
    public void Footer_OnALocalBuild_SaysLocal()
    {
        this.AddSite("/en/");

        var cut = Render<MainLayout>(p => p.Add(l => l.Body, (RenderFragment)(b => b.AddContent(0, "body"))));

        var version = cut.Find(".site-version");
        Assert.Equal($"v{SiteVersion.Current} · build local", Normalize(version.TextContent));
        Assert.Null(version.QuerySelector("a"));
    }

    [Theory]
    [InlineData("1.2.0+439189f9abcdef", "1.2.0")]
    [InlineData("1.0.0", "1.0.0")]
    [InlineData(null, "0.0.0")]
    [InlineData("", "0.0.0")]
    public void SiteVersion_DropsTheCommitSuffix(string? informational, string expected) =>
        Assert.Equal(expected, SiteVersion.Parse(informational));

    [Fact]
    public void SiteVersion_ComesFromTheBuildProps() =>
        Assert.Matches(@"^\d+\.\d+\.\d+", SiteVersion.Current);

    private static string Normalize(string text) => string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
