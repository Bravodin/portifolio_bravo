# 0004 — Content as data; i18n by route; invariant globalization

- Status: Accepted
- Date: 2026-11-19

## Context

Three languages (PT-BR, EN, ES). The content changes more often than the code, and every number must trace back to the CV ([content inventory](../../product/content-inventory.md)).

## Decision

- **Content as data:** `content/{locale}/{identity,metrics,stages,log,toolbox,garage,ui}.json`, validated against a JSON Schema in CI. Metrics share stable IDs (`M1`…`M15`) across locales. Each locale has its own copy, but the values are referenced by ID.
- **Routes per locale:** `/` = `pt-BR` (the domain is `.com.br`), `/en/`, `/es/`. Localized slugs (`/recrutador/`, `/en/recruiter/`, `/es/reclutador/`). Each page lists `hreflang` alternates and `x-default` → `/en/`.
- **No automatic redirects.** A dismissible banner suggests the browser's language. Redirects are bad for crawlers and annoying for people.
- **Numbers are pre-formatted in the content** (`"−70 %"`, `"1,2 s"` in PT/ES and `"1.2 s"` in EN), so the WASM islands can run with **`InvariantGlobalization=true`**, which shrinks the payload by dropping ICU data.

## Alternatives considered

- `.resx` + `IStringLocalizer`: fine for UI strings, but awkward for long structured content, and it adds satellite assemblies to the WASM payload.
- A headless CMS: unnecessary for a single author, and it would need a build hook.

## Consequences

- ✅ Manuel edits JSON (or Markdown inside JSON) and pushes, and CI does the rest
- ✅ "Missing key" becomes a build error rather than a runtime surprise
- ⚠️ Pre-formatted numbers mean the Dyno timer formats its decimals with a tiny locale switch in `Trace.Domain`, not with `CultureInfo`
