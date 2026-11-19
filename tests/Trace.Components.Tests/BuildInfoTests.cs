using Microsoft.Extensions.Time.Testing;
using Trace.Web.Site;

namespace Trace.Components.Tests;

public sealed class BuildInfoTests : IDisposable
{
    private readonly string _results = Path.Combine(Path.GetTempPath(), "trace-results-" + Guid.NewGuid().ToString("N"));
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 11, 19, 20, 30, 0, TimeSpan.Zero));

    public BuildInfoTests()
    {
        Directory.CreateDirectory(Path.Combine(_results, "a"));
        File.WriteAllText(Path.Combine(_results, "summary.txt"), "Test run summary: Passed!\n  total: 186\n  failed: 0\n  succeeded: 186\n  skipped: 0\n");
        File.WriteAllText(Path.Combine(_results, "a", "coverage.cobertura.191126154500938.xml"), """<coverage><packages><package name="Trace.Domain" line-rate="0.9876" /><package name="Trace.Web" line-rate="0" /></packages></coverage>""");
        File.WriteAllText(Path.Combine(_results, "b.cobertura.xml"), """<coverage><packages><package name="Trace.Domain" line-rate="0.5" /></packages></coverage>""");
    }

    public void Dispose() => Directory.Delete(_results, recursive: true);

    private Func<string, string?> Env(bool inPipeline = true) => name => (inPipeline, name) switch
    {
        (true, "TF_BUILD") => "True",
        (_, "BUILD_BUILDNUMBER") => "20261119.5",
        (_, "BUILD_SOURCEVERSION") => "439189f9abcdef",
        (_, "BUILD_SOURCEBRANCHNAME") => "main",
        (_, "SYSTEM_COLLECTIONURI") => "https://dev.azure.com/org/",
        (_, "SYSTEM_TEAMPROJECT") => "TRACE",
        (_, "BUILD_BUILDID") => "42",
        (_, "TRACE_TEST_RESULTS") => _results,
        _ => null,
    };

    [Fact]
    public void From_InThePipeline_ReadsTheRunTestsAndCoverage()
    {
        var build = BuildInfo.From(Env(), _clock)!;

        Assert.Equal("20261119.5", build.Number);
        Assert.Equal("439189f", build.ShortCommit);
        Assert.Equal("main", build.Branch);
        Assert.Equal(new Uri("https://dev.azure.com/org/TRACE/_build/results?buildId=42"), build.RunUrl);
        Assert.Equal((186, 186), (build.TestsPassed, build.TestsTotal));
        Assert.Equal(0.9876, build.DomainCoverage);
        Assert.Equal(_clock.GetUtcNow(), build.BuiltAt);
    }

    [Fact]
    public void From_OutsideThePipeline_IsNull() => Assert.Null(BuildInfo.From(Env(inPipeline: false), _clock));

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("nothing here", null, null)]
    [InlineData("  total: 10\n  failed: 2\n  succeeded: 8\n", 8, 10)]
    public void ParseSummary_ReadsTheTotals(string? text, int? passed, int? total) =>
        Assert.Equal((passed, total), BuildInfo.ParseSummary(text));

    [Fact]
    public void DomainLineRate_WithoutTheDomainPackage_IsNull() =>
        Assert.Null(BuildInfo.DomainLineRate("""<coverage><packages><package name="Other" line-rate="1" /></packages></coverage>"""));
}
