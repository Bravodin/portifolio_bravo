# 0013 — Contact: LinkedIn and e-mail

- Status: Accepted
- Date: 2026-11-21
- Changes: the "LinkedIn only" contact rule in [vision.md](../../product/vision.md), [interaction-spec.md](../../ux/interaction-spec.md) and CLAUDE.md

## Context

From Phase 0 the site offered one contact channel, LinkedIn, with no e-mail or phone anywhere (site, HTML, CV PDF), mainly to avoid scraping and spam. Tests enforced it.

Manuel now wants recruiters to be able to write to him directly, without a LinkedIn account or a connection request.

## Decision

- **Contact is LinkedIn or e-mail.** The address is `identity.json`'s `email` (the same in every locale, checked by the content validator): `manuelbravoc@gmail.com`.
- It appears next to the LinkedIn button in **Pit Stop** and **Recruiter Mode**, as a `mailto:` link that shows the address itself, so it can be copied and survives printing.
- **Still no phone number**, anywhere.
- The domain keeps rejecting mail (`v=spf1 -all`, DMARC `p=reject`): the published address is a Gmail one, not `@manuelbravo.com.br`.
- The CV PDF is unchanged (LinkedIn only) until Manuel publishes a new one.
- Tests: the content may contain no e-mail address other than this one, and no phone number; Pit Stop and Recruiter Mode have exactly one `mailto:` link and no `tel:` link.

## Alternatives considered

| Option | Why not |
|---|---|
| Keep LinkedIn only | Recruiters without LinkedIn access, or who prefer e-mail, have no way in |
| Contact form | Needs a backend or a third-party form service; the site has no server (ADR-0003) |
| Obfuscated address (assembled by JS, or "name [at] domain") | Breaks for people without JS and for copy-paste; Gmail's spam filter handles scraped-address spam well enough |

## Consequences

- ✅ A direct channel for recruiters, also on the printed Recruiter Mode page
- ⚠️ The address is in the HTML, so scrapers will find it; Gmail's spam filter is the mitigation
