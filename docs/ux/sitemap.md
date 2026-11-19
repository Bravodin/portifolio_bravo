# Sitemap & information architecture

## Routes

Each route exists in three locales and is **prerendered to static HTML** at build time.

| Route (PT-BR default) | EN | ES | Purpose | Persona |
|---|---|---|---|---|
| `/` | `/en/` | `/es/` | The ride: hero + Trace + Service Log + Toolbox + Garage + Pit Stop | All |
| `/recrutador/` | `/en/recruiter/` | `/es/reclutador/` | Recruiter Mode: one screen, printable | P1 |
| `/como-foi-feito/` | `/en/colophon/` | `/es/como-se-hizo/` | How this site was built: stack, ADRs, Lighthouse, build hash | P3 |
| `/garagem/` | `/en/garage/` | `/es/garaje/` | Full photo gallery (track days, workbench, games), filterable by category, with a lightbox | P4 |
| `/404.html` | — | — | Localized "Wrong turn" page (detects locale from the path) | All |
| `/cv/manuel-bravo-{pt,en,es}.pdf` | | | Static PDF (no phone or e-mail; LinkedIn only) | P1 |

Global overlays (not routes): **Terminal** (`~` or `` ` `` key, or the footer link) and **Achievements drawer**.

## Page map

```mermaid
flowchart TD
    subgraph HOME["/ — The Ride (single long page)"]
        H0["#start · Hero / Ignition (gear N)"]
        subgraph TRACE["#trace · The Trace"]
            T1["#client · 1 Client — Dyno demo"]
            T2["#edge · 2 Edge & Security"]
            T3["#gateway · 3 API Gateway"]
            T4["#services · 4 Services"]
            T5["#cache · 5 Cache & Messaging"]
            T6["#database · 6 Database"]
            T7["#pit · PIT — Pit Crew (Observability & Quality)"]
        end
        S1["#service-log · Service Log (career)"]
        S2["#toolbox · Toolbox (skills)"]
        S3["#garage · Garage (hobbies)"]
        S4["#pit-stop · Pit Stop (contact)"]
        H0 --> T1 --> T2 --> T3 --> T4 --> T5 --> T6 --> T7 --> S1 --> S2 --> S3 --> S4
    end

    R["/recruiter — Recruiter Mode"]
    C["/colophon — How it was built"]
    G["/garage — Photo gallery"]
    PDF[("CV PDF")]
    GH(["GitHub repo"])
    LI(["LinkedIn"])

    H0 -- "Recruiter mode (30 s)" --> R
    R -- "Take the ride instead" --> H0
    R --> PDF
    R --> LI
    S4 --> PDF
    S4 --> LI
    S4 --> C
    S3 -- "See all photos" --> G
    G -- "Back to the ride" --> S3
    C --> GH
```

## Global navigation

| Element | Desktop | Mobile |
|---|---|---|
| **HUD** (top bar) | Logo/monogram · gear indicator · trace progress bar · language · Recruiter mode · menu | Monogram · gear · Recruiter mode · menu |
| **Section menu** | Off-canvas list of the 12 anchors, current one highlighted | Full-screen sheet |
| **Sticky CTA** | — | Bottom bar: "CV (PDF)" + "LinkedIn" after the visitor scrolls past the hero |
| **Footer** | Contact · Colophon · Source on GitHub · Terminal hint · ©/build hash | Same, stacked |

## Content hierarchy per Trace stage

```
Stage header      [gear #] [layer name] [garage metaphor icon]
Hook              one line: the problem in plain words
Metric            BEFORE → AFTER  (big)   ·  Δ chip
Case (expand)     Problem · Action · Result · My role
Tech chips        5–7 chips max
Achievement       unlocked when the stage is 60 % in view
```
