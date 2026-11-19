using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Trace.Prerender;

/// <summary>Generates the files that sit next to the pages: sitemap, robots and the SWA config.</summary>
internal static class SiteFiles
{
    public const string InlineScriptHashesPlaceholder = "__INLINE_SCRIPT_HASHES__";

    // Keep quotes readable in the CSP header (the default encoder escapes them as 0027).
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private static readonly XNamespace SitemapNs = "http://www.sitemaps.org/schemas/sitemap/0.9";

    public static string Sitemap(Uri baseUrl, IEnumerable<string> canonicalPages)
    {
        var urls = canonicalPages
            .Order(StringComparer.Ordinal)
            .Select(page => new XElement(SitemapNs + "url", new XElement(SitemapNs + "loc", new Uri(baseUrl, page).AbsoluteUri)));

        var document = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(SitemapNs + "urlset", urls));
        return document.Declaration + "\n" + document.Root + "\n";
    }

    public static string Robots(Uri baseUrl) =>
        $"User-agent: *\nAllow: /\n\nSitemap: {new Uri(baseUrl, "/sitemap.xml").AbsoluteUri}\n";

    /// <summary>
    /// Fills the repo's <c>staticwebapp.config.json</c> template:
    /// <list type="bullet">
    /// <item>replaces <see cref="InlineScriptHashesPlaceholder"/> with the hashes of every inline script;</item>
    /// <item>adds one route per immutable (fingerprinted) file with long-lived caching, ahead of the template's
    /// routes, so files without a hash in their name never get <c>immutable</c> (spike finding 4).</item>
    /// </list>
    /// </summary>
    public static string StaticWebAppConfig(string template, IEnumerable<string> inlineScriptHashes, IEnumerable<string> immutableRoutes)
    {
        var hashes = string.Join(' ', inlineScriptHashes.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal));
        if (!template.Contains(InlineScriptHashesPlaceholder, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"staticwebapp.config.json template has no {InlineScriptHashesPlaceholder} placeholder.");
        }

        var config = JsonNode.Parse(template.Replace(InlineScriptHashesPlaceholder, hashes, StringComparison.Ordinal))!.AsObject();

        var routes = new JsonArray();
        foreach (var route in immutableRoutes.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal))
        {
            routes.Add(new JsonObject
            {
                ["route"] = "/" + route.TrimStart('/'),
                ["headers"] = new JsonObject { ["Cache-Control"] = "public, max-age=31536000, immutable" },
            });
        }

        if (config["routes"] is JsonArray existing)
        {
            foreach (var route in existing.ToList())
            {
                existing.Remove(route);
                routes.Add(route);
            }
        }

        config["routes"] = routes;
        return config.ToJsonString(JsonOptions) + "\n";
    }

    public static async Task WriteAsync(string path, string content, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), cancellationToken);
    }
}
