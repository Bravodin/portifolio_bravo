# TRACE — manuelbravo.com.br

> An interactive portfolio built with **.NET 10 / Blazor WebAssembly**.
> Follow a single request as it travels from a phone to the database, and see what changed at each hop along the way.

This repository is a portfolio piece in its own right. The site, and also how it was designed, decided, tested and shipped, are all here in the open.

## Status

| Phase | Scope | State |
|---|---|---|
| 0 — Discovery & UX | Vision, personas, flows, wireframes, design tokens, architecture, ADRs | ✅ |
| 1 — Skeleton | Blazor Web App (.NET 10), build-time prerender (`Trace.Prerender`), media pipeline, local run that mirrors CI | ✅ · Azure Pipelines → Azure Static Web Apps and the custom domain: ⏳ |
| 2 — Content & i18n | PT-BR / EN / ES content as data, Recruiter Mode, Garage, Colophon, `hreflang` + sitemap | ✅ |
| 3 — Immersion | The Trace (7 stages) and the Dyno ✅ · motion, gear indicator, achievements, terminal ⏳ | 🟡 |
| 4 — Hardening | xUnit + bUnit (180+ tests) ✅ · Playwright + axe + Lighthouse CI budgets in the pipeline ⏳ | 🟡 |

## Documentation map

| Area | Document |
|---|---|
| Product | [Vision & goals](docs/product/vision.md) · [Personas](docs/product/personas.md) · [Content inventory](docs/product/content-inventory.md) |
| UX | [Concept](docs/ux/concept.md) · [Sitemap](docs/ux/sitemap.md) · [User flows](docs/ux/user-flows.md) · [Interaction spec](docs/ux/interaction-spec.md) |
| Content | [Content inventory](docs/product/content-inventory.md) · [Media & how to add photos](content/media/README.md) |
| Design | [Design system](docs/design/design-system.md) · [Tokens (DTCG / Figma)](docs/design/tokens/tokens.json) · [Wireframes](docs/design/wireframes/README.md) |
| Architecture | [Overview (C4)](docs/architecture/overview.md) · [CI/CD & hosting](docs/architecture/ci-cd-and-hosting.md) · [Quality strategy](docs/architecture/quality-strategy.md) · [ADRs](docs/architecture/adr/README.md) |

All diagrams are written in Mermaid, so GitHub renders them inline. The wireframes are SVG, which you can drag straight into Figma. The tokens follow the W3C Design Tokens format and can be imported into Figma Variables through Tokens Studio.

## Stack (planned)

- .NET 10 · Blazor Web App (Interactive WebAssembly) · build-time static prerender
- CSS scroll-driven animations + a thin JS interop module
- bUnit · Playwright for .NET · axe-core · Lighthouse CI
- Azure Pipelines → Azure Static Web Apps (Free) · custom domain `www.manuelbravo.com.br` (DNS on Cloudflare)

## Author

**Manuel Bravo** — Lead Software Engineer · .NET · Test automation & quality · AWS
[LinkedIn](https://www.linkedin.com/in/manuelbravoc/)
