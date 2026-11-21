# TRACE — manuelbravo.com.br

> An interactive portfolio built with **.NET 10 / Blazor**.
> Follow a single request as it travels from a phone to the database, and see what changed at each hop along the way.

**Live:** https://www.manuelbravo.com.br · in English, Spanish and Portuguese.

This repository is a portfolio piece in its own right. The site, and also how it was designed, decided, tested and shipped, are all here in the open.

## What's on the site

- **The Trace:** seven layers of a real platform (client → edge → gateway → services → cache → database → observability), each with the problem, the action and the **measured** result.
- **A live benchmark:** the only interactive island (Blazor WebAssembly) replays the Ionic vs native booking-journey benchmark (12–20 s → ~1.2 s).
- **Recruiter Mode:** everything a recruiter screens for on one screen, printable on one A4 page, plus the CV download.
- **Journey, Toolbox, Garage and About:** career history, skills, hobbies (track days, mechanics, games) and how I work.
- **Colophon:** how the site is built, with an architecture diagram, the decisions behind it and how it was **built with AI, directed by a human**.

## How it's built

```
content/*.json ──► Blazor Web App (.NET 10) ──► Trace.Prerender ──► dist/ (static HTML, CSS, WASM, AVIF/WebP)
                                                      │
GitHub ──► Azure Pipelines:  CI (build -warnaserror · format · tests · prerender) ──► CD (Azure Static Web Apps)
                                                                                              │
Visitor ──► www.manuelbravo.com.br (registro.br domain · Cloudflare DNS) ─────────────────────┘
```

- **Static first:** every page × language is rendered to HTML at build time, so the site works without JavaScript and needs no server at run time ([ADR-0003](docs/architecture/adr/0003-build-time-static-prerendering.md)).
- **Content as data:** all copy lives in `content/{en,es,pt-BR}/*.json`; the build fails if a translation, a metric or a key is missing ([ADR-0004](docs/architecture/adr/0004-i18n-content-as-data.md)).
- **Media pipeline:** photos become AVIF/WebP/JPEG at the right sizes, cropped on a focal point, with all metadata stripped ([ADR-0008](docs/architecture/adr/0008-media-pipeline.md)); videos are self-hosted MP4s that load only on click ([ADR-0012](docs/architecture/adr/0012-self-hosted-video.md)).
- **CI/CD:** Azure Pipelines runs CI on every push and pull request, then deploys: `main` → production, pull requests → a preview environment ([ADR-0009](docs/architecture/adr/0009-azure-pipelines-and-static-web-apps.md)).
- **Quality gates:** warnings as errors, format check, 180+ xUnit/bUnit tests, link/`hreflang`/CV/config-size validation in the prerender ([ADR-0007](docs/architecture/adr/0007-quality-gates.md)).

## Run it locally

Requirements: .NET 10 SDK and Node.js (only to serve `dist/` the way Azure Static Web Apps does).

```powershell
./scripts/run-local.ps1              # build, format check, tests, publish, prerender, serve on http://localhost:4280
./scripts/run-local.ps1 -SkipTests   # faster loop while editing content
dotnet run --project src/Trace.Web   # dev server with hot reload (no prerender, no generated images)
```

## Status

| Phase | Scope | State |
|---|---|---|
| 0 — Discovery & UX | Vision, personas, flows, wireframes, design tokens, architecture, ADRs | ✅ |
| 1 — Skeleton & delivery | Blazor Web App (.NET 10), build-time prerender, media pipeline, Azure Pipelines CI → CD to Azure Static Web Apps, custom domain (DNS on Cloudflare) | ✅ |
| 2 — Content & i18n | EN / ES / PT-BR content as data, The Trace (7 stages), the benchmark, Recruiter Mode, Garage, About, Colophon, `hreflang` + sitemap, design fonts | ✅ |
| 3 — Hardening | xUnit + bUnit ✅ · Playwright + axe + Lighthouse CI budgets in the pipeline ⏳ | 🟡 |

## Repository layout

```
content/                 # en/, es/, pt-BR/ JSON content · media/ (photos, gallery.json, videos.json) · cv/
src/Trace.Domain/        # pure C#: content model and validation, routes, benchmark simulation, media plan
src/Trace.Client/        # Blazor WebAssembly island (the benchmark)
src/Trace.Web/           # Blazor Web App: pages, layout, components, styles, fonts
tools/Trace.Prerender/   # renders the site to dist/, generates images, validates the output
tests/                   # xUnit (domain, prerender) and bUnit (components)
docs/                    # product, UX, design, architecture and ADRs
azure-pipelines.yml      # CI → CD
staticwebapp.config.json # headers, caching, 404 (filled in by the prerender)
```

## Documentation map

| Area | Document |
|---|---|
| Product | [Vision & goals](docs/product/vision.md) · [Personas](docs/product/personas.md) · [Content inventory](docs/product/content-inventory.md) |
| UX | [Concept](docs/ux/concept.md) · [Sitemap](docs/ux/sitemap.md) · [User flows](docs/ux/user-flows.md) · [Interaction spec](docs/ux/interaction-spec.md) |
| Content | [Content inventory](docs/product/content-inventory.md) · [Media & how to add photos and videos](content/media/README.md) |
| Design | [Design system](docs/design/design-system.md) · [Tokens (DTCG / Figma)](docs/design/tokens/tokens.json) · [Wireframes](docs/design/wireframes/README.md) |
| Architecture | [Overview (C4)](docs/architecture/overview.md) · [CI/CD & hosting](docs/architecture/ci-cd-and-hosting.md) · [Quality strategy](docs/architecture/quality-strategy.md) · [ADRs](docs/architecture/adr/README.md) |

All diagrams in the docs are written in Mermaid, so GitHub renders them inline. The wireframes are SVG, which you can drag straight into Figma. The tokens follow the W3C Design Tokens format and can be imported into Figma Variables through Tokens Studio.

## Stack

- .NET 10 · Blazor Web App with one Interactive WebAssembly island · build-time static prerender
- Hand-written CSS from design tokens · self-hosted Barlow, Barlow Condensed and JetBrains Mono (OFL)
- NetVips (images) · AngleSharp (HTML validation) · xUnit v3 + bUnit on Microsoft.Testing.Platform
- Azure Pipelines → Azure Static Web Apps (Free) · domain registered at registro.br, DNS on Cloudflare (Free)

## Built with AI

The site was built in partnership with Claude (Anthropic) in Claude Code: I set the intent, the content and the decisions, and reviewed every change; Claude proposed architecture and solutions and wrote the code and tests. Commits carry the co-authorship, and the [Colophon](https://www.manuelbravo.com.br/colophon/) tells the full story.

## Author

**Manuel Bravo** — Lead Software Engineer · .NET · Test automation & quality · AWS
[LinkedIn](https://www.linkedin.com/in/manuelbravoc/)
