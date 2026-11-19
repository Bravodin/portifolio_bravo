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
- [ ] Fill in the photographer credit for `track-01` and `track-03`.
- [ ] Confirm the captions (they're drafts) and the dates.
- [ ] Add photos for **workbench** (mechanics) and **games** to complete the Garage.
