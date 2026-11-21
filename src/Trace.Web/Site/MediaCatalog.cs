using System.Collections.Concurrent;
using System.Security.Cryptography;
using Trace.Domain.Media;

namespace Trace.Web.Site;

/// <summary>A responsive image ready for <c>&lt;picture&gt;</c>: every variant, plus alt text per locale (null = decorative).</summary>
public sealed record ResponsiveImage(IReadOnlyDictionary<string, string>? Alt, IReadOnlyList<ImageVariant> Variants)
{
    public IEnumerable<ImageVariant> In(ImageFormat format) => Variants.Where(v => v.Format == format);

    public static string Srcset(IEnumerable<ImageVariant> variants) =>
        string.Join(", ", variants.Select(v => $"{v.Url} {v.Width}w"));
}

/// <summary>
/// Photos and videos from <c>content/media/</c> (ADR-0008, ADR-0012). Pages ask it for responsive images and video
/// URLs; everything it hands out is remembered, so Trace.Prerender can write exactly the files the HTML references.
/// </summary>
public sealed class MediaCatalog
{
    private readonly string _root;
    private readonly List<GalleryItem> _gallery;
    private readonly Dictionary<string, GalleryItem> _items;
    private readonly ConcurrentDictionary<string, ImageVariant> _issued = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> _issuedFiles = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, (PixelSize Size, byte[] Bytes)> _originals = new(StringComparer.Ordinal);

    /// <param name="mediaRoot">The folder with gallery.json, videos.json, the photo originals and the video files.</param>
    public MediaCatalog(string mediaRoot)
    {
        _root = mediaRoot;
        _gallery = [.. Gallery.Load(Path.Combine(mediaRoot, "gallery.json"))];
        _items = _gallery.ToDictionary(i => i.Id, StringComparer.Ordinal);
        Videos = Trace.Domain.Media.Videos.Load(Path.Combine(mediaRoot, "videos.json"));
    }

    /// <summary>Every photo, in manifest order.</summary>
    public IReadOnlyList<GalleryItem> Photos => _gallery;

    public IReadOnlyList<VideoItem> Videos { get; }

    public GalleryItem Item(string id) =>
        _items.TryGetValue(id, out var item) ? item : throw new KeyNotFoundException($"gallery.json has no item '{id}'.");

    public ResponsiveImage Image(string id, ImageShape shape, IEnumerable<int> widths)
    {
        var item = Item(id);
        var path = Path.Combine(_root, item.File.Replace('/', Path.DirectorySeparatorChar));
        return Build(id, path, shape, item.FocalPoint, widths, item.Alt);
    }

    /// <summary>A 9:16 poster for a video, cut from the frame its manifest entry points at (ADR-0012).</summary>
    public ResponsiveImage Poster(VideoItem video, IEnumerable<int> widths) =>
        Build(video.Id, Existing(video.Poster, video.Id), ImageShape.Vertical, video.FocalPoint, widths, alt: null);

    /// <summary>
    /// The video's URL, <c>/media/{id}.{hash}.mp4</c>: the hash of the file's bytes, so a new encode gets a new URL
    /// and the file can be cached as immutable, like the images (ADR-0012).
    /// </summary>
    public string VideoUrl(VideoItem video)
    {
        var path = Existing(video.File, video.Id);
        var (_, bytes) = _originals.GetOrAdd(path, p => (default, File.ReadAllBytes(p)));
        var url = $"/media/{video.Id}.{Convert.ToHexStringLower(SHA256.HashData(bytes))[..10]}{Path.GetExtension(path).ToLowerInvariant()}";
        _issuedFiles[url] = path;
        return url;
    }

    /// <summary>A file some page referenced as-is (a video), by URL. Used by Trace.Prerender's media step.</summary>
    public bool TryGetIssuedFile(string url, out string sourcePath) => _issuedFiles.TryGetValue(url, out sourcePath!);

    private string Existing(string relativePath, string videoId)
    {
        var path = Path.Combine(_root, (relativePath ?? string.Empty).Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(path)
            ? path
            : throw new FileNotFoundException($"videos.json: '{videoId}' points at '{relativePath}', which isn't in content/media/.", path);
    }

    private ResponsiveImage Build(string id, string path, ImageShape shape, FocalPoint? focal, IEnumerable<int> widths, IReadOnlyDictionary<string, string>? alt)
    {
        var (size, bytes) = _originals.GetOrAdd(path, p => (ImageHeader.Read(p), File.ReadAllBytes(p)));

        var crop = MediaPlan.Crop(size, shape, focal?.X ?? 0.5, focal?.Y ?? 0.5);
        var fingerprint = MediaPlan.Fingerprint(bytes, shape, crop);

        var variants = (
            from format in MediaPlan.Formats
            from width in MediaPlan.Widths(crop, widths)
            select new ImageVariant(
                MediaPlan.Url(id, shape, width, fingerprint, format),
                path,
                crop,
                width,
                MediaPlan.HeightFor(crop, width),
                format)).ToList();

        foreach (var variant in variants)
        {
            _issued[variant.Url] = variant;
        }

        return new ResponsiveImage(alt, variants);
    }

    /// <summary>A variant some page referenced, by URL. Used by Trace.Prerender's media step.</summary>
    public bool TryGetIssued(string url, out ImageVariant variant) => _issued.TryGetValue(url, out variant!);

    /// <summary>
    /// <c>Trace:MediaRoot</c> when set (the prerender passes it); otherwise the repo's <c>content/media</c>,
    /// found by walking up from the app (dev runs and tests).
    /// </summary>
    public static string FindRoot(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "content", "media");
            if (File.Exists(Path.Combine(candidate, "gallery.json")))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("content/media/gallery.json not found; set Trace:MediaRoot.");
    }
}
