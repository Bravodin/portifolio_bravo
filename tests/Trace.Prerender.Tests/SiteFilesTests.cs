using System.Text.Json.Nodes;

namespace Trace.Prerender.Tests;

public sealed class SiteFilesTests
{
    private static readonly Uri BaseUrl = new("https://www.manuelbravo.com.br/");

    [Fact]
    public void Sitemap_ListsAbsoluteUrlsInOrder()
    {
        var xml = SiteFiles.Sitemap(BaseUrl, ["/en/colophon/", "/"]);

        Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>", xml, StringComparison.Ordinal);
        var home = xml.IndexOf("<loc>https://www.manuelbravo.com.br/</loc>", StringComparison.Ordinal);
        var colophon = xml.IndexOf("<loc>https://www.manuelbravo.com.br/en/colophon/</loc>", StringComparison.Ordinal);
        Assert.True(home > 0 && colophon > home);
    }

    [Fact]
    public void Robots_PointsAtSitemap() =>
        Assert.Contains("Sitemap: https://www.manuelbravo.com.br/sitemap.xml", SiteFiles.Robots(BaseUrl), StringComparison.Ordinal);

    [Fact]
    public void StaticWebAppConfig_FillsHashesAndPrependsImmutableRoutes()
    {
        const string template = """
            {
              "routes": [ { "route": "/old/*", "headers": { "X": "1" } } ],
              "globalHeaders": { "Content-Security-Policy": "script-src 'self' __INLINE_SCRIPT_HASHES__" }
            }
            """;

        var json = SiteFiles.StaticWebAppConfig(template, ["'sha256-b'", "'sha256-a'", "'sha256-a'"], ["_framework/x.h1.wasm"]);
        var config = JsonNode.Parse(json)!;

        Assert.Equal("script-src 'self' 'sha256-a' 'sha256-b'", (string?)config["globalHeaders"]!["Content-Security-Policy"]);
        var routes = config["routes"]!.AsArray();
        Assert.Equal("/_framework/x.h1.wasm", (string?)routes[0]!["route"]);
        Assert.Equal("public, max-age=31536000, immutable", (string?)routes[0]!["headers"]!["Cache-Control"]);
        Assert.Equal("/old/*", (string?)routes[1]!["route"]);
    }

    [Fact]
    public void StaticWebAppConfig_WithoutPlaceholder_Throws() =>
        Assert.Throws<InvalidOperationException>(() => SiteFiles.StaticWebAppConfig("{}", [], []));
}
