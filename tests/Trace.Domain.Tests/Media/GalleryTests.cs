using Trace.Domain.Media;
using Trace.Domain.Tests.Content;

namespace Trace.Domain.Tests.Media;

public sealed class GalleryTests
{
    private static readonly string[] VideoExtensions = [".mp4", ".webm", ".mov", ".m4v"];

    [Fact]
    public void Gallery_LoadsEveryPhoto() =>
        Assert.Equal(12, Gallery.Load(Path.Combine(RepoContent.Root, "media", "gallery.json")).Count);

    /// <summary>ADR-0014: no video on the site, to keep it light and its bandwidth predictable.</summary>
    [Fact]
    public void Media_HasNoVideoFiles()
    {
        var files = Directory.EnumerateFiles(Path.Combine(RepoContent.Root, "media"), "*", SearchOption.AllDirectories);

        Assert.DoesNotContain(files, f => VideoExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));
    }
}
