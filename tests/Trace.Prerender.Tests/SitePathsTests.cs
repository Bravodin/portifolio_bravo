namespace Trace.Prerender.Tests;

public sealed class SitePathsTests
{
    [Theory]
    [InlineData("/", "/")]
    [InlineData("", "/")]
    [InlineData("/en/colophon", "/en/colophon/")]
    [InlineData("/en/colophon/", "/en/colophon/")]
    [InlineData("en/colophon?x=1#top", "/en/colophon/")]
    public void CanonicalPage_AddsSlashesAndDropsQuery(string path, string expected) =>
        Assert.Equal(expected, SitePaths.CanonicalPage(path));

    [Theory]
    [InlineData("/", "index.html")]
    [InlineData("/en/colophon/", "en/colophon/index.html")]
    public void PageFile_MapsToFolderIndex(string page, string expected) =>
        Assert.Equal(expected, SitePaths.PageFile(page));

    [Theory]
    [InlineData("/en/colophon/", true)]
    [InlineData("/", true)]
    [InlineData("/app.css", false)]
    [InlineData("/files/cv.pdf?v=2", false)]
    public void LooksLikePage_TellsPagesFromFiles(string path, bool expected) =>
        Assert.Equal(expected, SitePaths.LooksLikePage(path));
}
