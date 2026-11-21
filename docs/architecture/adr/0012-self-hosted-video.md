# 0012 — Video: self-hosted MP4 behind a click-to-load facade

- Status: Accepted
- Date: 2026-11-21
- Supersedes: [0010](0010-video-youtube-facade.md)

## Context

[ADR-0010](0010-video-youtube-facade.md) played the four track-day Shorts in a `youtube-nocookie` iframe after a click. Once playing, YouTube's player shows the channel avatar and the video title over the clip, and no embed parameter removes them any more (`showinfo` was dropped in 2018, `modestbranding` in 2023). Covering them with CSS would break YouTube's terms and break again whenever the player changes.

The clips are Manuel's own (filmed by @mustelasportmedia), and he has the original files. They are short (3–22 s), so web-ready encodes are small: about 12 MB for all four.

## Decision

- **The site serves the videos itself.** Web-ready MP4s live in `content/media/<category>/videos/` and are listed in `content/media/videos.json` (validated by `videos.schema.json`) with a required `file` and `poster`.
- **Encoding happens once, by hand, not in the build.** ffmpeg turns each original into H.264 High, 720×1280, 30 fps, CRF 25, AAC 96 kb/s when there's sound, `+faststart`, all metadata stripped. Only the encodes are committed; the originals stay out of git. The pipeline needs no ffmpeg. The command is in `content/media/README.md`.
- **Posters are frames from the same file**, exported as JPEG into `content/media/<category>/` and run through the ADR-0008 image pipeline (AVIF/WebP/JPEG, 9:16 crop). Nothing is downloaded at build time any more.
- **Facade, as before:**
  - **L0 (no JS):** the card is a plain link to the MP4, which the browser plays on its own.
  - **L1 (`site.js`, no WASM):** on click the link becomes a `<video controls playsinline>` with the same poster and starts playing. No video bytes are requested before the click.
- **URLs carry a content hash** (`/media/{id}.{hash}.mp4`), so the existing `/media/*` immutable cache rule covers them. `MediaCatalog` issues the URL, and `Trace.Prerender`'s media step copies exactly the files the pages link to.
- **CSP:** `frame-src https://www.youtube-nocookie.com` is removed. `default-src 'self'` covers `<video>`.
- **Accessibility:** unchanged from ADR-0010. The accessible name is "Play video: {title}", the text description is the alternative for what happens on screen (WCAG 1.2.1), and nothing autoplays before a click.

## Alternatives considered

| Option | Why not |
|---|---|
| Keep the YouTube iframe (ADR-0010) | Avatar and title over the video; no parameter removes them |
| Hide YouTube's overlay with CSS (scale/crop the iframe) | Against YouTube's terms; fragile |
| Link out to YouTube | Takes the visitor off the site |
| Encode in the pipeline (ffmpeg in CI) | Slower builds and one more tool to install, for files that rarely change |
| Adaptive streaming (HLS/DASH) | Overkill for clips under 25 s; MP4 with `faststart` starts playing quickly |

## Consequences

- ✅ No YouTube branding, no third-party request or cookie at any point
- ✅ The CSP no longer allows any third-party frame
- ✅ Posters come from the actual clip, chosen on purpose
- ⚠️ About 12 MB of video in git and in the SWA deployment (Free plan: 250 MB). Each play costs bandwidth (100 GB/month); a few thousand plays a month is well within it
- ⚠️ Adding a video = encode it with the README command + a poster frame + one JSON entry
