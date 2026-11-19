namespace Trace.Prerender;

/// <summary>Catches a placeholder CV before it ships: the file must exist and start with the PDF signature.</summary>
internal static class PdfCheck
{
    private static readonly byte[] Signature = "%PDF-"u8.ToArray();

    /// <summary>A description of what's wrong, or null when the file looks like a real PDF.</summary>
    public static string? Problem(string path)
    {
        if (!File.Exists(path))
        {
            return "is missing";
        }

        var head = new byte[Signature.Length];
        using var stream = File.OpenRead(path);
        var read = stream.Read(head, 0, head.Length);
        return read == 0 ? "is empty (placeholder)"
            : read < head.Length || !head.AsSpan().SequenceEqual(Signature) ? "is not a PDF"
            : null;
    }
}
