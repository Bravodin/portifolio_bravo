using System.Text.Json;

namespace Trace.Prerender;

/// <summary>One URL the app serves for a static file, as listed in the publish output.</summary>
/// <param name="Route">URL path without the leading slash, e.g. <c>app.wclaqum5sz.css</c>.</param>
/// <param name="AssetFile">File under <c>wwwroot/</c> that backs the route, e.g. <c>app.css</c>.</param>
/// <param name="CacheControl">The <c>Cache-Control</c> value ASP.NET would send for this route.</param>
internal sealed record StaticAssetEndpoint(string Route, string AssetFile, string? CacheControl)
{
    /// <summary>True when the route is content-addressed (a hash in the name) and safe to cache forever.</summary>
    public bool IsImmutable => CacheControl?.Contains("immutable", StringComparison.OrdinalIgnoreCase) == true;
}

/// <summary>
/// Reads <c>{App}.staticwebassets.endpoints.json</c>. <c>MapStaticAssets</c> serves fingerprinted URLs
/// (<c>app.{hash}.css</c>) that don't exist as files; this manifest maps each route to its real file
/// (spike finding 1, ADR-0003).
/// </summary>
internal static class StaticAssetManifest
{
    public static IReadOnlyList<StaticAssetEndpoint> Load(string path)
    {
        using var stream = File.OpenRead(path);
        return Parse(stream);
    }

    /// <summary>
    /// Returns one endpoint per route, identity encoding only: content-negotiated variants
    /// (<c>Selectors</c>) and <c>.br</c>/<c>.gz</c> routes are skipped because SWA compresses on its own.
    /// </summary>
    public static IReadOnlyList<StaticAssetEndpoint> Parse(Stream json)
    {
        using var document = JsonDocument.Parse(json);
        var result = new Dictionary<string, StaticAssetEndpoint>(StringComparer.Ordinal);

        foreach (var endpoint in document.RootElement.GetProperty("Endpoints").EnumerateArray())
        {
            var route = endpoint.GetProperty("Route").GetString()!;
            if (IsCompressedRoute(route) || HasSelectors(endpoint) || result.ContainsKey(route))
            {
                continue;
            }

            result[route] = new StaticAssetEndpoint(
                route,
                endpoint.GetProperty("AssetFile").GetString()!,
                Header(endpoint, "Cache-Control"));
        }

        return [.. result.Values];
    }

    public static bool IsCompressedRoute(string route) =>
        route.EndsWith(".br", StringComparison.OrdinalIgnoreCase) || route.EndsWith(".gz", StringComparison.OrdinalIgnoreCase);

    private static bool HasSelectors(JsonElement endpoint) =>
        endpoint.TryGetProperty("Selectors", out var selectors) && selectors.GetArrayLength() > 0;

    private static string? Header(JsonElement endpoint, string name)
    {
        if (!endpoint.TryGetProperty("ResponseHeaders", out var headers))
        {
            return null;
        }

        foreach (var header in headers.EnumerateArray())
        {
            if (string.Equals(header.GetProperty("Name").GetString(), name, StringComparison.OrdinalIgnoreCase))
            {
                return header.GetProperty("Value").GetString();
            }
        }

        return null;
    }
}
