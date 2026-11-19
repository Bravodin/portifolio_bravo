using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Trace.Web.Site;

/// <summary>
/// Numbers about this repository, measured when the site is built, so the Colophon never goes stale.
/// <c>Commits</c> is null when git isn't available. CI must check out the full history (fetchDepth: 0),
/// or a shallow clone would report 1.
/// </summary>
public sealed partial record RepoStats(int? Commits, int Adrs, int TestMethods)
{
    public static RepoStats Measure()
    {
        var root = FindRoot();
        if (root is null)
        {
            return new RepoStats(null, 0, 0);
        }

        var adrs = Directory.EnumerateFiles(Path.Combine(root, "docs", "architecture", "adr"), "0*.md").Count();
        var tests = Directory.EnumerateFiles(Path.Combine(root, "tests"), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Sum(f => TestAttribute().Count(File.ReadAllText(f)));
        return new RepoStats(CountCommits(root), adrs, tests);
    }

    private static int? CountCommits(string root)
    {
        try
        {
            using var git = Process.Start(new ProcessStartInfo("git", "rev-list --count HEAD")
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            });
            if (git is null || !git.WaitForExit(10_000))
            {
                return null;
            }

            return git.ExitCode == 0 && int.TryParse(git.StandardOutput.ReadToEnd().Trim(), out var count) ? count : null;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return null; // git isn't installed
        }
    }

    private static string? FindRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Trace.slnx")))
            {
                return dir.FullName;
            }
        }

        return null;
    }

    [GeneratedRegex(@"^\s*\[(Fact|Theory)\b", RegexOptions.Multiline)]
    private static partial Regex TestAttribute();
}
