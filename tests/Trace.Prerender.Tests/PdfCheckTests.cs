namespace Trace.Prerender.Tests;

public sealed class PdfCheckTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"trace-{Guid.NewGuid():N}.pdf");

    public void Dispose() => File.Delete(_path);

    [Fact]
    public void Missing_IsReported() => Assert.Equal("is missing", PdfCheck.Problem(_path));

    [Fact]
    public void Empty_IsAPlaceholder()
    {
        File.WriteAllBytes(_path, []);

        Assert.Equal("is empty (placeholder)", PdfCheck.Problem(_path));
    }

    [Theory]
    [InlineData("hello world")]
    [InlineData("%PD")]
    public void NotAPdf_IsReported(string content)
    {
        File.WriteAllText(_path, content);

        Assert.Equal("is not a PDF", PdfCheck.Problem(_path));
    }

    [Fact]
    public void RealPdfSignature_Passes()
    {
        File.WriteAllText(_path, "%PDF-1.7 ...");

        Assert.Null(PdfCheck.Problem(_path));
    }
}
