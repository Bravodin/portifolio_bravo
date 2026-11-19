# Architecture Decision Records

Format: [MADR](https://adr.github.io/madr/) (lightweight). One decision per file, numbered, never deleted. When a decision changes, write a new ADR that supersedes the old one.

| # | Decision | Status |
|---|---|---|
| [0001](0001-blazor-web-app-net10-wasm-islands.md) | Blazor Web App on .NET 10 with interactive WebAssembly islands | Accepted |
| [0002](0002-hosting-github-pages.md) | Host on GitHub Pages with a custom domain | Superseded by [0009](0009-azure-pipelines-and-static-web-apps.md) |
| [0003](0003-build-time-static-prerendering.md) | Build-time static prerendering with an in-process crawler | Accepted (spike in Phase 1) |
| [0004](0004-i18n-content-as-data.md) | Content as data; i18n by route; invariant globalization | Accepted (routes superseded by [0011](0011-english-at-the-root.md)) |
| [0005](0005-motion-css-first.md) | Motion: CSS scroll-driven animations first, Blazor for state only | Accepted |
| [0006](0006-analytics-privacy.md) | Cookie-free, privacy-first analytics | Proposed |
| [0007](0007-quality-gates.md) | Quality gates in CI (coverage, axe, Lighthouse, no-JS) | Accepted |
| [0008](0008-media-pipeline.md) | Media pipeline: originals in the repo, responsive variants at build time | Accepted |
| [0009](0009-azure-pipelines-and-static-web-apps.md) | CI/CD on Azure Pipelines, hosting on Azure Static Web Apps | Accepted |
| [0010](0010-video-youtube-facade.md) | Video: YouTube Shorts behind a click-to-load facade | Accepted |
| [0011](0011-english-at-the-root.md) | English at the root URL; Portuguese under `/pt/` | Accepted |

## Template

```markdown
# NNNN — Title
- Status: Proposed | Accepted | Superseded by NNNN
- Date: YYYY-MM-DD

## Context
## Decision
## Alternatives considered
## Consequences
```
