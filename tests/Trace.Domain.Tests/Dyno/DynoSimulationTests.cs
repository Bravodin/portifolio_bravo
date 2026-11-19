using Trace.Domain.Dyno;

namespace Trace.Domain.Tests.Dyno;

public sealed class DynoSimulationTests
{
    [Fact]
    public void Create_SameSeed_ReturnsSameRun() =>
        Assert.Equal(DynoSimulation.Create(42), DynoSimulation.Create(42));

    [Fact]
    public void Create_AnySeed_NativeIsMeasuredDuration() =>
        Assert.Equal(TimeSpan.FromSeconds(1.2), DynoSimulation.Create(7).Native);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Create_AnySeed_LegacyIsWithinMeasuredRange(int seed)
    {
        var legacy = DynoSimulation.Create(seed).Legacy;

        Assert.InRange(legacy, TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(20));
    }

    [Fact]
    public void Create_ManySeeds_LegacyIsRoundedToTenths()
    {
        for (var seed = 0; seed < 500; seed++)
        {
            var tenths = DynoSimulation.Create(seed).Legacy.TotalSeconds * 10;
            Assert.Equal(Math.Round(tenths), tenths, precision: 6);
        }
    }

    [Fact]
    public void Create_ManySeeds_SpreadsAcrossTheRange()
    {
        var legacies = Enumerable.Range(0, 500).Select(seed => DynoSimulation.Create(seed).Legacy).ToList();

        Assert.True(legacies.Min() < TimeSpan.FromSeconds(13));
        Assert.True(legacies.Max() > TimeSpan.FromSeconds(19));
    }
}
