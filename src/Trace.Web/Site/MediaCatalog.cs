using System.Collections.Concurrent;
using Trace.Domain.Media;

namespace Trace.Web.Site;

/// <summary>A responsive image ready for <c>&lt;picture&gt;</c>: every variant, plus the item for its alt text.</summary>
public sealed record ResponsiveImage(GalleryItem Item, IReadOnlyList<ImageVariant> Variants)
{
    public IEnumerable<ImageVariant> In(ImageFormat format) => Variants.Where(v => v.Format == format);

    public static string Srcset(IEnumerable<ImageVariant> variants) =>
        string.Join(", ", variants.Select(v => $"{v.Url} {v.Width}w"));
}

/// <summary>
/// Photos from <c>content/media/</c> (ADR-0008). Pages ask it for responsive images; every variant it hands out
/// is remembered, so Trace.Prerender can generate exactly the files the HTML references.
/// </summary>
public sealed class MediaCatalog
{
    private readonly string _root;
    private readonly Dictionary<string, GalleryItem> _items;
    private readonly ConcurrentDictionary<string, ImageVariant> _issued = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, (PixelSize Size, byte[] Bytes)> _originals = new(StringComparer.Ordinal);

    public MediaCatalog(string mediaRoot)
    {
        _root = mediaRoot;
        _items = Gallery.Load(Path.Combine(mediaRoot, "gallery.json")).ToDictionary(i => i.Id, StringComparer.Ordinal);
    }

    public GalleryItem Item(string id) =>
        _items.TryGetValue(id, out var item) ? item : throw new KeyNotFoundException($"gallery.json has no item '{id}'.");

    public ResponsiveImage Image(string id, ImageShape shape, IEnumerable<int> widths)
    {
        var item = Item(id);
        var path = Path.Combine(_root, item.File.Replace('/', Path.DirectorySeparatorChar));
        var (size, bytes) = _originals.GetOrAdd(path, p => (ImageHeader.Read(p), File.ReadAllBytes(p)));

        var crop = MediaPlan.Crop(size, shape, item.FocalPoint?.X ?? 0.5, item.FocalPoint?.Y ?? 0.5);
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

        return new ResponsiveImage(item, variants);
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
