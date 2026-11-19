namespace Trace.Prerender;

/// <summary>Maps site URLs to files in dist/ and back.</summary>
internal static class SitePaths
{
    /// <summary>
    /// Canonical form of a page path: leading and trailing slash, no query or fragment.
    /// <c>/en/colophon</c> and <c>/en/colophon/</c> are the same page, and SWA serves it from a folder.
    /// </summary>
    public static string CanonicalPage(string path)
    {
        var clean = path.Split('?', '#')[0];
        if (!clean.StartsWith('/'))
        {
            clean = "/" + clean;
        }

        return clean.EndsWith('/') ? clean : clean + "/";
    }

    /// <summary><c>/</c> → <c>index.html</c>; <c>/en/colophon/</c> → <c>en/colophon/index.html</c>.</summary>
    public static string PageFile(string canonicalPage) =>
        canonicalPage.Trim('/') is { Length: > 0 } folder ? $"{folder}/index.html" : "index.html";

    /// <summary>
    /// True for links that point at pages (no file extension in the last segment),
    /// false for assets such as <c>app.css</c> or <c>cv.pdf</c>.
    /// </summary>
    public static bool LooksLikePage(string path)
    {
        var clean = path.Split('?', '#')[0].TrimEnd('/');
        var last = clean[(clean.LastIndexOf('/') + 1)..];
        return !last.Contains('.', StringComparison.Ordinal);
    }

    /// <summary>A path inside dist/ for a URL path, using the OS separator.</summary>
    public static string ToDistPath(string distRoot, string relativeUrlPath) =>
        Path.Combine(distRoot, relativeUrlPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
}
