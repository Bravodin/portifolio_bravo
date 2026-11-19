# 0002 — Host on GitHub Pages with a custom domain

- Status: Superseded by [0009](0009-azure-pipelines-and-static-web-apps.md)
- Date: 2026-11-19

## Context

Hosting must be free, with HTTPS and the custom domain `manuelbravo.com.br`. The repository itself is part of the portfolio (it renews an old GitHub profile).

## Decision

Deploy `dist/` to **GitHub Pages** with the official `actions/upload-pages-artifact` + `actions/deploy-pages`. The canonical host is `www.manuelbravo.com.br`, and the apex redirects to it. `dist/` always contains `CNAME` and `.nojekyll`.

## Alternatives considered

| Option | Pros | Cons |
|---|---|---|
| **GitHub Pages** ✅ | Free; same place as the code; a public, visible pipeline; custom domain + HTTPS | No custom headers; no automatic Brotli; no PR previews |
| Cloudflare Pages | Free; Brotli; headers; PR previews; global CDN | One more account; pipeline split from the repo |
| Azure Static Web Apps (Free) | First-class Blazor support; PR staging environments | Limits on the free tier; the Azure portal adds overhead |
| Netlify / Vercel | Great DX, previews | Commercial free tiers change often |

## Consequences

- ✅ Zero cost, zero servers, and the deployment history is public
- ⚠️ No `Cache-Control` control. If needed, put Cloudflare's free proxy in front, with no code changes
- ⚠️ Without PR previews, PRs upload `dist/` as an artifact instead
- 🔁 Revisit if we need edge functions or headers (move to Cloudflare Pages; the `dist/` contract stays the same)
