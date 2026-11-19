using Trace.Domain.Content;

namespace Trace.Domain.Dyno;

/// <summary>
/// Every string the Dyno island shows, in one locale. It's a plain record so the prerendered page can
/// serialize it into the island's parameters: the WASM side never loads content files itself.
/// </summary>
public sealed record DynoCopy(
    string Title,
    string LegacyLabel,
    string NativeLabel,
    string RunLabel,
    string RunAgainLabel,
    string SkipLabel,
    string FasterLabel,
    string Footnote,
    string NativeFinished,
    string LegacyFinished,
    string DecimalSeparator,
    string MeasuredBefore,
    string MeasuredAfter,
    string MeasuredDelta,
    string BeyondSpeed)
{
    public static DynoCopy From(SiteContent content)
    {
        var m1 = content.Metric("M1");
        return new DynoCopy(
            content.T("dyno.title"),
            content.T("dyno.legacy"),
            content.T("dyno.native"),
            content.T("dyno.run"),
            content.T("dyno.runAgain"),
            content.T("dyno.skip"),
            content.T("dyno.faster"),
            content.T("dyno.footnote"),
            content.T("dyno.nativeFinished"),
            content.T("dyno.legacyFinished"),
            content.T("dyno.decimal"),
            m1.Before,
            m1.After,
            m1.Delta,
            content.Stage("client").Story[^1]);
    }
}
