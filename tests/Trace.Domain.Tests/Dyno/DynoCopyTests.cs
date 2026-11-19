using Trace.Domain.Content;
using Trace.Domain.Dyno;
using Trace.Domain.Tests.Content;

namespace Trace.Domain.Tests.Dyno;

public sealed class DynoCopyTests
{
    private static readonly ContentStore Store = ContentStore.Load(RepoContent.Root);

    [Fact]
    public void From_Portuguese_UsesLocalizedLabelsAndMeasuredNumbers()
    {
        var copy = DynoCopy.From(Store.For(Locales.Portuguese));

        Assert.Equal("Rodar o dinamômetro", copy.RunLabel);
        Assert.Equal(",", copy.DecimalSeparator);
        Assert.Equal("12–20 s", copy.MeasuredBefore);
        Assert.Equal("~1,2 s", copy.MeasuredAfter);
        Assert.Contains("wearables", copy.BeyondSpeed, StringComparison.Ordinal);
    }

    [Fact]
    public void From_English_UsesDotDecimal() =>
        Assert.Equal(".", DynoCopy.From(Store.For(Locales.English)).DecimalSeparator);
}
