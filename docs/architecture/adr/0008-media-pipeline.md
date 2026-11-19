# 0008 — Media pipeline: originals in the repo, responsive variants at build time

- Status: Accepted
- Date: 2026-11-19

## Context

The site now has real photos (a portrait and track days), and Manuel wants to keep adding more over time without touching code. The photos are the heaviest assets on the page. Badly handled images would break the LCP and CLS budgets, and they could leak metadata (GPS, device) that the site shouldn't publish.

## Decision

- **Originals live in `content/media/{profile,garage,workbench,games}/`.** They're never served as-is.
- **One manifest, `content/media/gallery.json`**, validated by `gallery.schema.json`, holds the id, category, featured flag, focal point, credit, and `alt`/caption in PT-BR, EN and ES.
- **`Trace.Prerender` runs a media step** with **NetVips** (MIT, libvips bindings) that:
  - generates **AVIF + WebP + JPEG** at widths **480 / 960 / 1600** (gallery) and **160 / 320 / 640 / 800** (profile);
  - crops the square and 4:3 thumbnails **around `focalPoint`**;
  - **strips all metadata** (EXIF, GPS, ICC is converted to sRGB);
  - computes a **tiny blurred placeholder** (≈ 300 bytes, inlined as a data URI) plus the dominant color, so there's no layout shift and no grey boxes while images load;
  - writes fingerprinted file names (`track-03-lean.960.3f9a2c.avif`).
- **Markup:** `<picture>` with `srcset`/`sizes`, explicit `width`/`height`, `loading="lazy"` and `decoding="async"` for everything below the fold, and `fetchpriority="high"` only for the portrait in Recruiter Mode.
- **Open Graph images** (1200 × 630, one per locale) are composed at build time from the portrait, name and tagline.
- **CI fails when** a file has no manifest entry (or the reverse), an `alt` text is missing in any locale, an original is > 5 MB, or an original is < 800 px on its long side (warning only for the portrait).

## Alternatives considered

| Option | Why not |
|---|---|
| Serve originals and let CSS resize them | ~270 KB per photo on mobile; no AVIF; metadata leak |
| SixLabors.ImageSharp | Great API, but its split license needs a closer look for commercial use; NetVips is MIT and faster |
| SkiaSharp | MIT, but no AVIF encoder |
| External image CDN (Cloudinary, imgix) | One more account and a runtime dependency, which goes against the zero-servers goal |
| Gallery in a third-party widget (Instagram embed) | Tracking scripts, slow, inaccessible |

## Consequences

- ✅ Adding a photo = drop the file + 1 JSON entry + push
- ✅ Page weight stays predictable: a gallery thumbnail ≈ 15–30 KB in AVIF
- ✅ Privacy by default (metadata stripped)
- ⚠️ libvips native binaries in CI (the `NetVips.Native` package covers linux-x64)
- ⚠️ The repo grows with originals. If it gets past ~200 MB, move `content/media` to Git LFS
