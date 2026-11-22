using System.Reflection;

namespace Trace.Web.Site;

/// <summary>The site's version (&lt;Version&gt; in Directory.Build.props), shown in the footer.</summary>
public static class SiteVersion
{
    public static string Current { get; } = Parse(
        typeof(SiteVersion).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

    /// <summary>"1.2.0+439189f…" → "1.2.0": the SDK appends the commit, which the footer shows apart.</summary>
    public static string Parse(string? informationalVersion) =>
        informationalVersion is { Length: > 0 } v ? v.Split('+', 2)[0] : "0.0.0";
}
