using System.Text.Json;

namespace Trace.Domain.Media;

/// <summary>
/// One video from <c>content/media/videos.json</c> (ADR-0010). Titles and descriptions are keyed by locale.
/// <c>Poster</c> (e.g. <c>garage/short-01-frame.jpg</c>) replaces YouTube's thumbnail with a frame of our choosing.
/// </summary>
public sealed record VideoItem(
    string Id,
    string YoutubeId,
    string Category,
    bool Featured,
    FocalPoint? FocalPoint,
    string? Poster,
    Credit Credit,
    IReadOnlyDictionary<string, string> Title,
    IReadOnlyDictionary<string, string> Description)
{
    public Uri WatchUrl => new($"https://www.youtube.com/shorts/{YoutubeId}");
}

public static class Videos
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<VideoItem> Load(string manifestPath)
    {
        using var stream = File.OpenRead(manifestPath);
        using var document = JsonDocument.Parse(stream);
        return document.RootElement.GetProperty("items").Deserialize<List<VideoItem>>(Json)
            ?? throw new InvalidDataException($"{manifestPath} has no items.");
    }
}
