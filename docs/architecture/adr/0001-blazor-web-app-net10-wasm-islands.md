# 0001 — Blazor Web App on .NET 10 with interactive WebAssembly islands

- Status: Accepted
- Date: 2026-11-19

## Context

The portfolio has to show modern .NET skill (the core of Manuel's profile). It also has to load fast for recruiters on phones, be indexable, and cost nothing to host. A whole-page Blazor WebAssembly SPA would make the ~2 MB runtime part of the critical path and hurt both LCP and SEO.

## Decision

Use a **Blazor Web App (.NET 10, LTS)**:

- All pages are **static server-side rendered** components (no interactivity by default).
- Only three components are interactive, with `@rendermode InteractiveWebAssembly`: `DynoIsland`, `TerminalIsland` and `AchievementsIsland`. They live in `Trace.Client`.
- Business logic lives in `Trace.Domain` (plain `net10.0`), with no UI dependencies.
- The output is rendered to static files at build time ([ADR-0003](0003-build-time-static-prerendering.md)). There's no server at runtime.

The .NET 10 features we use: declarative persistent component state (`[PersistentState]`) to hand content to the islands without fetching, resource preloading for the WASM boot, and fingerprinted static assets.

## Alternatives considered

| Option | Why not |
|---|---|
| Standalone Blazor WASM SPA | Runtime on the critical path; content invisible until boot; weak SEO |
| Blazor Server / interactive Server mode | Needs an always-on server (cost, cold starts) |
| Static site generator (Astro, Hugo) + a small Blazor widget | Faster to build, but it no longer shows .NET as the craft |
| ASP.NET Core Razor Pages, prerendered | No client interactivity in C# |

## Consequences

- ✅ Content-first performance profile, with .NET everywhere
- ✅ Islands can be tested in isolation with bUnit
- ⚠️ Islands-on-a-static-host must be validated early (Phase 1 spike)
- ⚠️ Two render contexts to keep in mind (static SSR vs. WASM). Shared components must not assume interactivity.
