namespace Trace.Prerender.Tests;

public sealed class HreflangValidatorTests
{
    private static readonly Uri Site = new("https://www.manuelbravo.com.br/");
    private static readonly string[] Locales = ["pt-BR", "en"];

    private static RenderedPage Page(string path, string lang, Dictionary<string, string> alternates) =>
        new(path, "<html></html>", new PageAnalysis(lang, [], [], [], alternates));

    private static Dictionary<string, string> Full() => new()
    {
        ["pt-BR"] = "https://www.manuelbravo.com.br/",
        ["en"] = "https://www.manuelbravo.com.br/en/",
        ["x-default"] = "https://www.manuelbravo.com.br/en/",
    };

    [Fact]
    public void ReciprocalCompleteSet_IsValid() =>
        Assert.Empty(HreflangValidator.Validate([Page("/", "pt-BR", Full()), Page("/en/", "en", Full())], Site, Locales));

    [Fact]
    public void MissingLocaleOrXDefault_IsReported()
    {
        var partial = Full();
        partial.Remove("en");
        partial.Remove("x-default");

        var errors = HreflangValidator.Validate([Page("/", "pt-BR", partial)], Site, Locales).ToList();

        Assert.Contains("/ has no hreflang=\"en\" alternate", errors);
        Assert.Contains("/ has no hreflang=\"x-default\" alternate", errors);
    }

    [Fact]
    public void AlternateToUnrenderedPage_IsReported()
    {
        var errors = HreflangValidator.Validate([Page("/", "pt-BR", Full())], Site, Locales).ToList();

        Assert.Contains(errors, e => e.Contains("points at https://www.manuelbravo.com.br/en/, which wasn't rendered", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingReturnLink_IsReported()
    {
        var oneWay = Full();
        oneWay["pt-BR"] = "https://www.manuelbravo.com.br/elsewhere/";

        var errors = HreflangValidator.Validate([Page("/", "pt-BR", Full()), Page("/en/", "en", oneWay)], Site, Locales).ToList();

        Assert.Contains("/en/ doesn't link back to / as hreflang=\"pt-BR\"", errors);
    }

    [Fact]
    public void OwnLanguagePointingElsewhere_IsReported()
    {
        var wrong = Full();
        wrong["en"] = "https://www.manuelbravo.com.br/";

        var errors = HreflangValidator.Validate([Page("/en/", "en", wrong), Page("/", "pt-BR", Full())], Site, Locales).ToList();

        Assert.Contains(errors, e => e.StartsWith("/en/ is <html lang=\"en\"> but its hreflang=\"en\" points at", StringComparison.Ordinal));
    }

    [Fact]
    public void PageWithoutAlternates_IsSkipped() =>
        Assert.Empty(HreflangValidator.Validate([Page("/not-found/", "pt-BR", [])], Site, Locales));
}
