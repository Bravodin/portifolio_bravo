using Trace.Domain.Media;
using Trace.Domain.Tests.Content;

namespace Trace.Domain.Tests.Media;

public sealed class ImageHeaderTests
{
    [Fact]
    public void Read_RealJpeg_ReturnsItsSize() =>
        Assert.Equal(new PixelSize(361, 423), ImageHeader.Read(Path.Combine(RepoContent.Root, "media", "profile", "manuel-bravo.jpg")));

    [Fact]
    public void Read_LargeJpegWithExif_SkipsSegmentsToTheFrame() =>
        Assert.Equal(new PixelSize(6000, 4000), ImageHeader.Read(Path.Combine(RepoContent.Root, "media", "garage", "track-04-panning.jpg")));

    [Fact]
    public void Read_PngHeader_ReturnsItsSize()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 13, 0x49, 0x48, 0x44, 0x52, 0, 0, 0x03, 0x20, 0, 0, 0x02, 0x58];

        Assert.Equal(new PixelSize(800, 600), ImageHeader.Read(new MemoryStream(png)));
    }

    [Theory]
    [InlineData(new byte[] { 1, 2, 3 })]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
    [InlineData(new byte[] { 0xFF, 0xD8, 0x00, 0x00, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })]
    public void Read_TooShortUnknownOrBroken_ReturnsNull(byte[] bytes) =>
        Assert.Null(ImageHeader.Read(new MemoryStream(bytes)));

    [Fact]
    public void Read_PathToNonImage_Throws()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "not an image, just text that is long enough");
            Assert.Throws<InvalidDataException>(() => ImageHeader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
