using Trace.Domain.Media;
using Trace.Domain.Tests.Content;

namespace Trace.Domain.Tests.Media;

public sealed class VideosTests
{
    private static readonly IReadOnlyList<VideoItem> Items = Videos.Load(Path.Combine(RepoContent.Root, "media", "videos.json"));

    [Fact]
    public void Load_ReadsTheFourShorts_WithCreditAndAllLanguages()
    {
        Assert.Equal(4, Items.Count);
        Assert.All(Items, v =>
        {
            Assert.Equal("@mustelasportmedia", v.Credit.Name);
            Assert.Equal(["pt-BR", "en", "es"], v.Description.Keys);
        });
    }

    [Fact]
    public void EveryVideo_HasItsFileAndPosterInTheRepo() =>
        Assert.All(Items, v =>
        {
            Assert.True(File.Exists(Path.Combine(RepoContent.Root, "media", v.File)), v.File);
            Assert.True(File.Exists(Path.Combine(RepoContent.Root, "media", v.Poster)), v.Poster);
        });

    [Fact]
    public void Gallery_LoadsEveryPhoto() =>
        Assert.Equal(7, Gallery.Load(Path.Combine(RepoContent.Root, "media", "gallery.json")).Count);
}
