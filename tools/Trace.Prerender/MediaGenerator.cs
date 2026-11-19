using NetVips;
using Trace.Domain.Media;

namespace Trace.Prerender;

/// <summary>
/// Writes one responsive image variant with libvips (ADR-0008): crop, resize, sRGB, and no metadata at all
/// (no EXIF, no GPS, no camera info) in AVIF, WebP or JPEG.
/// </summary>
internal static class MediaGenerator
{
    public static void Write(ImageVariant variant, string target)
    {
        using var original = NetVips.Image.NewFromFile(variant.SourcePath);

        // The crop is computed from the raw header size, so a rotated original would be cut in the wrong place.
        // Fail loudly instead: the fix is to save the original upright.
        if (original.Contains("orientation") && original.Get("orientation") is int orientation && orientation > 1)
        {
            throw new InvalidOperationException($"{variant.SourcePath} has EXIF orientation {orientation}; save it upright and try again.");
        }

        var crop = variant.Crop;
        using var srgb = original.Colourspace(Enums.Interpretation.Srgb);
        using var cropped = srgb.ExtractArea(crop.Left, crop.Top, crop.Width, crop.Height);
        using var resized = cropped.Resize(variant.Width / (double)crop.Width, vscale: variant.Height / (double)crop.Height, kernel: Enums.Kernel.Lanczos3);

        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        switch (variant.Format)
        {
            case ImageFormat.Avif:
                resized.Heifsave(target, q: 55, compression: Enums.ForeignHeifCompression.Av1, effort: 4, keep: Enums.ForeignKeep.None);
                break;
            case ImageFormat.Webp:
                resized.Webpsave(target, q: 78, effort: 5, keep: Enums.ForeignKeep.None);
                break;
            default:
                resized.Jpegsave(target, q: 82, optimizeCoding: true, interlace: true, keep: Enums.ForeignKeep.None);
                break;
        }
    }
}
