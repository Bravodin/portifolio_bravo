# 0011 — English at the root URL

- Status: Accepted
- Date: 2026-11-19
- Supersedes: the route layout in [ADR-0004](0004-i18n-content-as-data.md) (content as data and invariant globalization still stand)

## Context

[ADR-0004](0004-i18n-content-as-data.md) put Portuguese at the root (`/`) because the domain is `.com.br`, with English under `/en/` and Spanish under `/es/`. Once the site went live, Manuel asked for English first: most recruiters and hiring managers who reach the root URL from LinkedIn read English, and the language switcher already lists EN · ES · PT.

## Decision

- **English is the default locale and lives at the root:** `/`, `/recruiter/`, `/colophon/`, `/garage/`.
- **Spanish** stays under `/es/` and **Portuguese** moves to **`/pt/`** (`/pt/recrutador/`, `/pt/como-foi-feito/`, `/pt/garagem/`). The `<html lang>` and `hreflang` values stay `en`, `es` and `pt-BR`; only the URL prefix is `pt`.
- `x-default` already pointed to English, so search engines get the same answer.
- **No automatic redirect by browser language** (unchanged from ADR-0004).
- **Old URLs keep working:** `staticwebapp.config.json` answers the previous English (`/en/…`) and Portuguese (root) URLs with a **301** to the new ones.

## Alternatives considered

| Option | Why not |
|---|---|
| Keep PT at the root and redirect `/` to `/en/` | Two URLs for the English home, and a redirect on every first visit |
| Pick the language from `Accept-Language` | Already rejected in ADR-0004: bad for crawlers and for people who chose another language |

## Consequences

- ✅ The first page a visitor or crawler sees is in English, matching the switcher order
- ✅ Shorter English URLs, which are the ones shared most
- ⚠️ The validator still compares translations against PT-BR (`Locales.ContentReference`); that's an implementation detail, not a preference
- ⚠️ Links shared before 2026-11-19 rely on the 301s; keep them in the SWA config
