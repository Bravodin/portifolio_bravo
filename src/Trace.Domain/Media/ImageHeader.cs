using System.Buffers.Binary;

namespace Trace.Domain.Media;

/// <summary>Pixel size of an image, read from its header (no decoding, no native libraries).</summary>
public readonly record struct PixelSize(int Width, int Height);

/// <summary>Reads width and height from JPEG and PNG headers, so pages can set width/height without decoding images.</summary>
public static class ImageHeader
{
    public static PixelSize Read(string path)
    {
        using var stream = File.OpenRead(path);
        return Read(stream) ?? throw new InvalidDataException($"{path}: not a JPEG or PNG, or its header is damaged.");
    }

    public static PixelSize? Read(Stream stream)
    {
        Span<byte> head = stackalloc byte[24];
        if (stream.Read(head) < 24)
        {
            return null;
        }

        // PNG: 8-byte signature, then the IHDR chunk with big-endian width and height.
        if (head[..8].SequenceEqual((ReadOnlySpan<byte>)[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]))
        {
            return new PixelSize(BinaryPrimitives.ReadInt32BigEndian(head[16..]), BinaryPrimitives.ReadInt32BigEndian(head[20..]));
        }

        return head[0] == 0xFF && head[1] == 0xD8 ? ReadJpeg(stream) : null;
    }

    // Walks JPEG segments until a start-of-frame marker (SOF0–SOF15, except DHT/JPG/DAC), which holds the size.
    private static PixelSize? ReadJpeg(Stream stream)
    {
        stream.Position = 2;
        Span<byte> segment = stackalloc byte[9];
        while (stream.Read(segment[..4]) == 4)
        {
            if (segment[0] != 0xFF)
            {
                return null;
            }

            var marker = segment[1];
            var length = BinaryPrimitives.ReadUInt16BigEndian(segment[2..]);
            if (marker is >= 0xC0 and <= 0xCF and not 0xC4 and not 0xC8 and not 0xCC)
            {
                if (stream.Read(segment[..5]) < 5)
                {
                    return null;
                }

                return new PixelSize(BinaryPrimitives.ReadUInt16BigEndian(segment[3..]), BinaryPrimitives.ReadUInt16BigEndian(segment[1..]));
            }

            stream.Position += length - 2;
        }

        return null;
    }
}
