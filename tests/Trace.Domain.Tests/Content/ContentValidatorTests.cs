using System.Text.Json.Nodes;
using Trace.Domain.Content;

namespace Trace.Domain.Tests.Content;

public sealed class ContentValidatorTests : IDisposable
{
    private readonly string _root = RepoContent.CopyToTemp();

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void Load_MissingUiKeyInOneLocale_FailsWithClearMessage()
    {
        Edit("es/ui.json", json => json.AsObject().Remove("dyno.skip"));

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("es is missing ui key(s): dyno.skip", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_ExtraMetricInOneLocale_Fails()
    {
        Edit("en/metrics.json", json => json.AsArray().Add(JsonNode.Parse("""{ "id": "M99", "label": "x", "before": "1", "after": "2", "delta": "3", "where": "x" }""")));

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("en has metric(s) that pt-BR doesn't: M99", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_DifferentStagesOrLogLength_Fails()
    {
        Edit("pt-BR/stages.json", json => json[0]!["id"] = "renamed");
        Edit("es/log.json", json => json.AsArray().RemoveAt(0));

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("stage(s)", error.Message, StringComparison.Ordinal);
        Assert.Contains("log.json has 10 entries in pt-BR but 9 in es", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_StageWithUnknownOrMismatchedMetric_Fails()
    {
        Edit("pt-BR/stages.json", json => json[1]!["metric"] = "M77");

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("stage 'edge' uses metric 'M77'", error.Message, StringComparison.Ordinal);
        Assert.Contains("stage 'edge' has a different number or metric in en", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_StagePartWithUnknownOrMismatchedMetric_Fails()
    {
        Edit("pt-BR/stages.json", json => json[6]!["parts"]![2]!["metric"] = "M99");

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("stage 'pit' part 'ai' uses metric 'M99'", error.Message, StringComparison.Ordinal);
        Assert.Contains("stage 'pit' has different parts or part metrics in en", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_CaseMissingADecisionOrLayerInOneLocale_Fails()
    {
        Edit("es/stages.json", json => json[0]!["case"]!["architecture"]!["after"]![1]!["layers"]!.AsArray().RemoveAt(0));
        Edit("en/stages.json", json => json[3]!["case"]!["decisions"]!.AsArray().RemoveAt(0));

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("stage 'client' has a different case (decisions or architecture layers) in es", error.Message, StringComparison.Ordinal);
        Assert.Contains("stage 'services' has a different case (decisions or architecture layers) in en", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_AboutWithAMissingPrinciple_Fails()
    {
        Edit("en/about.json", json => json["principles"]!.AsArray().RemoveAt(0));

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("about.json has a different number of paragraphs or principles in en", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_MissingToolboxDrawer_Fails()
    {
        Edit("es/toolbox.json", json => json.AsArray().RemoveAt(0));

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("es is missing toolbox drawer(s): core", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_CaseStudyMissingOrPointingElsewhereInOneLocale_Fails()
    {
        Edit("es/cases.json", json => json.AsArray().RemoveAt(0));
        Edit("en/cases.json", json => json[1]!["repo"] = "https://github.com/someone-else/repo");

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("es is missing case study(s): erp-mock", error.Message, StringComparison.Ordinal);
        Assert.Contains("case study 'loja-mock' has a different repository, thumbnail or number of highlights in en", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_DifferentLinkedIn_Fails()
    {
        Edit("en/identity.json", json =>
        {
            json["linkedIn"] = "https://www.linkedin.com/in/someone-else/";
            json["careerStartYear"] = 1999;
        });

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("LinkedIn URL differs in en", error.Message, StringComparison.Ordinal);
        Assert.Contains("careerStartYear differs in en", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_RecruiterReferencesUnknownMetricOrTooManyRoles_Fails()
    {
        Edit("recruiter.json", json =>
        {
            json["metrics"]!.AsArray().Add("M42");
            json["roles"] = 50;
        });

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("metric 'M42'", error.Message, StringComparison.Ordinal);
        Assert.Contains("asks for 50 roles", error.Message, StringComparison.Ordinal);
    }

    private void Edit(string relativePath, Action<JsonNode> change)
    {
        var path = Path.Combine(_root, relativePath);
        var json = JsonNode.Parse(File.ReadAllText(path))!;
        change(json);
        File.WriteAllText(path, json.ToJsonString());
    }
}
