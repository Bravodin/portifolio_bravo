using Trace.Domain.Media;
using Trace.Web.Site;

namespace Trace.Components.Tests;

public sealed class MediaCatalogTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "trace-media-" + Guid.NewGuid().ToString("N"));

    public MediaCatalogTests()
    {
        Directory.CreateDirectory(Path.Combine(_root, "garage"));
        File.Copy(Path.Combine(MediaCatalog.FindRoot(null), "profile", "manuel-bravo.jpg"), Path.Combine(_root, "garage", "frame.jpg"));
        File.WriteAllText(Path.Combine(_root, "gallery.json"), """{ "items": [] }""");
        File.WriteAllText(Path.Combine(_root, "videos.json"), """
            { "items": [
              { "id": "own", "youtubeId": "AAAAAAAAAAA", "category": "track", "poster": "garage/frame.jpg",
                "credit": { "name": "x" }, "title": { "en": "t" }, "description": { "en": "d" } },
              { "id": "yt", "youtubeId": "BBBBBBBBBBB", "category": "track",
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
    public void Poster_FromYouTube_NeedsTheDownloadedThumbnail()
    {
        Assert.Null(new MediaCatalog(_root).Poster(new MediaCatalog(_root).Videos[1], [270]));

        var cache = Path.Combine(_root, "cache");
        Directory.CreateDirectory(cache);
        File.Copy(Path.Combine(_root, "garage", "frame.jpg"), Path.Combine(cache, "BBBBBBBBBBB.jpg"));
        var catalog = new MediaCatalog(_root, cache);

        Assert.NotNull(catalog.Poster(catalog.Videos[1], [270]));
    }

    [Fact]
    public void Image_UnknownId_Throws() =>
        Assert.Throws<KeyNotFoundException>(() => new MediaCatalog(_root).Image("nope", ImageShape.Square, [160]));
}
