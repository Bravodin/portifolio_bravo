# 0010 — Video: YouTube Shorts behind a click-to-load facade

- Status: Accepted
- Date: 2026-11-19

## Context

Manuel has four track-day clips (vertical, engine sound only, no speech) filmed by @mustelasportmedia, uploaded as YouTube Shorts. The Garage should be able to play them on the page.

Hosting video in the repo would bloat git, eat the SWA Free limits (250 MB per environment, 100 GB/month bandwidth, [ADR-0009](0009-azure-pipelines-and-static-web-apps.md)) and lose adaptive streaming. A standard YouTube `<iframe>`, on the other hand, loads 500 KB+ of JavaScript and sets tracking cookies on page load, which breaks the performance budget (overview §6) and the cookie-free goal ([ADR-0006](0006-analytics-privacy.md)).

## Decision

- **Videos live on YouTube** and are listed in `content/media/videos.json` (validated by `videos.schema.json`): YouTube ID, credit, and a title plus a text description in PT-BR, EN and ES.
- **Facade, not an embed.** Each video renders as a 9:16 card with a poster image and a ▶ button. Nothing from YouTube loads until the visitor clicks.
  - **L0 (no JS):** the card is a plain link to `https://www.youtube.com/shorts/{id}`, so it works everywhere.
  - **L1 (a small JS module, no WASM):** on click, the link is swapped for an `<iframe>` from **`https://www.youtube-nocookie.com/embed/{id}?autoplay=1&playsinline=1&rel=0`**, which plays in place.
- **Posters are self-hosted.** `Trace.Prerender`'s media step downloads each video's thumbnail at build time and runs it through the ADR-0008 pipeline (AVIF/WebP, metadata stripped). An optional `poster` in the manifest overrides it with a frame from `content/media/`. The page never requests `i.ytimg.com`.
- **CSP:** add `frame-src https://www.youtube-nocookie.com` and nothing else. `img-src` stays `'self'`.
- **Accessibility:** the button's accessible name is "Play video: {title}". The clips have no speech, so they need no captions. The text description is the alternative for what happens on screen (WCAG 1.2.1). Videos never autoplay before a click, which also respects reduced motion.

## Alternatives considered

| Option | Why not |
|---|---|
| Standard YouTube `<iframe>` | 500 KB+ JS and cookies on every page view |
| Self-hosted MP4/WebM | Repo and bandwidth limits, no adaptive streaming, encoding pipeline to maintain |
| Link out only | Takes the visitor off the site; no in-place playback |
| `lite-youtube-embed` web component | Same idea, but a third-party script; ~40 lines of our own keep the CSP to `'self'` |

## Consequences

- ✅ Zero YouTube bytes and zero cookies until the visitor asks for a video
- ✅ Adding a video = upload a Short + one JSON entry
- ⚠️ After the click, YouTube's privacy-enhanced mode still applies its own policy; that's the visitor's choice
- ⚠️ If a video is removed from YouTube, the build's thumbnail download fails, which is the signal to update the manifest
