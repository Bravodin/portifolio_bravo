# CLAUDE.md — project memory for TRACE (manuelbravo.com.br)

Context for Claude Code sessions in this repository. Read this first, then the docs it links to.

## What this is

Manuel Bravo's interactive portfolio, **TRACE**: the visitor follows a request through the stack (client → edge → gateway → services → cache → database → pit crew), and each layer shows what Manuel modernized there, with measured results. The metaphors come from his hobbies: **motorcycles and mechanics** (gear indicator, dyno, service log, toolbox, garage) and **games** (achievements, terminal easter egg, Konami code).

Start with: `README.md` → `docs/ux/concept.md` → `docs/architecture/overview.md` → `docs/architecture/adr/README.md`.

## Working agreements

- **Talk to Manuel in Brazilian Portuguese.** Write repo docs, code, comments and commit messages in **English**.
- **Numbers come only from `docs/product/content-inventory.md`** (sourced from his CV). Never invent metrics, dates, team sizes or methods. Mark any copy that needs his input with `TODO(manuel)`.
- **Contact is LinkedIn only.** No phone number or e-mail anywhere on the site, in the HTML, or in the published CV PDF.
- **Photos:** every file in `content/media/` must have an entry in `gallery.json` (alt text in pt-BR/en/es, credit for event photos). See `content/media/README.md`.
- **Commits:** Conventional Commits (`feat:`, `fix:`, `docs:`, `ci:`, `test:`, `chore:`), small and focused. Show the diff summary and ask before `git push`.
- **When a decision changes, write a new ADR** that supersedes the old one. Don't rewrite history.
- Accessibility (WCAG 2.2 AA, reduced motion, keyboard) and the performance budgets in `docs/architecture/overview.md` §6 are requirements, not nice-to-haves.

## Status

- ✅ **Phase 0** — product, UX, wireframes (`docs/design/wireframes`), design tokens, architecture, ADRs 0001–0008, photos + media manifest.
- ⏳ **Phase 1/2 — next:** skeleton + pipeline (below).

## Pending decision to record (do this first)

Manuel decided to **run CI/CD on Azure DevOps (Azure Pipelines)** and **host on Azure Static Web Apps (Free plan)** instead of GitHub Actions + GitHub Pages. The code stays on **GitHub** (public portfolio), connected to Azure Pipelines.

1. Write `docs/architecture/adr/0009-azure-pipelines-and-static-web-apps.md` (status Accepted, supersedes 0002). Mark 0002 as "Superseded by 0009" and update the ADR index. Rationale:
   - SWA Free gives 100 GB/month bandwidth, 2 custom domains, 3 preview environments (one per PR), 250 MB per environment, 15k files, free SSL.
   - `staticwebapp.config.json` gives custom headers (long-lived cache for fingerprinted `_framework/*`, CSP), a real 404 (`responseOverrides`) and Brotli. This removes the GitHub Pages limitations listed in the overview's risks.
   - It matches Manuel's Microsoft/Azure profile.
   - Caveats: new Azure DevOps orgs need the **free parallelism grant** (form: https://aka.ms/azpipelines-parallelism-request, takes days; a self-hosted agent works meanwhile). Public project = 10 free parallel jobs; private = 1,800 min/month. The **apex domain** needs ALIAS/ANAME/CNAME flattening, which registro.br's DNS likely doesn't offer → move DNS to **Cloudflare (free)** and keep the registration at registro.br. Canonical host: `www.manuelbravo.com.br`; apex redirects to www.
2. Rewrite `docs/architecture/ci-cd-and-hosting.md` for `azure-pipelines.yml` + SWA (deployment token stored as a secret pipeline variable; PR builds deploy to a preview environment; `main` deploys to production). Remove the GitHub Pages specifics (`CNAME`, `.nojekyll`, A records) from the docs, the overview and ADR-0003.

## Phase 1/2 plan — skeleton

Solution (see `docs/architecture/overview.md` §5):

```
global.json                    # pin .NET 10 SDK
Trace.sln
src/Trace.Domain/              # net10.0 class library — AchievementService, DynoSimulation, TerminalInterpreter, content records
src/Trace.Client/              # Blazor WASM islands (InteractiveWebAssembly)
src/Trace.Web/                 # Blazor Web App host, static SSR pages
tools/Trace.Prerender/         # in-process TestServer crawler → dist/ (ADR-0003) + media step (ADR-0008, NetVips)
tests/Trace.Domain.Tests/      # xUnit, ≥ 90 % coverage gate
tests/Trace.Components.Tests/  # bUnit
tests/Trace.E2E/               # Playwright for .NET + axe (later)
content/{pt-BR,en,es}/*.json   # content as data (ADR-0004)
staticwebapp.config.json
azure-pipelines.yml
```

Order:
1. ADR-0009 + docs updates (above).
2. `global.json`, solution, projects, `Directory.Build.props` (nullable, warnings as errors, analyzers), `.editorconfig`.
3. **Spike (ADR-0003 exit criteria):** one prerendered page + one `InteractiveWebAssembly` island (a tiny Dyno) → deployed to SWA. Confirm that it hydrates with static GETs only.
4. `Trace.Prerender` minimal: routes × locales → `dist/`, plus `sitemap.xml` and `404.html`.
5. `azure-pipelines.yml`: restore → build → test + coverage → prerender → `AzureStaticWebApp@0` deploy (`skip_app_build: true`, `app_location: dist`).
6. Then content JSON + the Recruiter Mode page (highest value for recruiters).

## Useful commands

```bash
dotnet --version                                   # expect 10.0.x
dotnet build -warnaserror
dotnet test --collect:"XPlat Code Coverage"
dotnet run --project tools/Trace.Prerender -- --out dist
python3 docs/design/wireframes/_src/wf_desktop.py docs/design/wireframes   # regenerate wireframes (needs Pillow)
python3 docs/design/wireframes/_src/wf_mobile.py  docs/design/wireframes
```

## Manuel's environment

Windows, Visual Studio 2026 Community, .NET 10 SDK, Git, and the Claude Code CLI. GitHub user: `Bravodin`. LinkedIn: `manuelbravoc`. Domain `manuelbravo.com.br` (registro.br).
