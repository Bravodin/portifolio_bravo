using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Trace.Web.Site;

/// <summary>
/// The pipeline run that built this version of the site, read at build time from the Azure Pipelines
/// environment and the test step's outputs (TRACE_TEST_RESULTS). Null on a local build.
/// </summary>
public sealed partial record BuildInfo(
    string Number,
    string Commit,
    string Branch,
    DateTimeOffset BuiltAt,
    Uri? RunUrl,
    int? TestsPassed,
    int? TestsTotal,
    double? DomainCoverage)
{
    public string ShortCommit => Commit.Length > 7 ? Commit[..7] : Commit;

    /// <summary>Reads the current process environment; null when not running in Azure Pipelines.</summary>
    public static BuildInfo? FromEnvironment(TimeProvider clock) =>
        From(name => Environment.GetEnvironmentVariable(name), clock);

    public static BuildInfo? From(Func<string, string?> env, TimeProvider clock)
    {
        if (!string.Equals(env("TF_BUILD"), "true", StringComparison.OrdinalIgnoreCase) || env("BUILD_BUILDNUMBER") is not { } number)
        {
            return null;
        }

        var results = env("TRACE_TEST_RESULTS");
        var (passed, total) = results is null ? (null, null) : ParseSummary(ReadOrNull(Path.Combine(results, "summary.txt")));
        double? coverage = results is null || !Directory.Exists(results)
            ? null
            : Directory.EnumerateFiles(results, "*cobertura*.xml", SearchOption.AllDirectories)
                .Select(f => DomainLineRate(File.ReadAllText(f)))
                .Max();

        return new BuildInfo(
            number,
            env("BUILD_SOURCEVERSION") ?? "",
            env("BUILD_SOURCEBRANCHNAME") ?? "",
            clock.GetUtcNow(),
            RunUrlFrom(env("SYSTEM_COLLECTIONURI"), env("SYSTEM_TEAMPROJECT"), env("BUILD_BUILDID")),
            passed,
            total,
            coverage);
    }

    /// <summary>Totals from the Microsoft.Testing.Platform summary ("total: 186", "succeeded: 186").</summary>
    public static (int? Passed, int? Total) ParseSummary(string? text)
    {
        if (text is null)
        {
            return (null, null);
        }

        int? Read(string label) =>
            SummaryLine().Matches(text).Where(m => m.Groups[1].Value == label).Select(m => (int?)int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture)).LastOrDefault();

        return (Read("succeeded"), Read("total"));
    }

    /// <summary>Line coverage of the Trace.Domain package in a Cobertura report (0–1), or null when absent.</summary>
    public static double? DomainLineRate(string cobertura)
    {
        var package = XDocument.Parse(cobertura).Descendants("package")
            .FirstOrDefault(p => (string?)p.Attribute("name") == "Trace.Domain");
        return package?.Attribute("line-rate") is { } rate ? double.Parse(rate.Value, CultureInfo.InvariantCulture) : null;
    }

    private static Uri? RunUrlFrom(string? collection, string? project, string? buildId) =>
        collection is null || project is null || buildId is null
            ? null
            : new Uri($"{collection.TrimEnd('/')}/{Uri.EscapeDataString(project)}/_build/results?buildId={buildId}");

    private static string? ReadOrNull(string path) => File.Exists(path) ? File.ReadAllText(path) : null;

    [GeneratedRegex(@"^\s*(total|failed|succeeded|skipped):\s*(\d+)\s*$", RegexOptions.Multiline)]
    private static partial Regex SummaryLine();
}

/// <summary>DI wrapper: the build that produced this site, or none for a local build.</summary>
public sealed record BuildSnapshot(BuildInfo? Build);
