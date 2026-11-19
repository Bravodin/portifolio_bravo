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
    public void WatchUrl_PointsAtTheShort() =>
        Assert.Equal(new Uri("https://www.youtube.com/shorts/xEh1hYPC3lM"), Items[0].WatchUrl);

    [Fact]
    public void Gallery_LoadsEveryPhoto() =>
        Assert.Equal(11, Gallery.Load(Path.Combine(RepoContent.Root, "media", "gallery.json")).Count);
}
