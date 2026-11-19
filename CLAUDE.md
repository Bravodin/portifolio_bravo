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

- ✅ **Phase 0** — product, UX, wireframes (`docs/design/wireframes`), design tokens, architecture, ADRs 0001–0009, photos + media manifest.
- ✅ Hosting decision recorded: **Azure Pipelines + Azure Static Web Apps (Free)**, DNS on Cloudflare, apex → `www` redirect ([ADR-0009](docs/architecture/adr/0009-azure-pipelines-and-static-web-apps.md), [ci-cd-and-hosting.md](docs/architecture/ci-cd-and-hosting.md)). ADR-0002 (GitHub Pages) is superseded.
- ✅ Skeleton (step 2) and local spike (step 3): the Dyno island hydrates from static files only. Results and the findings that shape the prerender are in [ADR-0003](docs/architecture/adr/0003-build-time-static-prerendering.md#spike-results-2026-11-19-local).
- ✅ Step 4: `Trace.Prerender` builds `dist/` from the publish output (details in ADR-0003, "Step 4 result"). The media step (ADR-0008, NetVips) is not written yet.
- ⏳ **Next:** step 5, `azure-pipelines.yml`. Manuel has no Azure account yet, so the SWA production check waits for it.
- 🎬 4 YouTube Shorts (@mustelasportmedia, engine sound only) are listed in `content/media/videos.json`. They'll be shown through a click-to-load facade ([ADR-0010](docs/architecture/adr/0010-video-youtube-facade.md)). The facade component and the poster download (Prerender media step) come with the Garage. Open items are in `content/media/README.md`.

## Phase 1/2 plan — skeleton

Solution (see `docs/architecture/overview.md` §5):

```
global.json                    # pin .NET 10 SDK + Microsoft.Testing.Platform runner
Trace.slnx
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
1. ~~ADR-0009 + docs updates~~ ✅ done.
2. ~~`global.json`, solution, projects, `Directory.Build.props`, `.editorconfig`~~ ✅ done (central package management in `Directory.Packages.props`; xUnit v3 + bUnit on Microsoft.Testing.Platform with `coverlet.MTP`).
3. ~~**Spike (ADR-0003 exit criteria)**~~ ✅ passed locally (SWA CLI). The deploy to the real SWA happens with step 5.
4. ~~`Trace.Prerender` minimal~~ ✅ done. Still to add: the media step (ADR-0008), `hreflang` validation once locales exist, and removing the antiforgery token from the HTML.
5. `azure-pipelines.yml`: restore → build → test + coverage → prerender → `AzureStaticWebApp@0` deploy (`skip_app_build: true`, `app_location: dist`).
6. Then content JSON + the Recruiter Mode page (highest value for recruiters).

## Useful commands

```bash
./scripts/run-local.ps1                            # everything the pipeline does, then serve dist/ on :4280 (-SkipTests, -NoServe)
dotnet run --project src/Trace.Web                 # dev server with hot reload (not the static build)
dotnet --version                                   # expect 10.0.x
dotnet build Trace.slnx -warnaserror
dotnet test --solution Trace.slnx                  # exit code 8 = a test project ran zero tests
dotnet test --solution Trace.slnx --coverlet --coverlet-output-format cobertura --results-directory artifacts/test-results
dotnet format Trace.slnx --verify-no-changes
dotnet publish src/Trace.Web -c Release -o artifacts/publish
dotnet run --project tools/Trace.Prerender -c Release -- --publish-dir artifacts/publish --out dist
npx @azure/static-web-apps-cli start dist --port 4280 --swa-config-location dist   # serve dist/ like SWA (needs Node)
python3 docs/design/wireframes/_src/wf_desktop.py docs/design/wireframes   # regenerate wireframes (needs Pillow)
python3 docs/design/wireframes/_src/wf_mobile.py  docs/design/wireframes
```

## Manuel's environment

Windows, Visual Studio 2026 Community, .NET 10 SDK, Git, and the Claude Code CLI. GitHub user: `Bravodin`. LinkedIn: `manuelbravoc`. Domain `manuelbravo.com.br` (registro.br).
