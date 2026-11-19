using System.Text.Json;

namespace Trace.Domain.Media;

public sealed record FocalPoint(double X, double Y);

public sealed record Credit(string Name, Uri? Url);

/// <summary>
/// One image from <c>content/media/gallery.json</c> (ADR-0008). Alt text is keyed by locale.
/// <c>AiGenerated</c> images are labelled as such on the site instead of showing a photographer credit.
/// </summary>
public sealed record GalleryItem(
    string Id,
    string File,
    string Category,
    bool Featured,
    FocalPoint? FocalPoint,
    Credit? Credit,
    bool AiGenerated,
    IReadOnlyDictionary<string, string> Alt,
    IReadOnlyDictionary<string, string>? Caption);

public static class Gallery
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<GalleryItem> Load(string manifestPath)
    {
        using var stream = File.OpenRead(manifestPath);
        using var document = JsonDocument.Parse(stream);
        return document.RootElement.GetProperty("items").Deserialize<List<GalleryItem>>(Json)
            ?? throw new InvalidDataException($"{manifestPath} has no items.");
    }
}
