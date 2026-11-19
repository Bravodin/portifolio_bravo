namespace Trace.Domain.Dyno;

/// <summary>
/// Simulates the stage 1 booking journey (content inventory M1): a 2016 Ionic app took 12–20 s,
/// the native rewrite takes ~1.2 s. No real systems are called.
/// </summary>
public static class DynoSimulation
{
    public static readonly TimeSpan NativeDuration = TimeSpan.FromSeconds(1.2);
    public static readonly TimeSpan LegacyMinimum = TimeSpan.FromSeconds(12);
    public static readonly TimeSpan LegacyMaximum = TimeSpan.FromSeconds(20);

    /// <summary>
    /// Creates a run whose legacy duration is drawn from the measured 12–20 s range,
    /// rounded to tenths of a second. The same seed always gives the same run.
    /// </summary>
    public static DynoRun Create(int seed)
    {
#pragma warning disable CA5394 // Not security-sensitive: a seeded, reproducible simulation.
        var sample = new Random(seed).NextDouble();
#pragma warning restore CA5394
        var range = LegacyMaximum - LegacyMinimum;
        var tenths = Math.Round((LegacyMinimum + (range * sample)).TotalSeconds * 10, MidpointRounding.AwayFromZero);
        return new DynoRun(NativeDuration, TimeSpan.FromSeconds(tenths / 10));
    }
}
