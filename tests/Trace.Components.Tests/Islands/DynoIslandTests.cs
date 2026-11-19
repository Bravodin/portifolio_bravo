using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Trace.Client.Islands;
using Trace.Domain.Content;
using Trace.Domain.Dyno;

namespace Trace.Components.Tests.Islands;

public sealed class DynoIslandTests : BunitContext
{
    private static readonly DynoCopy English = DynoCopy.From(TestContent.Store.For(Locales.English));
    private static readonly DynoCopy Portuguese = DynoCopy.From(TestContent.Store.For(Locales.Portuguese));

    private readonly FakeTimeProvider _clock = new();

    public DynoIslandTests() => Services.AddSingleton<TimeProvider>(_clock);

    [Fact]
    public void Prerendered_ShowsStaticResultCardWithMeasuredNumbers()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));

        var cut = Render<DynoIsland>(p => p.Add(x => x.Copy, English));

        var card = cut.Find("[data-testid=dyno-static]");
        Assert.Contains("12–20 s", card.TextContent, StringComparison.Ordinal);
        Assert.Contains("~1.2 s", card.TextContent, StringComparison.Ordinal);
        Assert.Contains("wearables", card.TextContent, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll("button"));
    }

    [Fact]
    public void Prerendered_Portuguese_UsesLocalizedNumbers()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));

        var cut = Render<DynoIsland>(p => p.Add(x => x.Copy, Portuguese));

        Assert.Contains("~1,2 s", cut.Find("[data-testid=dyno-static]").TextContent, StringComparison.Ordinal);
        Assert.Equal("Dinamômetro: jornada de agendamento", cut.Find("h3").TextContent);
    }

    [Fact]
    public void Interactive_Idle_ShowsRunButton()
    {
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));

        var cut = Render<DynoIsland>(p => p.Add(x => x.Copy, English));

        Assert.Equal("Run the dyno", cut.Find("button.dyno-run").TextContent);
        Assert.Equal("idle", cut.Find("section").GetAttribute("data-dyno-state"));
    }

    [Fact]
    public void Interactive_Run_AnnouncesNativeThenFinishesWithLegacy()
    {
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));
        var cut = Render<DynoIsland>(p => p.Add(x => x.Copy, English).Add(x => x.NextSeed, () => 42));

        cut.Find("button.dyno-run").Click();
        Advance(TimeSpan.FromSeconds(1.25));
        cut.WaitForAssertion(() => Assert.Contains("Native finished in 1.2 s", cut.Markup, StringComparison.Ordinal));

        Advance(TimeSpan.FromSeconds(20));
        cut.WaitForAssertion(() => Assert.Equal("done", cut.Find("section").GetAttribute("data-dyno-state")));
        Assert.Contains("Legacy finished in", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("wearables", cut.Find("[data-testid=dyno-result]").TextContent, StringComparison.Ordinal);
        Assert.Equal("Run again", cut.Find("button.dyno-run").TextContent);
    }

    [Fact]
    public void Interactive_Portuguese_AnnouncesWithDecimalComma()
    {
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));
        var cut = Render<DynoIsland>(p => p.Add(x => x.Copy, Portuguese).Add(x => x.NextSeed, () => 42));

        cut.Find("button.dyno-run").Click();
        Advance(TimeSpan.FromSeconds(1.25));

        cut.WaitForAssertion(() => Assert.Contains("Nativo terminou em 1,2 s", cut.Markup, StringComparison.Ordinal));
    }

    [Fact]
    public void Interactive_SkipAppearsAfterThreeSeconds_AndEndsTheRun()
    {
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));
        var cut = Render<DynoIsland>(p => p.Add(x => x.Copy, English).Add(x => x.NextSeed, () => 42));

        cut.Find("button.dyno-run").Click();
        Advance(TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => Assert.Empty(cut.FindAll("button.dyno-skip")));

        Advance(TimeSpan.FromSeconds(1.5));
        cut.WaitForAssertion(() => cut.Find("button.dyno-skip").Click());

        cut.WaitForAssertion(() => Assert.Equal("done", cut.Find("section").GetAttribute("data-dyno-state")));
        Assert.NotNull(cut.Find("[data-testid=dyno-result]"));
    }

    // Step in timer-sized increments so the PeriodicTimer fires on every tick.
    private void Advance(TimeSpan total)
    {
        var step = TimeSpan.FromMilliseconds(50);
        for (var done = TimeSpan.Zero; done < total; done += step)
        {
            _clock.Advance(step);
        }
    }
}
