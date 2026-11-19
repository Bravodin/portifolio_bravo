# 0009 — CI/CD on Azure Pipelines, hosting on Azure Static Web Apps

- Status: Accepted
- Date: 2026-11-19
- Supersedes: [0002](0002-hosting-github-pages.md)

## Context

[ADR-0002](0002-hosting-github-pages.md) chose GitHub Actions + GitHub Pages. Its known costs were no custom headers (so no long-lived cache for fingerprinted assets and no CSP header), no Brotli, no real 404 handling beyond a single `404.html`, no PR previews, and the Jekyll and domain-takeover workarounds (`.nojekyll`, TXT verification).

Manuel's professional profile is Microsoft/Azure. A pipeline on Azure DevOps and hosting on Azure show that stack in the portfolio itself, and the hosting can still be free.

The code stays on **GitHub**. The repository is public and part of the portfolio.

## Decision

- **CI/CD:** **Azure Pipelines** (Azure DevOps), defined in `azure-pipelines.yml` at the repo root and connected to the GitHub repository through the Azure Pipelines GitHub App. The pipeline: restore → build → test + coverage → prerender → deploy.
- **Hosting:** **Azure Static Web Apps, Free plan**. The pipeline builds `dist/` itself and deploys it with the `AzureStaticWebApp@0` task (`app_location: dist`, `skip_app_build: true`, `skip_api_build: true`). The SWA deployment token is a **secret pipeline variable**.
  - Pushes to `main` deploy to **production**.
  - PR builds deploy to a **named preview environment** (one per PR).
- **Configuration:** `staticwebapp.config.json` (copied into `dist/`) defines:
  - `globalHeaders`: CSP, `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`;
  - long-lived `Cache-Control: public, max-age=31536000, immutable` for fingerprinted files only (`_framework/*` and versioned fonts and images), and short caching for HTML;
  - `responseOverrides` for `404` → `/404.html` with a real 404 status;
  - MIME types for `.wasm`, `.dat` and `.blat` if the defaults don't cover them (to be checked in the spike).
- **Domain and DNS:** the domain stays **registered at registro.br**, but its **nameservers move to Cloudflare (free plan)**.
  - Canonical host: **`www.manuelbravo.com.br`**, a SWA custom domain (`CNAME www → <app>.azurestaticapps.net`, DNS-only / grey cloud so that SWA issues and renews the certificate).
  - The apex `manuelbravo.com.br` **redirects (301) to `www`**. Registro.br's DNS doesn't offer ALIAS/ANAME/CNAME flattening, which an apex pointing at SWA needs, so the apex needs Cloudflare. The exact setup is in [ci-cd-and-hosting.md](../ci-cd-and-hosting.md#domain-and-dns).

## Why SWA Free is enough

| Limit (Free plan) | Value | Our need |
|---|---|---|
| Bandwidth | 100 GB / month | A portfolio; far below |
| Custom domains | 2 | `www` (+ apex if ever needed) |
| Preview (staging) environments | 3 | One per open PR; solo project |
| App size per environment | 250 MB | `dist/` is a few MB plus images |
| Files per environment | 15,000 | Well below |
| SSL | Free, managed | ✅ |

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **Azure Pipelines + SWA Free** ✅ | Headers, Brotli, real 404, PR previews, free SSL; matches Manuel's Azure profile | A second account (Azure DevOps); the parallelism grant; apex DNS needs Cloudflare |
| GitHub Actions + GitHub Pages ([ADR-0002](0002-hosting-github-pages.md)) | Everything in one place; simplest | The limitations listed in Context |
| GitHub Actions + SWA | SWA's built-in workflow; automatic PR environments | Doesn't show Azure DevOps, which is part of the profile |
| Cloudflare Pages | Headers, Brotli, previews | Doesn't match the Azure profile |

## Consequences

- ✅ Custom headers: immutable caching for `_framework/*` and a CSP header instead of a `<meta>` tag
- ✅ Brotli/gzip compression served by the platform; a real 404 status
- ✅ PR previews let a reviewer open the change before merging
- ✅ The removal of the GitHub Pages workarounds: no `CNAME`, no `.nojekyll`, no GitHub domain verification
- ⚠️ **Parallelism grant:** new Azure DevOps organizations have no free Microsoft-hosted parallel jobs until Microsoft approves the request (form: <https://aka.ms/azpipelines-parallelism-request>, takes a few business days). Until then, a **self-hosted agent** on Manuel's machine runs the pipeline. After approval: a public project gets 10 free parallel jobs; a private project gets 1 job with 1,800 minutes/month.
- ⚠️ **Preview environments aren't removed automatically** when a PR closes, unlike the SWA GitHub Actions integration. The Free plan allows 3, so stale ones must be deleted (portal or `az staticwebapp environment delete`), or the pipeline cleans them up later.
- ⚠️ **PRs from forks** don't get secret variables, so they build and test but don't deploy a preview. That's the safe default and is fine for a solo repo.
- ⚠️ `responseOverrides` is site-wide, so there's one `404.html` for all locales. It must work in all three languages (see the ADR-0003 spike criteria).
- ⚠️ The DNS move to Cloudflare is a one-time step outside the repo, and nameserver changes can take hours to propagate.
- 🔁 The `dist/` contract from [ADR-0003](0003-build-time-static-prerendering.md) is unchanged. Moving to another static host later only means replacing the deploy step and translating `staticwebapp.config.json`.
