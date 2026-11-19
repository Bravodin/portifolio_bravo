# 0006 — Cookie-free, privacy-first analytics

- Status: Proposed
- Date: 2026-11-19

## Context

We need conversion metrics (CV downloads, LinkedIn clicks, how far visitors get in the Trace), but we don't want a cookie banner breaking the first impression, and LGPD/GDPR compliance should be trivial.

## Decision (proposed)

Use **GoatCounter** (free for personal sites, open source, cookie-free) **or Cloudflare Web Analytics** (free, cookie-free) if Cloudflare ends up in front ([ADR-0002](0002-hosting-github-pages.md)). Track custom events only for: `cv_download`, `linkedin_click`, `recruiter_mode`, `dyno_run`, `trace_complete`, `terminal_open`.

## Alternatives considered

- Google Analytics 4: needs a consent banner in practice; heavy.
- No analytics: we couldn't verify the success metrics in the vision.

## Consequences

- ✅ No banner, no personal data
- ⚠️ Aggregated data only, which is enough for a portfolio
