# Personas

Four visitors, ordered by business priority. Each design decision should be traceable to at least one of them.

---

## P1 — Renata, Tech Recruiter

| | |
|---|---|
| **Context** | Opens 40+ profiles a day, often on her phone, in between calls |
| **Goal** | Decide in under a minute whether Manuel fits the role, then forward him |
| **Needs** | Current role, years of experience, core stack, languages, location, work model, CV PDF |
| **Frustrations** | Splash screens, scroll-jacking, content hidden behind animations, "click to enter" pages |
| **Success** | Finds everything without scrolling and downloads the PDF in 2 taps |

**Design implications:** Recruiter Mode is reachable from the first viewport on every page. The prerendered HTML already contains the essentials, before WASM loads. There's a sticky "Download CV" action on mobile.

---

## P2 — Diego, Engineering Manager / Hiring Manager

| | |
|---|---|
| **Context** | Hiring a Lead or Staff engineer for a regulated domain (health, insurance, finance) |
| **Goal** | Understand *how* Manuel thinks: trade-offs, impact, leadership |
| **Needs** | Results with numbers and context, team size, modernization stories, quality culture |
| **Frustrations** | Lists of buzzwords, claims without context |
| **Success** | Goes through The Trace, opens 2–3 "case" panels and clicks LinkedIn |

**Design implications:** Each Trace stage follows the format *Problem → Action → Result*. Metrics show their baseline ("from 12–20 s"), not only the final value.

---

## P3 — Ana, Senior Engineer / Technical Interviewer

| | |
|---|---|
| **Context** | Asked to "take a look at this candidate" before the technical interview |
| **Goal** | Judge the craft: code quality, architecture, testing |
| **Needs** | The GitHub repo, ADRs, tests, CI, performance numbers |
| **Frustrations** | Portfolio sites that are just templates |
| **Success** | Opens the repo from the footer, reads an ADR, sees green CI and coverage badges, finds the terminal |

**Design implications:** "View source" is linked from the footer and the terminal. A "How this site was built" panel shows the live Lighthouse score and the build hash.

---

## P4 — Carlos, Peer / Network contact

| | |
|---|---|
| **Context** | Comes from a LinkedIn post or an event, with no hiring intent |
| **Goal** | Get to know Manuel as a person and maybe start a conversation |
| **Needs** | Personality, hobbies, something fun |
| **Success** | Plays with the Dyno, unlocks achievements, reads the Garage section and shares the link |

**Design implications:** The Garage, the achievements and the terminal reward exploring. A share card (Open Graph image) is localized per language.

---

## Priority matrix

| Feature | P1 Recruiter | P2 Manager | P3 Engineer | P4 Peer |
|---|:-:|:-:|:-:|:-:|
| Recruiter Mode + PDF | ●●● | ● | | |
| The Trace (impact by layer) | ● | ●●● | ●● | ●● |
| Dyno demo (feel the latency) | | ●● | ●● | ●●● |
| Service Log (timeline) | ●● | ●● | ● | ● |
| Toolbox (skills) | ●●● | ● | ●● | |
| Garage (hobbies) | | ● | ● | ●●● |
| Terminal + achievements | | | ●●● | ●●● |
| Repo / ADRs / CI | | ● | ●●● | |
