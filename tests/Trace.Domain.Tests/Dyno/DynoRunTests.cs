using Trace.Domain.Dyno;

namespace Trace.Domain.Tests.Dyno;

public sealed class DynoRunTests
{
    private static readonly DynoRun Run = new(TimeSpan.FromSeconds(1.2), TimeSpan.FromSeconds(15));

    [Fact]
    public void SpeedUp_IsLegacyOverNative() => Assert.Equal(12.5, Run.SpeedUp, precision: 6);

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(0.6, 0.5)]
    [InlineData(1.2, 1)]
    [InlineData(5, 1)]
    public void NativeProgress_IsClampedFraction(double seconds, double expected) =>
        Assert.Equal(expected, Run.NativeProgress(TimeSpan.FromSeconds(seconds)), precision: 6);

    [Fact]
    public void LegacyProgress_HalfWay_IsHalf() =>
        Assert.Equal(0.5, Run.LegacyProgress(TimeSpan.FromSeconds(7.5)), precision: 6);

    [Fact]
    public void Progress_ZeroDuration_IsComplete() =>
        Assert.Equal(1, DynoRun.Progress(TimeSpan.FromSeconds(3), TimeSpan.Zero));

    [Fact]
    public void IsDone_FlipsExactlyAtDuration()
    {
        Assert.False(Run.IsNativeDone(TimeSpan.FromSeconds(1.1)));
        Assert.True(Run.IsNativeDone(TimeSpan.FromSeconds(1.2)));
        Assert.False(Run.IsLegacyDone(TimeSpan.FromSeconds(14.9)));
        Assert.True(Run.IsLegacyDone(TimeSpan.FromSeconds(15)));
    }
}
