namespace Trace.Prerender.Tests;

public sealed class HtmlAnalyzerTests
{
    private const string Page = """
        <!DOCTYPE html>
        <html lang="en"><head>
          <base href="/">
          <link rel="stylesheet" href="app.abc.css">
          <script type="importmap">{ "imports": { "./_framework/dotnet.js": "./_framework/dotnet.h1.js", "_framework/rc.js.gz": "./_framework/rc.js.gz" } }</script>
          <script type="application/ld+json">{ "@type": "Person" }</script>
          <link rel="alternate" hreflang="pt-BR" href="https://www.manuelbravo.com.br/">
          <link rel="alternate" hreflang="en" href="https://www.manuelbravo.com.br/en/">
        </head><body>
          <a href="/en/colophon">Colophon</a>
          <a href="#top">Top</a>
          <a href="https://www.linkedin.com/in/manuelbravoc/">LinkedIn</a>
          <a href="/files/cv.pdf">CV</a>
          <img src="/img/a.480.avif" srcset="/img/a.480.avif 480w, /img/a.960.avif 960w" alt="">
          <script src="_framework/blazor.web.h2.js"></script>
          <script>console.log(1)</script>
        </body></html>
        """;

    private static PageAnalysis Analyze() => HtmlAnalyzer.Analyze(Page, new Uri("http://localhost/en/"));

    [Fact]
    public void Analyze_ReadsLang() => Assert.Equal("en", Analyze().Lang);

    [Fact]
    public void Analyze_ReadsHreflangAlternates() =>
        Assert.Equal("https://www.manuelbravo.com.br/en/", Analyze().Alternates["en"]);

    [Fact]
    public void Analyze_PageLinks_AreSameSiteCanonicalPagesOnly() =>
        Assert.Equal(["/en/colophon/"], Analyze().PageLinks);

    [Fact]
    public void Analyze_AssetReferences_ResolveAgainstBaseAndSkipCompressed()
    {
        Assert.Equal(
            ["/_framework/blazor.web.h2.js", "/_framework/dotnet.h1.js", "/app.abc.css", "/files/cv.pdf", "/img/a.480.avif", "/img/a.960.avif"],
            Analyze().AssetReferences);
    }

    [Fact]
    public void Analyze_InlineScripts_IncludeImportMapButNotJsonLd()
    {
        var scripts = Analyze().InlineScripts;

        Assert.Equal(2, scripts.Count);
        Assert.Contains(scripts, s => s.Contains("imports", StringComparison.Ordinal));
        Assert.DoesNotContain(scripts, s => s.Contains("@type", StringComparison.Ordinal));
    }

    [Fact]
    public void Analyze_NoLang_IsNull() =>
        Assert.Null(HtmlAnalyzer.Analyze("<html><body></body></html>", new Uri("http://localhost/")).Lang);
}
