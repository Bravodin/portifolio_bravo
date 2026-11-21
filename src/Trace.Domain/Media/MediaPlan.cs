using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Trace.Domain.Media;

/// <summary>Output formats, best first: browsers take the first <c>&lt;source&gt;</c> they support (ADR-0008).</summary>
public enum ImageFormat
{
    Avif,
    Webp,
    Jpeg,
}

/// <summary>How a variant is framed.</summary>
public enum ImageShape
{
    /// <summary>The whole image, resized.</summary>
    Original,

    /// <summary>A square cut around the focal point (avatars, the Recruiter Mode portrait, gallery thumbnails).</summary>
    Square,

    /// <summary>A 9:16 cut around the focal point (posters for vertical videos).</summary>
    Vertical,
}

/// <summary>A source rectangle in original pixels.</summary>
public readonly record struct CropBox(int Left, int Top, int Width, int Height);

/// <summary>One generated file: where it's served, what it's cut from and how big it is.</summary>
public sealed record ImageVariant(string Url, string SourcePath, CropBox Crop, int Width, int Height, ImageFormat Format);

/// <summary>
/// Pure recipe for responsive images: crop, sizes and file names. Trace.Web uses it to write
/// <c>&lt;picture&gt;</c> markup; Trace.Prerender uses the same recipe to generate exactly those files.
/// </summary>
public static class MediaPlan
{
    /// <summary>Bump when the encoder settings change, so every file name (and cache entry) changes too.</summary>
    public const int RecipeVersion = 1;

    public static readonly IReadOnlyList<ImageFormat> Formats = [ImageFormat.Avif, ImageFormat.Webp, ImageFormat.Jpeg];

    /// <summary>
    /// The crop for a shape: the largest box with the shape's aspect ratio, centred on the focal point
    /// (0–1 from the top-left corner) and pushed back inside the image when it would overflow.
    /// </summary>
    public static CropBox Crop(PixelSize source, ImageShape shape, double focalX = 0.5, double focalY = 0.5)
    {
        var ratio = shape switch
        {
            ImageShape.Square => 1.0,
            ImageShape.Vertical => 9.0 / 16.0,
            _ => source.Width / (double)source.Height,
        };

        var (width, height) = source.Width / (double)source.Height > ratio
            ? ((int)Math.Round(source.Height * ratio), source.Height)
            : (source.Width, (int)Math.Round(source.Width / ratio));

        var left = Math.Clamp((int)Math.Round((focalX * source.Width) - (width / 2.0)), 0, source.Width - width);
        var top = Math.Clamp((int)Math.Round((focalY * source.Height) - (height / 2.0)), 0, source.Height - height);
        return new CropBox(left, top, width, height);
    }

    /// <summary>Requested widths that don't upscale, plus the crop's own width when every request is too big.</summary>
    public static IReadOnlyList<int> Widths(CropBox crop, IEnumerable<int> requested)
    {
        var fitting = requested.Where(w => w <= crop.Width).Distinct().Order().ToList();
        return fitting.Count > 0 ? fitting : [crop.Width];
    }

    /// <summary>A short content hash of the original plus the recipe, so a new photo or recipe gets a new URL.</summary>
    public static string Fingerprint(byte[] original, ImageShape shape, CropBox crop)
    {
        var recipe = Encoding.UTF8.GetBytes(FormattableString.Invariant($"{RecipeVersion}|{shape}|{crop}"));
        var hash = SHA256.HashData([.. original, .. recipe]);
        return Convert.ToHexStringLower(hash)[..10];
    }

    public static string Url(string id, ImageShape shape, int width, string fingerprint, ImageFormat format)
    {
        var suffix = shape switch
        {
            ImageShape.Square => "-sq",
            ImageShape.Vertical => "-v",
            _ => string.Empty,
        };
        var extension = format switch
        {
            ImageFormat.Avif => "avif",
            ImageFormat.Webp => "webp",
            _ => "jpg",
        };
        return string.Create(CultureInfo.InvariantCulture, $"/media/{id}{suffix}-{width}.{fingerprint}.{extension}");
    }

    public static int HeightFor(CropBox crop, int width) =>
        (int)Math.Round(crop.Height * (width / (double)crop.Width));
}
