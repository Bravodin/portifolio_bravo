using Bunit;
using Trace.Web.Components.Pages;

namespace Trace.Components.Tests.Pages;

public sealed class RecruiterPageTests : BunitContext
{
    [Fact]
    public void English_ShowsIdentityResultsStackAndRoles()
    {
        this.AddSite("/en/recruiter/");

        var cut = Render<Recruiter>();

        Assert.Equal("Manuel Bravo", cut.Find("h1").TextContent);
        Assert.Equal(6, cut.FindAll(".metric").Count);
        Assert.Equal("~1.2 s", cut.Find("[data-metric=M1] .metric-after").TextContent);
        Assert.Equal(3, cut.FindAll(".roles li").Count);
        Assert.Contains("20 years · since 2006", cut.Find(".facts").TextContent, StringComparison.Ordinal);
        Assert.Equal("‹ Take the ride instead", cut.Find("a.back").TextContent);
        Assert.Equal("/en/", cut.Find("a.back").GetAttribute("href"));
    }

    [Fact]
    public void Portuguese_IsFullyTranslated()
    {
        this.AddSite("/recrutador/");

        var cut = Render<Recruiter>();

        Assert.Contains("20 anos · desde 2006", cut.Find(".facts").TextContent, StringComparison.Ordinal);
        Assert.Equal("~1,2 s", cut.Find("[data-metric=M1] .metric-after").TextContent);
        Assert.Equal("Resultados-chave (medidos)", cut.Find("#recruiter-results").TextContent);
        Assert.Equal("/", cut.Find("a.back").GetAttribute("href"));
    }

    [Fact]
    public void Portrait_IsResponsiveSizedAndPrioritized()
    {
        this.AddSite("/recrutador/");

        var picture = Render<Recruiter>().Find("picture.portrait");

        Assert.Equal(["image/avif", "image/webp"], picture.QuerySelectorAll("source").Select(s => s.GetAttribute("type")));
        Assert.Contains("-sq-320.", picture.QuerySelector("source")!.GetAttribute("srcset"), StringComparison.Ordinal);
        var img = picture.QuerySelector("img")!;
        Assert.Equal(("112", "112"), (img.GetAttribute("width"), img.GetAttribute("height")));
        Assert.Equal(("eager", "high"), (img.GetAttribute("loading"), img.GetAttribute("fetchpriority")));
        Assert.StartsWith("Retrato de Manuel Bravo", img.GetAttribute("alt"), StringComparison.Ordinal);
    }

    [Fact]
    public void ContactIsLinkedInOnly()
    {
        this.AddSite("/es/reclutador/");

        var cut = Render<Recruiter>();

        Assert.Equal("https://www.linkedin.com/in/manuelbravoc/", cut.Find(".id-actions a.btn-primary").GetAttribute("href"));
        Assert.Empty(cut.FindAll("a[href^='mailto:'], a[href^='tel:']"));
    }

    [Fact]
    public void MetricWithDifferentDelta_ShowsIt_OtherwiseNot()
    {
        this.AddSite("/en/recruiter/");

        var cut = Render<Recruiter>();

        Assert.Equal("−30 %", cut.Find("[data-metric=M3] .metric-delta").TextContent);
        Assert.Empty(cut.FindAll("[data-metric=M2] .metric-delta"));
    }

    [Fact]
    public void CvButton_LinksToThePdf()
    {
        this.AddSite("/en/recruiter/");

        var cut = Render<Recruiter>();

        Assert.Equal("/cv/manuel-bravo-cv.pdf", cut.Find("a[download]").GetAttribute("href"));
        Assert.Equal("Download CV (PDF)", cut.Find("a[download]").TextContent);
    }
}
