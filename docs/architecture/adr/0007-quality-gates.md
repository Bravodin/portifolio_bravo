# 0007 — Quality gates in CI

- Status: Accepted
- Date: 2026-11-19

## Context

Manuel's profile emphasizes quality engineering (90 % Sonar gates, BDD, load testing before go-live). The portfolio's own pipeline should show it.

## Decision

A PR can't merge unless:

1. `dotnet build -warnaserror` passes and `dotnet format --verify-no-changes` is clean;
2. all unit and bUnit tests pass, with **≥ 90 % line coverage on `Trace.Domain`** (coverlet + ReportGenerator, summary posted to the PR);
3. Playwright E2E (BDD scenarios in [quality-strategy.md](../quality-strategy.md)) pass on Chromium and WebKit;
4. axe-core reports **0 serious/critical** issues on every route × locale;
5. Lighthouse CI meets the budgets (Perf ≥ 90, A11y/BP/SEO = 100, LCP < 2 s, CLS < 0.05);
6. the no-JS crawl confirms that all content is present.

Coverage and Lighthouse badges are shown in the README.

## Alternatives considered

- SonarCloud (free for public repos): nice dashboard, and it can be added later. Coverlet gates are enough for v1.

## Consequences

- ✅ The pipeline itself becomes evidence for the "quality" claims
- ⚠️ A slower CI (~6–8 min), acceptable for a solo project
