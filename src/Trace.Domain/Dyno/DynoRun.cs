namespace Trace.Domain.Dyno;

/// <summary>One dyno run: how long each lane takes to finish the booking journey.</summary>
public sealed record DynoRun(TimeSpan Native, TimeSpan Legacy)
{
    /// <summary>How many times faster the native lane is (legacy ÷ native).</summary>
    public double SpeedUp => Legacy / Native;

    public static double Progress(TimeSpan elapsed, TimeSpan duration) =>
        duration <= TimeSpan.Zero ? 1 : Math.Clamp(elapsed / duration, 0, 1);

    public double NativeProgress(TimeSpan elapsed) => Progress(elapsed, Native);

    public double LegacyProgress(TimeSpan elapsed) => Progress(elapsed, Legacy);

    public bool IsNativeDone(TimeSpan elapsed) => elapsed >= Native;

    public bool IsLegacyDone(TimeSpan elapsed) => elapsed >= Legacy;
}
