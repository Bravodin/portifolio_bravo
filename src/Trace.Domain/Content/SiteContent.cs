namespace Trace.Domain.Content;

/// <summary>Who Manuel is, in one locale (content/{locale}/identity.json).</summary>
public sealed record Identity(
    string Name,
    string Role,
    string Tagline,
    string Subtitle,
    string CurrentRole,
    int CareerStartYear,
    string Sectors,
    string Location,
    string Languages,
    string Education,
    Uri LinkedIn,
    string? CvUrl);

/// <summary>A measured result (content inventory §2). Values are pre-formatted per locale (ADR-0004).</summary>
public sealed record Metric(string Id, string Label, string Before, string After, string Delta, string Where);

/// <summary>One "revision" in the Service Log (content inventory §4).</summary>
public sealed record LogEntry(string Period, string Company, string Role, string Highlight);

/// <summary>A Trace stage: Problem → Action → Result (content inventory §3).</summary>
/// <remarks>
/// <c>Metaphor</c> is the motorcycle part the layer maps to ("Fuel injection" for the API gateway).
/// <c>Metric</c> is the ID of the headline metric (content inventory §2) shown as before → after.
/// </remarks>
public sealed record Stage(
    string Id,
    int Number,
    string Title,
    string Metaphor,
    string Metric,
    string Problem,
    string Action,
    string Result,
    IReadOnlyList<string> Story,
    IReadOnlyList<string> Tech);

/// <summary>A Toolbox drawer (content inventory §5). No skill bars: just the tools.</summary>
public sealed record Drawer(string Id, string Name, IReadOnlyList<string> Tools);

/// <summary>
/// A Garage card (content inventory §6): a hobby and what it says about the work. <c>Photo</c> is a gallery id;
/// cards without one show a line-art <c>Icon</c> until a photo arrives.
/// </summary>
public sealed record GarageCard(string Id, string Title, string Text, string Link, string? Photo, string? Icon);

/// <summary>Locale-neutral choices for Recruiter Mode (content/recruiter.json).</summary>
public sealed record RecruiterSelection(IReadOnlyList<string> Metrics, IReadOnlyList<string> CoreStack, int Roles);

/// <summary>Everything the site says in one locale.</summary>
public sealed record SiteContent(
    string Locale,
    Identity Identity,
    IReadOnlyList<Metric> Metrics,
    IReadOnlyList<LogEntry> Log,
    IReadOnlyList<Stage> Stages,
    IReadOnlyList<Drawer> Toolbox,
    IReadOnlyList<GarageCard> Garage,
    IReadOnlyDictionary<string, string> Ui)
{
    /// <summary>A UI string from ui.json. A missing key is a content bug, so it throws instead of rendering a blank.</summary>
    public string T(string key) =>
        Ui.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException($"content/{Locale}/ui.json has no key '{key}'.");

    public Metric Metric(string id) =>
        Metrics.FirstOrDefault(m => m.Id == id) ?? throw new KeyNotFoundException($"content/{Locale}/metrics.json has no metric '{id}'.");

    public Stage Stage(string id) =>
        Stages.FirstOrDefault(s => s.Id == id) ?? throw new KeyNotFoundException($"content/{Locale}/stages.json has no stage '{id}'.");
}
