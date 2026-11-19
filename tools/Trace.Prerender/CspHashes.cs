using System.Security.Cryptography;
using System.Text;

namespace Trace.Prerender;

/// <summary>CSP source expressions for inline scripts (spike finding 3, ADR-0003).</summary>
internal static class CspHashes
{
    /// <summary>
    /// <c>'sha256-…'</c> for an inline script. Browsers hash the text after the HTML parser
    /// turns CRLF and CR into LF, so hashing raw Windows output would give a value that never matches.
    /// </summary>
    public static string ForInlineScript(string content)
    {
        var normalized = content.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return $"'sha256-{Convert.ToBase64String(hash)}'";
    }
}
