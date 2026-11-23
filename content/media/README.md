# Media: how to add photos

The site's photos live here as **originals**. The build generates every web version automatically (AVIF + WebP + JPEG at several widths, metadata stripped). See [ADR-0008](../../docs/architecture/adr/0008-media-pipeline.md).

## Add a new photo in 3 steps

1. **Drop the file** into the right folder:

   | Folder | What goes there | Where it appears |
   |---|---|---|
   | `profile/` | Portraits | Recruiter Mode, Pit Stop, Open Graph image |
   | `garage/` | Motorcycles, track days, mechanics | Garage section + gallery |
   | `workbench/` | The bike in the workshop, tools, repairs | Garage section + gallery |
   | `games/` | Setup, controllers, favorite games | Garage section + gallery |
   | `colophon/` | Screenshots of how the site is built (category `colophon`) | Colophon only, placed by id in `Colophon.razor` |
   | `projects/` | One thumbnail per case study, taken from the repo's screenshots (category `projects`) | Case studies on the home page, by the `photo` id in `content/{locale}/cases.json` |

   Name it in lowercase with hyphens: `track-04-interlagos.jpg`. Use JPEG/PNG/WebP, ideally ≥ 1600 px on the long side (≥ 800 px for portraits).

2. **Add an entry** to [`gallery.json`](gallery.json). Copy an existing item and change it:

   ```json
   {
     "id": "track-04",
     "file": "garage/track-04-interlagos.jpg",
     "category": "track",
     "featured": false,
     "focalPoint": { "x": 0.5, "y": 0.5 },
     "date": "2026-12",
     "credit": { "name": "Photographer name", "url": null },
     "alt":     { "pt-BR": "…", "en": "…", "es": "…" },
     "caption": { "pt-BR": "…", "en": "…", "es": "…" }
   }
   ```

   - `alt` is **required** in all 3 languages. It describes the photo for screen readers.
   - `featured: true` puts the photo on the Garage cards (up to 3). The rest appear only in the gallery.
   - `focalPoint` tells the crop where the subject is (0–1, from the top-left corner). That way square thumbnails don't cut off the bike.
   - `credit` shows the photographer's name in the lightbox.

3. **Commit and push.** CI validates the manifest, generates the images and publishes them.

## Rules

- **Event photos:** credit the photographer. If possible, ask the photographer for permission and a version without the watermark. Keep their watermark if you're using the watermarked version.
- **Other people:** avoid identifiable faces of people who didn't agree to appear.
- **Don't** commit files that are larger than needed (> 5 MB): CI rejects them.
- The CI **fails** if an image exists without an entry in the manifest (or the reverse), or if an `alt` text is missing.

## To do for the current photos

- [ ] `profile/manuel-bravo.jpg` is 361 × 423 px. That's fine for the avatar, but send a version ≥ 800 px for the Open Graph image and high-density screens.
- [x] Fill in the photographer credit for `track-01` and `track-03` (@mustelasportmedia).
- [ ] Confirm the captions (they're drafts) and the dates.
- [x] Caption for `track-09` (consistency before speed). The gallery keeps four track photos (01, 02, 03, 09); the others were removed and can be restored from git history.
- [x] **Workbench**: `workbench/manu-mechanic.png`, an AI-generated illustration (marked with `aiGenerated: true`, so the site labels it instead of showing a credit).
- [x] **Games**: `games/games-01-controller.jpg`. The laptop screen showed Super Mario World (Nintendo's art), so the screen area was blurred before committing; never commit game art, logos or screenshots.


## Videos

Videos are served by the site itself, as small MP4 files ([ADR-0012](../../docs/architecture/adr/0012-self-hosted-video.md)). The site shows a poster with a ▶ button and only loads the video when someone clicks. Only the web-ready encode goes into git, never the camera original.

1. Encode the original (vertical clip; drop `-c:a aac -b:a 96k` and add `-an` if it has no sound):

   ```powershell
   ffmpeg -i "MVI_1234.MP4" -map 0:v:0 -map 0:a:0? -vf "scale=720:1280:flags=lanczos,fps=30000/1001,format=yuv420p" `
     -c:v libx264 -preset slow -crf 25 -profile:v high -level 4.0 -c:a aac -b:a 96k -ac 2 `
     -map_metadata -1 -map_chapters -1 -movflags +faststart garage/videos/short-05-interlagos.mp4
   ```

   Aim for a few MB per clip. Install ffmpeg with `winget install Gyan.FFmpeg`.
2. Export a poster frame (`-ss` = the second you want):

   ```powershell
   ffmpeg -ss 8 -i "MVI_1234.MP4" -frames:v 1 -vf "scale=1080:1920" -q:v 2 -map_metadata -1 garage/short-05-interlagos-poster.jpg
   ```

3. Add an entry to [`videos.json`](videos.json), validated by `videos.schema.json`:

```json
{
  "id": "short-05-interlagos",
  "file": "garage/videos/short-05-interlagos.mp4",
  "category": "track",
  "featured": false,
  "aspectRatio": "9:16",
  "poster": "garage/short-05-interlagos-poster.jpg",
  "credit": { "name": "@mustelasportmedia", "url": "https://www.instagram.com/mustelasportmedia/" },
  "title":       { "pt-BR": "…", "en": "…", "es": "…" },
  "description": { "pt-BR": "…", "en": "…", "es": "…" }
}
```

- `description` is **required** in all 3 languages: it tells people who can't watch what happens. Say what can be heard ("engine sound only", "no sound"). If someone speaks, the video needs captions.
- Video files and poster frames are **not** listed in `gallery.json`. The build fails if a `file` or `poster` is missing.

### To do for the current videos

- [x] `short-01-roll-out`: other riders appear small in the paddock background; Manuel confirmed that's fine (2026-11-21).
- [x] Originals matched by eye (01 = MVI_7492, 02 = MVI_7781, 03 = MVI_7769, 04 = MVI_9404); Manuel said the order doesn't matter.
- [x] All four clips are silent (Manuel's call; short-01's audio track was removed), and their descriptions say so.
