using System.Text.Json;

namespace Trace.Domain.Content;

/// <summary>
/// Loads <c>content/{locale}/*.json</c> and <c>content/recruiter.json</c> (ADR-0004: content as data).
/// Loading validates that every locale says the same things, so a missing translation fails the build.
/// </summary>
public sealed class ContentStore
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private readonly Dictionary<string, SiteContent> _byLocale;

    private ContentStore(Dictionary<string, SiteContent> byLocale, RecruiterSelection recruiter)
    {
        _byLocale = byLocale;
        Recruiter = recruiter;
    }

    public RecruiterSelection Recruiter { get; }

    public SiteContent For(string locale) =>
        _byLocale.TryGetValue(locale, out var content)
            ? content
            : throw new ArgumentOutOfRangeException(nameof(locale), locale, "Unsupported locale.");

    public static ContentStore Load(string root)
    {
        var byLocale = Locales.All.ToDictionary(locale => locale, locale => LoadLocale(root, locale), StringComparer.Ordinal);
        var recruiter = Read<RecruiterSelection>(Path.Combine(root, "recruiter.json"));
        var store = new ContentStore(byLocale, recruiter);

        var problems = ContentValidator.Validate(store).ToList();
        return problems.Count == 0
            ? store
            : throw new InvalidDataException("Content is inconsistent:\n- " + string.Join("\n- ", problems));
    }

    private static SiteContent LoadLocale(string root, string locale)
    {
        var folder = Path.Combine(root, locale);
        return new SiteContent(
            locale,
            Read<Identity>(Path.Combine(folder, "identity.json")),
            Read<About>(Path.Combine(folder, "about.json")),
            Read<List<Metric>>(Path.Combine(folder, "metrics.json")),
            Read<List<LogEntry>>(Path.Combine(folder, "log.json")),
            Read<List<Stage>>(Path.Combine(folder, "stages.json")),
            Read<List<Drawer>>(Path.Combine(folder, "toolbox.json")),
            Read<List<GarageCard>>(Path.Combine(folder, "garage.json")),
            Read<List<CaseStudy>>(Path.Combine(folder, "cases.json")),
            Read<ColophonCopy>(Path.Combine(folder, "colophon.json")),
            Read<Dictionary<string, string>>(Path.Combine(folder, "ui.json")));
    }

    private static T Read<T>(string path)
    {
        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream, Json)
            ?? throw new InvalidDataException($"{path} is empty.");
    }
}
