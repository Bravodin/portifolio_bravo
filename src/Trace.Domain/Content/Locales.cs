namespace Trace.Domain.Content;

/// <summary>The site's three languages (ADR-0004). English is the default and lives at the root URL (ADR-0011).</summary>
public static class Locales
{
    public const string Portuguese = "pt-BR";
    public const string English = "en";
    public const string Spanish = "es";

    /// <summary>In switcher order, as Manuel chose it: EN · ES · PT.</summary>
    public static readonly IReadOnlyList<string> All = [English, Spanish, Portuguese];

    public const string Default = English;

    /// <summary>
    /// The locale the others are compared against when validating content. Any locale would do; PT-BR keeps
    /// the validator's messages stable now that the default moved to English.
    /// </summary>
    public const string ContentReference = Portuguese;

    /// <summary>The <c>hreflang="x-default"</c> target: English for visitors whose language isn't offered.</summary>
    public const string XDefault = English;

    public static bool IsSupported(string? locale) => locale is not null && All.Contains(locale);
}
