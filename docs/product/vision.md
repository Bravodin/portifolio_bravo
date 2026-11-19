# Product vision

## One-liner

**A portfolio you ride through, not read.** A visitor follows a single request from a phone, through firewall, gateway, microservices, cache and database, and finds out at each hop what Manuel built, fixed or modernized there, along with the measured result.

## Why this exists

| Problem | Consequence | Answer |
|---|---|---|
| The GitHub profile is old and doesn't reflect current seniority | Technical reviewers find little to judge | This repo *is* the showcase: clean architecture, tests, ADRs, CI |
| A PDF CV lists results, but nobody *feels* them | "12–20 s → 1.2 s" reads like any other bullet point | Interactive demos let visitors experience the before and after |
| Recruiters scan for ~30 s | Immersive sites often bury the essentials | **Recruiter Mode**: one screen with the key facts plus the PDF, always one click away |
| Opportunities come from BR, LATAM and global markets | Single-language sites lose part of the audience | PT-BR / EN / ES, each one prerendered and indexable |

## Positioning statement

> For **hiring managers and tech leads** looking for a senior engineer who can modernize critical platforms without breaking production, **Manuel Bravo** is a Lead Engineer who has spent his career turning legacy systems into fast, secure, tested ones, **unlike** profiles that only list technologies, because every claim here is tied to a measured result you can explore.

## Core narrative

The site's message rests on three ideas, each of which shows up throughout the content:

1. **Diagnose end to end.** He follows the problem all the way from the client to the database.
2. **Restore and modernize.** He takes legacy systems (VB6, SOAP, Ionic) and rebuilds them as modern stacks while production keeps running.
3. **Quality is the throttle.** Test automation, load tests and coverage gates are what let a team go fast safely.

The hobbies (motorcycles, mechanics, games) aren't a footnote. They're the **metaphor system**: a mechanic diagnoses by following the system, a restorer respects the original while upgrading it, and a gamer understands progression and feedback loops.

## Goals and success metrics

| Goal | Metric | Target |
|---|---|---|
| Convert visits into conversations | Clicks on LinkedIn / CV download per unique visitor | ≥ 8 % |
| Respect the recruiter's time | Time until Recruiter Mode is reachable | 0 s (visible in the first viewport) |
| Prove the craft | Lighthouse Performance / Accessibility / Best Practices / SEO (mobile) | ≥ 90 / 100 / 100 / 100 |
| Be fast despite WASM | LCP on a simulated 4G connection | < 2.0 s (prerendered HTML; WASM loads afterwards) |
| Get found | Google search for "Manuel Bravo .NET" | Top 3 within 60 days |
| Engage | Visitors who reach the end of The Trace | ≥ 35 % of immersive sessions |

Analytics use a privacy-friendly, cookie-free provider (see [ADR-0006](../architecture/adr/0006-analytics-privacy.md)).

## Scope

**In (v1):** landing, Recruiter Mode, The Trace (7 stages), Dyno demo, Service Log (timeline), Toolbox (skills), Garage (hobbies), Pit Stop (contact), terminal easter egg, achievements, three languages, dark theme, reduced-motion mode.

**Out (v1):** blog, CMS, contact form with a backend, light theme, 3D/WebGL scenes (could come in v2), user accounts.

## Constraints

- **Hosting cost: zero.** The site is static output only (Azure Static Web Apps, Free plan; CI on Azure Pipelines).
- **Domain:** `manuelbravo.com.br`, already owned, with DNS at registro.br.
- **Contact = LinkedIn only.** No e-mail or phone number on the site, in the HTML source or in the public CV PDF (it lists only the LinkedIn URL). This also avoids scraping and spam.
- **Confidentiality:** use only the figures already published in the CV, and don't include internal screenshots or employer code.
