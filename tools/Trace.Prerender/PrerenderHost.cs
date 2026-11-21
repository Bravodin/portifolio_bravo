using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace Trace.Prerender;

/// <summary>
/// Hosts Trace.Web in memory (TestServer: no ports, no waits) on top of the <c>dotnet publish</c> output,
/// so the HTML references exactly the fingerprinted files that ship.
/// </summary>
internal sealed class PrerenderHost(string publishDirectory) : WebApplicationFactory<Program>
{
    public const string ManifestFileName = "Trace.Web.staticwebassets.endpoints.json";

    public string ManifestPath => Path.Combine(publishDirectory, ManifestFileName);

    public string WebRoot => Path.Combine(publishDirectory, "wwwroot");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.UseContentRoot(publishDirectory);
        builder.UseWebRoot(WebRoot);
        builder.UseSetting("Trace:StaticAssetsManifest", ManifestPath);
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
    }
}
