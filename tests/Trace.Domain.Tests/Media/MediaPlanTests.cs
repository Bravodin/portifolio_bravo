using Trace.Domain.Media;

namespace Trace.Domain.Tests.Media;

public sealed class MediaPlanTests
{
    private static readonly PixelSize Portrait = new(361, 423);

    [Fact]
    public void Crop_Original_IsTheWholeImage() =>
        Assert.Equal(new CropBox(0, 0, 361, 423), MediaPlan.Crop(Portrait, ImageShape.Original));

    [Fact]
    public void Crop_SquareNearTheTop_IsClampedInsideTheImage() =>
        Assert.Equal(new CropBox(0, 0, 361, 361), MediaPlan.Crop(Portrait, ImageShape.Square, 0.5, 0.38));

    [Fact]
    public void Crop_SquareOnLandscape_FollowsTheFocalPoint()
    {
        var crop = MediaPlan.Crop(new PixelSize(6000, 4000), ImageShape.Square, 0.49, 0.47);

        Assert.Equal(new CropBox(940, 0, 4000, 4000), crop);
    }

    [Fact]
    public void Crop_SquareAtTheRightEdge_IsClamped() =>
        Assert.Equal(2000, MediaPlan.Crop(new PixelSize(6000, 4000), ImageShape.Square, 1, 0.5).Left);

    [Fact]
    public void Widths_NeverUpscale() =>
        Assert.Equal([160, 320], MediaPlan.Widths(new CropBox(0, 0, 361, 361), [160, 320, 640, 800, 320]));

    [Fact]
    public void Widths_AllTooBig_FallBackToTheCropWidth() =>
        Assert.Equal([300], MediaPlan.Widths(new CropBox(0, 0, 300, 300), [480, 960]));

    [Fact]
    public void Fingerprint_ChangesWithThePhotoAndTheCrop()
    {
        byte[] photo = [1, 2, 3];
        var crop = new CropBox(0, 0, 10, 10);
        var fingerprint = MediaPlan.Fingerprint(photo, ImageShape.Square, crop);

        Assert.Equal(10, fingerprint.Length);
        Assert.Equal(fingerprint, MediaPlan.Fingerprint(photo, ImageShape.Square, crop));
        Assert.NotEqual(fingerprint, MediaPlan.Fingerprint([1, 2, 4], ImageShape.Square, crop));
        Assert.NotEqual(fingerprint, MediaPlan.Fingerprint(photo, ImageShape.Square, crop with { Left = 1 }));
    }

    [Theory]
    [InlineData(ImageShape.Square, ImageFormat.Avif, "/media/profile-sq-320.abc.avif")]
    [InlineData(ImageShape.Original, ImageFormat.Webp, "/media/profile-320.abc.webp")]
    [InlineData(ImageShape.Original, ImageFormat.Jpeg, "/media/profile-320.abc.jpg")]
    public void Url_EncodesShapeWidthHashAndFormat(ImageShape shape, ImageFormat format, string expected) =>
        Assert.Equal(expected, MediaPlan.Url("profile", shape, 320, "abc", format));

    [Fact]
    public void HeightFor_KeepsTheAspectRatio() =>
        Assert.Equal(2667, MediaPlan.HeightFor(new CropBox(0, 0, 6000, 4000), 4000));
}
