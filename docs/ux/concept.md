# Experience concept — "TRACE"

## The big idea

> **Follow one request through the whole stack, and at every hop, see what Manuel did there.**

Most portfolios are organized by *employer*. This one is organized by *system layer*: the way a Lead Engineer actually thinks when something breaks in production. The visitor "rides" a request from the phone in someone's hand down to the database, and each layer is a stop that tells a story of modernization with a measured result.

Three metaphor layers sit on top of each other. Each one is light enough to ignore, but together they give the site a personality:

| Layer | Source | Role in the site | Where it shows |
|---|---|---|---|
| **The Stack** | Career: end-to-end troubleshooting | *Structure.* The content is organized by layer | Trace stages 1–7 |
| **The Garage** | Hobby: motorcycles and mechanics | *Voice and visuals.* Gauges, gears, service log, toolbox | HUD, Dyno, Service Log, Toolbox, Garage |
| **The Game** | Hobby: games | *Motivation.* Progress, achievements, secrets | HUD progress, achievements, terminal, Konami code |

**Rule of thumb:** the Stack carries the information, the Garage adds the look and feel, and the Game rewards exploring. If a metaphor ever gets in the way of reading the information, the metaphor loses.

## The ride, in one paragraph

You land on a dark garage-at-night page. Manuel's name, role and tagline are readable right away, with two buttons: **Recruiter mode (30 s)** and **Start the ride**. Start it, and a gear indicator in the HUD goes from **N** to **1**. A glowing packet leaves a phone at the top of the screen. As you scroll it travels down the spine of the stack: through the app (where you can *run the Dyno* and feel 15 s versus 1.2 s), past the security edge, into the gateway, through the rebuilt service engine, the cache and the database, and finally into the **pit**, where the crew of tests, monitoring and AI keeps everything running. Each gear change unlocks a stage, and each stage ends with a result. After the pit stop you reach the **Service Log** (career history), the **Toolbox**, the **Garage** (the person behind the work) and the **Pit Stop** (contact).

## Experience principles

1. **Content first, spectacle second.** All the information is in prerendered HTML. Animation only enhances it. With JS off, the site is still a complete, well-written CV.
2. **Never trap the visitor.** No scroll-jacking and no forced intro. Native scrolling, anchors, and a skip link that always works.
3. **Show the before.** A result without its baseline means nothing, so every metric is shown as *before → after*.
4. **Felt, not read.** Wherever possible, let the visitor *experience* the result (Dyno), not just read about it.
5. **Respect the 30 seconds.** Recruiter Mode is one tap away from every viewport.
6. **Reward curiosity.** Secrets for people who explore (terminal, Konami code, achievements), with no penalty for people who don't.
7. **Accessible is non-negotiable.** WCAG 2.2 AA, `prefers-reduced-motion` honored, full keyboard path.

## Tone of voice

| Do | Don't |
|---|---|
| Speak in the first person, plainly: "I led the migration…" | "Passionate rockstar ninja" |
| Use numbers with context: "from 12–20 s to ~1.2 s" | Superlatives with no numbers |
| Light garage humor in microcopy: "Pit stop", "Run the dyno" | Puns in the body text of cases |
| Use short sentences, written for scanning | Paragraphs longer than 3 lines |

## What makes it memorable (the "3 moments")

1. **The Dyno**: watching the legacy spinner crawl while the native one is already done.
2. **The gear indicator**: N → 1 → … → 6 → PIT. A small detail that makes scrolling feel like riding.
3. **`sudo hire manuel`**: the terminal punchline that technical reviewers will screenshot and share.
