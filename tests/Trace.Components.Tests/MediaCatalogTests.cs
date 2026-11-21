using Trace.Domain.Media;
using Trace.Web.Site;

namespace Trace.Components.Tests;

public sealed class MediaCatalogTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "trace-media-" + Guid.NewGuid().ToString("N"));

    public MediaCatalogTests()
    {
        Directory.CreateDirectory(Path.Combine(_root, "garage", "videos"));
        File.Copy(Path.Combine(MediaCatalog.FindRoot(null), "profile", "manuel-bravo.jpg"), Path.Combine(_root, "garage", "frame.jpg"));
        File.WriteAllBytes(Path.Combine(_root, "garage", "videos", "clip.mp4"), [1, 2, 3]);
        File.WriteAllText(Path.Combine(_root, "gallery.json"), """{ "items": [] }""");
        File.WriteAllText(Path.Combine(_root, "videos.json"), """
            { "items": [
              { "id": "own", "file": "garage/videos/clip.mp4", "category": "track", "poster": "garage/frame.jpg",
                "credit": { "name": "x" }, "title": { "en": "t" }, "description": { "en": "d" } },
              { "id": "gone", "file": "garage/videos/missing.mp4", "category": "track", "poster": "garage/missing.jpg",
                "credit": { "name": "x" }, "title": { "en": "t" }, "description": { "en": "d" } }
            ] }
            """);
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void Poster_FromOwnFrame_IsCutVertically()
    {
        var catalog = new MediaCatalog(_root);

        var poster = catalog.Poster(catalog.Videos[0], [270]);

        Assert.NotNull(poster);
        var variant = poster.Variants[0];
        Assert.Equal(new CropBox(62, 0, 238, 423), variant.Crop);
        Assert.StartsWith("/media/own-v-238.", variant.Url, StringComparison.Ordinal);
        Assert.Null(poster.Alt);
    }

    [Fact]
    public void VideoUrl_IsHashed_AndRememberedForThePrerender()
    {
        var catalog = new MediaCatalog(_root);

        var url = catalog.VideoUrl(catalog.Videos[0]);

        Assert.Matches(@"^/media/own\.[0-9a-f]{10}\.mp4$", url);
        Assert.True(catalog.TryGetIssuedFile(url, out var source));
        Assert.Equal(Path.Combine(_root, "garage", "videos", "clip.mp4"), source);
        Assert.False(catalog.TryGetIssuedFile("/media/other.mp4", out _));
    }

    [Fact]
    public void Video_WithMissingFiles_SaysWhichEntryIsWrong()
    {
        var catalog = new MediaCatalog(_root);

        var video = Assert.Throws<FileNotFoundException>(() => catalog.VideoUrl(catalog.Videos[1]));
        var poster = Assert.Throws<FileNotFoundException>(() => catalog.Poster(catalog.Videos[1], [270]));

        Assert.Contains("'gone'", video.Message, StringComparison.Ordinal);
        Assert.Contains("garage/missing.jpg", poster.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Image_UnknownId_Throws() =>
        Assert.Throws<KeyNotFoundException>(() => new MediaCatalog(_root).Image("nope", ImageShape.Square, [160]));
}
