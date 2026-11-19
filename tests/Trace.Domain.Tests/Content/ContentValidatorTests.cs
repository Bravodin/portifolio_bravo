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
        Assert.Contains("log.json has 9 entries in pt-BR but 8 in es", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Load_DifferentLinkedIn_Fails()
    {
        Edit("en/identity.json", json => json["linkedIn"] = "https://www.linkedin.com/in/someone-else/");

        var error = Assert.Throws<InvalidDataException>(() => ContentStore.Load(_root));
        Assert.Contains("LinkedIn URL differs in en", error.Message, StringComparison.Ordinal);
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
