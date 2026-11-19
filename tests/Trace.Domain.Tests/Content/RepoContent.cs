namespace Trace.Domain.Tests.Content;

/// <summary>Locates the repository's real <c>content/</c> folder from the test binaries.</summary>
internal static class RepoContent
{
    public static string Root { get; } = Find();

    private static string Find()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Trace.slnx")))
            {
                return Path.Combine(dir.FullName, "content");
            }
        }

        throw new DirectoryNotFoundException("Could not find the repository root (Trace.slnx) above the test binaries.");
    }

    /// <summary>Copies the real content into a temp folder so a test can break it on purpose.</summary>
    public static string CopyToTemp()
    {
        var target = Path.Combine(Path.GetTempPath(), "trace-content-" + Guid.NewGuid().ToString("N"));
        foreach (var file in Directory.EnumerateFiles(Root, "*.json", SearchOption.AllDirectories)
                     .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}media{Path.DirectorySeparatorChar}", StringComparison.Ordinal)))
        {
            var destination = Path.Combine(target, Path.GetRelativePath(Root, file));
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(file, destination);
        }

        return target;
    }
}
