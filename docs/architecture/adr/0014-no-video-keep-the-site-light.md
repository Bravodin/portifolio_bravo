# 0014 — No video: keep the site light

- Status: Accepted
- Date: 2026-11-23
- Supersedes: [0012](0012-self-hosted-video.md)

## Context

[ADR-0012](0012-self-hosted-video.md) served four track-day clips from the site itself and accepted this trade-off:

> About 12 MB of video in git and in the SWA deployment (Free plan: 250 MB). Each play costs bandwidth (100 GB/month); a few thousand plays a month is well within it.

The goal of the site is to be **light**: static HTML that works without JavaScript, images in AVIF/WebP at the right size, and the performance budgets in [overview §6](../overview.md). The four clips weighed about 12 MB, more than the rest of the deployment's media put together, and every play is served from our own bandwidth quota.

"Well within it" only holds while traffic behaves. One shared link, a crawler that follows the MP4 links (they are plain `<a href>` in L0, so any bot can fetch them), or someone replaying the files in a loop turns video into most of the site's traffic. On the Free plan that means hitting the 100 GB/month limit. Moving to a paid plan, or putting the files on another host, means a bill that wasn't planned. The clips add colour to the Garage, but they don't help the portfolio do its job, and the risk isn't worth it.

## Decision

- **The site has no video.** The four MP4s, their poster frames, `videos.json` and `videos.schema.json` are removed from `content/media/`.
- **The code that existed only for video goes too:** `VideoFacade`, `Videos`/`VideoItem`, `MediaCatalog.VideoUrl`/`Poster`, the prerender's copy of linked files, the `<video>` in the `site.js` lightbox, the `.mp4` MIME type in `staticwebapp.config.json`, the video CSS and UI strings, and their tests.
- **The Garage stays a photo page**: track days, workshop and games, all through the ADR-0008 image pipeline. The lightbox now only shows photos.
- **Bringing video back takes a new ADR.** It has to answer the bandwidth question first (for example, a host whose cost doesn't depend on how often the files are played). The YouTube option in [ADR-0010](0010-video-youtube-facade.md) was turned down for its branding over the video, and that reason hasn't changed.

## Alternatives considered

| Option | Why not |
|---|---|
| Keep self-hosted MP4s (ADR-0012) | ~12 MB per full set of plays from our own quota; a spike or a bot can use up the 100 GB/month or create cost |
| Smaller encodes (lower CRF, shorter clips) | Less risk, but the same kind of risk; video is still the heaviest thing on the site |
| Back to YouTube (ADR-0010) | No bandwidth cost, but YouTube's avatar and title over the clip, plus third-party requests and cookies once it plays |
| Video on a CDN or object storage | Another service to run and pay for; the bill still grows with plays |
| Animated posters (GIF/WebP) instead of video | Heavier than video for the same motion, and it plays without being asked |

## Consequences

- ✅ The deployment loses ~12 MB, and no single visit can cost more than a page of photos
- ✅ Bandwidth only grows with page views, and pages stay inside the performance budgets
- ✅ Less code: no video facade, no linked-file copy step, a photo-only lightbox
- ⚠️ The Garage loses motion; the track-day photos have to carry it
- ⚠️ The MP4s stay in git history (removing them from history would mean rewriting `main`). Only the deployment and the working tree get lighter
