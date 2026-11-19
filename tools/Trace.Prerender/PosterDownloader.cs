using Trace.Domain.Media;

namespace Trace.Prerender;

/// <summary>
/// Downloads each video's YouTube thumbnail once, at build time, so posters are served from our own domain
/// (ADR-0010: no request to YouTube before the visitor clicks). Files are cached under artifacts/.
/// </summary>
internal static class PosterDownloader
{
    // hq720 is 1280×720; for Shorts the frame sits in a centred 9:16 column, which MediaPlan cuts out.
    private static readonly string[] Thumbnails = ["hq720.jpg", "hqdefault.jpg"];

    public static async Task<List<string>> DownloadAsync(HttpClient http, IEnumerable<VideoItem> videos, string cacheDirectory, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(cacheDirectory);
        var errors = new List<string>();

        foreach (var video in videos)
        {
            var target = Path.Combine(cacheDirectory, video.YoutubeId + ".jpg");
            if (File.Exists(target) && new FileInfo(target).Length > 0)
            {
                continue;
            }

            var saved = false;
            foreach (var name in Thumbnails)
            {
                using var response = await http.GetAsync(new Uri($"https://i.ytimg.com/vi/{video.YoutubeId}/{name}"), cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    await File.WriteAllBytesAsync(target, await response.Content.ReadAsByteArrayAsync(cancellationToken), cancellationToken);
                    saved = true;
                    break;
                }
            }

            if (!saved)
            {
                errors.Add($"video '{video.Id}' ({video.YoutubeId}): no thumbnail on YouTube; was it removed? Update content/media/videos.json");
            }
        }

        return errors;
    }
}
