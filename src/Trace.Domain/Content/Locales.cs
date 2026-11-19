namespace Trace.Domain.Content;

/// <summary>The site's three languages (ADR-0004). PT-BR is the default: it lives at the root of a .com.br domain.</summary>
public static class Locales
{
    public const string Portuguese = "pt-BR";
    public const string English = "en";
    public const string Spanish = "es";

    /// <summary>In switcher order, as Manuel chose it: EN · ES · PT. The default (root URL) is still PT-BR.</summary>
    public static readonly IReadOnlyList<string> All = [English, Spanish, Portuguese];

    public const string Default = Portuguese;

    /// <summary>The <c>hreflang="x-default"</c> target: English for visitors whose language isn't offered.</summary>
    public const string XDefault = English;

    public static bool IsSupported(string? locale) => locale is not null && All.Contains(locale);
}
