using Trace.Domain.Media;
using Trace.Web.Site;

namespace Trace.Components.Tests;

public sealed class MediaCatalogTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "trace-media-" + Guid.NewGuid().ToString("N"));

    public MediaCatalogTests()
    {
        Directory.CreateDirectory(_root);
        File.WriteAllText(Path.Combine(_root, "gallery.json"), """{ "items": [] }""");
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void Image_UnknownId_Throws() =>
        Assert.Throws<KeyNotFoundException>(() => new MediaCatalog(_root).Image("nope", ImageShape.Square, [160]));
}
