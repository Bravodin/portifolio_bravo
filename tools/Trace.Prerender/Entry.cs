using System.Globalization;

namespace Trace.Prerender;

/// <summary>
/// Usage: <c>dotnet run --project tools/Trace.Prerender -- --publish-dir artifacts/publish --out dist</c>.
/// Run <c>dotnet publish src/Trace.Web -c Release -o artifacts/publish</c> first.
/// </summary>
internal static class Entry
{
    public static async Task<int> Main(string[] args)
    {
        var options = ParseArguments(args);
        if (options is null)
        {
            await Console.Error.WriteLineAsync("Usage: Trace.Prerender --publish-dir <dir> [--out dist] [--base-url https://www.manuelbravo.com.br/] [--swa-config staticwebapp.config.json]");
            return 2;
        }

        var result = await new Prerenderer(options).RunAsync(CancellationToken.None);

        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"Prerendered {result.Pages} page(s) → {options.OutputDirectory}: {result.Files} files, {result.Bytes / 1024.0 / 1024.0:0.0} MB"));

        foreach (var warning in result.Warnings)
        {
            Console.WriteLine($"warning: {warning}");
        }

        foreach (var error in result.Errors)
        {
            await Console.Error.WriteLineAsync($"error: {error}");
        }

        return result.Errors.Count == 0 ? 0 : 1;
    }

    private static PrerenderOptions? ParseArguments(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["--out"] = "dist",
            ["--base-url"] = "https://www.manuelbravo.com.br/",
            ["--swa-config"] = "staticwebapp.config.json",
        };

        for (var i = 0; i + 1 < args.Length; i += 2)
        {
            values[args[i]] = args[i + 1];
        }

        return values.TryGetValue("--publish-dir", out var publish) && Directory.Exists(publish)
            ? new PrerenderOptions(
                Path.GetFullPath(publish),
                Path.GetFullPath(values["--out"]),
                new Uri(values["--base-url"]),
                Path.GetFullPath(values["--swa-config"]),
                IsCiBuild: string.Equals(Environment.GetEnvironmentVariable("TF_BUILD"), "true", StringComparison.OrdinalIgnoreCase))
            : null;
    }
}
