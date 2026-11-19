using Trace.Domain.Media;

namespace Trace.Prerender.Tests;

public sealed class MediaGeneratorTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "trace-media-" + Guid.NewGuid().ToString("N"));

    private static string Profile { get; } = FindProfile();

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Theory]
    [InlineData(ImageFormat.Avif, "heifload")]
    [InlineData(ImageFormat.Webp, "webpload")]
    [InlineData(ImageFormat.Jpeg, "jpegload")]
    public void Write_ProducesTheRequestedFormatAndSize(ImageFormat format, string loader)
    {
        var target = Path.Combine(_dir, $"out.{format}");
        var variant = new ImageVariant("/media/x", Profile, new CropBox(0, 0, 361, 361), 160, 160, format);

        MediaGenerator.Write(variant, target);

        using var image = NetVips.Image.NewFromBuffer(File.ReadAllBytes(target));
        Assert.Equal((160, 160), (image.Width, image.Height));
        Assert.StartsWith(loader, (string)image.Get("vips-loader"), StringComparison.Ordinal);
    }

    [Fact]
    public void Write_StripsAllMetadata()
    {
        var target = Path.Combine(_dir, "out.jpg");
        MediaGenerator.Write(new ImageVariant("/media/x", Profile, new CropBox(0, 0, 361, 361), 160, 160, ImageFormat.Jpeg), target);

        using var image = NetVips.Image.NewFromBuffer(File.ReadAllBytes(target));
        Assert.DoesNotContain(image.GetFields(), f => f.StartsWith("exif", StringComparison.Ordinal) || f is "xmp-data" or "iptc-data");
    }

    [Fact]
    public void Write_IsSmallForAnAvatar()
    {
        var target = Path.Combine(_dir, "out.avif");
        MediaGenerator.Write(new ImageVariant("/media/x", Profile, new CropBox(0, 0, 361, 361), 160, 160, ImageFormat.Avif), target);

        Assert.InRange(new FileInfo(target).Length, 1, 15_000);
    }

    private static string FindProfile()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var path = Path.Combine(dir.FullName, "content", "media", "profile", "manuel-bravo.jpg");
            if (File.Exists(path))
            {
                return path;
            }
        }

        throw new FileNotFoundException("profile photo not found above the test binaries");
    }
}
