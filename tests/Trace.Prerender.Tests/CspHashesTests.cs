namespace Trace.Prerender.Tests;

public sealed class CspHashesTests
{
    [Fact]
    public void ForInlineScript_HashesLikeTheBrowser_AfterNewlineNormalization()
    {
        Assert.Equal(CspHashes.ForInlineScript("a\nb\n"), CspHashes.ForInlineScript("a\r\nb\r\n"));
        Assert.Equal(CspHashes.ForInlineScript("a\nb"), CspHashes.ForInlineScript("a\rb"));
    }

    [Fact]
    public void ForInlineScript_KnownValue()
    {
        // printf 'alert(1)' | openssl dgst -sha256 -binary | base64
        Assert.Equal("'sha256-bhHHL3z2vDgxUt0W3dWQOrprscmda2Y5pLsLg4GF+pI='", CspHashes.ForInlineScript("alert(1)"));
    }
}
