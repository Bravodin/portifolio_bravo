using Bunit;
using Microsoft.AspNetCore.Components;
using Trace.Web.Components.Layout;

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
}
