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

/// <summary>One "how I work" principle.</summary>
public sealed record Principle(string Title, string Text);

/// <summary>"About me" (content inventory §1.1, from Manuel; no numbers beyond the career log).</summary>
public sealed record About(string Title, IReadOnlyList<string> Paragraphs, string PrinciplesTitle, IReadOnlyList<Principle> Principles);

/// <summary>One "who did what" line in the Colophon.</summary>
public sealed record Role(string Who, string What);

/// <summary>A number with its label, pre-formatted per locale (e.g. "156,5 milhões" / "tokens processados").</summary>
public sealed record Figure(string Value, string Label);

/// <summary>
/// One step of the architecture, with the ADR numbers ("0009") that explain it. <c>Layer</c> names its swimlane;
/// <c>Flow</c> is "publish" (every push), "access" (every visit) or "both" (where the two meet).
/// </summary>
public sealed record ArchitectureNode(string Name, string Role, IReadOnlyList<string> Details, IReadOnlyList<string> Adrs, string Layer, string Flow);

/// <summary>Labels for the Colophon's "latest build" panel (the values come from the pipeline at build time).</summary>
public sealed record BuildCopy(string Title, string Intro, string Number, string Commit, string Branch, string BuiltAt, string Tests, string Coverage, string Run, string Local);

/// <summary>The Colophon's architecture diagram: an ordered flow from Manuel to the browser.</summary>
public sealed record Architecture(
    string Title,
    string Intro,
    string DecisionsLabel,
    IReadOnlyList<ArchitectureNode> Nodes,
    string ViewLabel,
    string GraphView,
    string ListView,
    string PublishFlow,
    string AccessFlow);

/// <summary>
/// The Colophon's "built with AI, directed by a human" section (content inventory §1.2). Usage figures come from
/// Manuel's Claude Code usage report; repository figures are measured at build time, so only their labels live here.
/// </summary>
public sealed record ColophonCopy(
    string AiTitle,
    string AiIntro,
    IReadOnlyList<Role> Roles,
    string ReviewsTitle,
    IReadOnlyList<string> Reviews,
    string NumbersTitle,
    string UsagePeriod,
    IReadOnlyList<Figure> Usage,
    string UsageNote,
    string RepoCommits,
    string RepoAdrs,
    string RepoTests,
    string History,
    Architecture Architecture,
    BuildCopy Build);

/// <summary>A measured result (content inventory §2). Values are pre-formatted per locale (ADR-0004).</summary>
public sealed record Metric(string Id, string Label, string Before, string After, string Delta, string Where);

/// <summary>One "revision" in the Service Log (content inventory §4).</summary>
public sealed record LogEntry(string Period, string Company, string Role, string Highlight);

/// <summary>A Trace stage: Problem → Action → Result (content inventory §3).</summary>
/// <remarks><c>Metric</c> is the ID of the headline metric (content inventory §2) shown as before → after.</remarks>
public sealed record Stage(
    string Id,
    int Number,
    string Title,
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
public sealed record GarageCard(string Id, string Title, string Text, string Link, string? Photo, string? Icon, IReadOnlyList<FavoriteGroup>? Favorites);

/// <summary>
/// A labelled list of favourites shown as text chips (e.g. "Retro": Mega Man X, Metroid…). Names only: game art,
/// logos and screenshots are copyrighted and never used on the site.
/// </summary>
public sealed record FavoriteGroup(string Label, IReadOnlyList<string> Items);

/// <summary>Locale-neutral choices for Recruiter Mode (content/recruiter.json).</summary>
public sealed record RecruiterSelection(IReadOnlyList<string> Metrics, IReadOnlyList<string> CoreStack, int Roles);

/// <summary>Everything the site says in one locale.</summary>
public sealed record SiteContent(
    string Locale,
    Identity Identity,
    About About,
    IReadOnlyList<Metric> Metrics,
    IReadOnlyList<LogEntry> Log,
    IReadOnlyList<Stage> Stages,
    IReadOnlyList<Drawer> Toolbox,
    IReadOnlyList<GarageCard> Garage,
    ColophonCopy Colophon,
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
