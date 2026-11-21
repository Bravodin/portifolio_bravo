using System.Text.Json;

namespace Trace.Domain.Media;

/// <summary>
/// One video from <c>content/media/videos.json</c> (ADR-0012). Titles and descriptions are keyed by locale.
/// <c>File</c> is the web-ready MP4 (e.g. <c>garage/videos/short-01-roll-out.mp4</c>) and <c>Poster</c> a frame
/// exported from it (e.g. <c>garage/short-01-roll-out-poster.jpg</c>), both relative to <c>content/media/</c>.
/// </summary>
public sealed record VideoItem(
    string Id,
    string File,
    string Category,
    bool Featured,
    FocalPoint? FocalPoint,
    string Poster,
    Credit Credit,
    IReadOnlyDictionary<string, string> Title,
    IReadOnlyDictionary<string, string> Description);

public static class Videos
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<VideoItem> Load(string manifestPath)
    {
        using var stream = System.IO.File.OpenRead(manifestPath);
        using var document = JsonDocument.Parse(stream);
        return document.RootElement.GetProperty("items").Deserialize<List<VideoItem>>(Json)
            ?? throw new InvalidDataException($"{manifestPath} has no items.");
    }
}
