using System.Text;

namespace Trace.Prerender.Tests;

public sealed class StaticAssetManifestTests
{
    private const string Manifest = """
        {
          "Version": 1,
          "ManifestType": "Publish",
          "Endpoints": [
            { "Route": "app.css", "AssetFile": "app.css", "Selectors": [],
              "ResponseHeaders": [ { "Name": "Cache-Control", "Value": "no-cache" } ] },
            { "Route": "app.css", "AssetFile": "app.css.br",
              "Selectors": [ { "Name": "Content-Encoding", "Value": "br", "Quality": "0.1" } ],
              "ResponseHeaders": [ { "Name": "Cache-Control", "Value": "no-cache" } ] },
            { "Route": "app.abc123.css", "AssetFile": "app.css", "Selectors": [],
              "ResponseHeaders": [ { "Name": "Cache-Control", "Value": "max-age=31536000, immutable" } ] },
            { "Route": "app.abc123.css.gz", "AssetFile": "app.css.gz", "Selectors": [],
              "ResponseHeaders": [] },
            { "Route": "_framework/dotnet.native.x1.wasm", "AssetFile": "_framework/dotnet.native.x1.wasm", "Selectors": [],
              "ResponseHeaders": [ { "Name": "Cache-Control", "Value": "max-age=31536000, immutable" } ] }
          ]
        }
        """;

    private static IReadOnlyList<StaticAssetEndpoint> Parse() =>
        StaticAssetManifest.Parse(new MemoryStream(Encoding.UTF8.GetBytes(Manifest)));

    [Fact]
    public void Parse_KeepsOneIdentityEndpointPerRoute()
    {
        var routes = Parse().Select(e => e.Route).ToList();

        Assert.Equal(["app.css", "app.abc123.css", "_framework/dotnet.native.x1.wasm"], routes);
    }

    [Fact]
    public void Parse_FingerprintedRoute_PointsAtOriginalFile()
    {
        var endpoint = Parse().Single(e => e.Route == "app.abc123.css");

        Assert.Equal("app.css", endpoint.AssetFile);
        Assert.True(endpoint.IsImmutable);
    }

    [Fact]
    public void Parse_PlainRoute_IsNotImmutable() =>
        Assert.False(Parse().Single(e => e.Route == "app.css").IsImmutable);

    [Theory]
    [InlineData("app.css.br", true)]
    [InlineData("app.css.GZ", true)]
    [InlineData("app.css", false)]
    public void IsCompressedRoute_DetectsPrecompressedVariants(string route, bool expected) =>
        Assert.Equal(expected, StaticAssetManifest.IsCompressedRoute(route));
}
