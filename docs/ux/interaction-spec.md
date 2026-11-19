# Interaction specification

How each component behaves. The visual values (colors, durations, easings) are tokens defined in [design-system.md](../design/design-system.md).

## Loading model (progressive enhancement)

| Layer | Delivered by | Available when | What works |
|---|---|---|---|
| L0 — Content | Prerendered static HTML + CSS | First paint | All text, metrics, links, PDF, anchors, print. **No JS at all** |
| L1 — Motion | CSS scroll-driven animations (`animation-timeline: view()`) with an `IntersectionObserver` fallback in a ~3 KB JS module | First paint (CSS) / DOMContentLoaded (JS) | Packet travel, stage reveals, gear indicator (CSS counter), progress bar |
| L2 — Interactive | Blazor WebAssembly (.NET 10) | Runtime loaded (target < 4 s on 4G) | Dyno, terminal, achievements, language memory, case expanders with state |

Rule: **nothing in L0 depends on L1 or L2.** The case expanders use native `<details>` in L0, and Blazor enhances them in L2.

## HUD

| Element | Behavior |
|---|---|
| Gear indicator | Shows `N` in the hero, `1`–`6` for stages 1–6, `PIT` for stage 7, `—` after the Trace. It changes when ≥ 60 % of the next stage is in view. A 150 ms "shift" animation: the digit slides up while the new one slides in from below |
| Progress bar | Width = scroll progress through `#trace` only (0–100 %). Thin, under the HUD |
| Recruiter mode | Always visible. On mobile it shrinks to an icon + "30 s" |
| Language | `PT · EN · ES` segmented control (desktop), menu item (mobile) |
| Fuel gauge | Only on "Start the ride" while WASM loads: a fill that tracks the real runtime download progress. Hidden once it's ready |

## The Trace: packet and spine

- **Spine:** a vertical line on the left (desktop: 96 px from the edge; mobile: 20 px). Stage nodes are circles on the spine.
- **Packet:** a 12 px glowing dot (accent color) whose position along the spine follows scroll position, driven by CSS `scroll-timeline`. On reaching a node it pulses once (300 ms).
- **Stage reveal:** the content fades and translates 24 px → 0 as it enters view (`view()` timeline, `entry 0% → entry 60%`).
- **Metric counter:** when the metric enters view, the *after* value counts up/down from the *before* value over 900 ms, eased with `ease-out-expo`. Screen readers get the final text only (`aria-hidden` on the animated span, a `visually-hidden` final value).

## Dyno (stage 1)

| State | UI |
|---|---|
| Intro (page text, L0) | `stage1.story.benchmark`, just above the dyno: the manager asked for a benchmark, Manuel ran it and presented the numbers, and the dyno replays it |
| Idle | Two lanes, labeled "Legacy · Ionic (2016)" and "Native · Swift/Kotlin". Tachometers at 0. Button "Run the dyno" |
| Running | Both needles rise. Each lane has a timer (`00.0 s`) and a booking-skeleton UI. Native completes at ~1.2 s with a ✓ |
| Skip | After 3 s a link appears: "Skip — you get the idea" |
| Done | Result card: before/after, computed speed-up, then the payoff line `stage1.story.beyondSpeed` (closer to the hardware → wearables), "Run again". Legacy duration = random 12–20 s (seeded per run, shown honestly). The no-JS static card shows the same payoff line |
| Footnote | "Simulation based on measured production timings. No real systems are called." |

Keyboard: the button is focusable, and Space/Enter starts the run. While running, `aria-live="polite"` announces "Native finished in 1.2 seconds" and then "Legacy finished in N seconds".

## Case expander

- Closed: hook + metric + chips + "Open case ▸"
- Open: Problem · Action · Result · *My role* (4 short blocks). Height animates with `interpolate-size: allow-keywords` where supported; otherwise it opens instantly.
- Only one case is open at a time on mobile; on desktop several can be open.

## Service Log

- Desktop: horizontal "odometer" ruler (2008 → 2026) on top. Selecting a year scrolls the list of "revision cards" below it.
- Mobile: vertical list. The odometer turns into a sticky year chip.
- Each card: shop (company), role, period, a highlight metric, and a "service notes" expander.

## Toolbox

- Drawers are tabs on desktop and accordions on mobile.
- Each tool chip opens a popover: years used, where, and one sentence of context. No skill bars.

## Garage (section on the home page)

- 3 cards (Motorcycles, Mechanics, Games), each with the category's `featured` photo cropped around its `focalPoint`, a 2-line story and a "link to work" sentence. A category without a photo shows a line-art illustration.
- Hover / focus on a photo: a subtle 1.03 scale + caption.
- A filmstrip under the cards shows the next 4 thumbnails plus "See all N photos →", which links to `/garage`.

## Gallery (`/garage` route) + lightbox

| Element | Behavior |
|---|---|
| Filter | Chips `All · Track · Workbench · Games` (counts shown). No-JS: anchors to grouped sections |
| Grid | Masonry-like CSS grid (`grid-auto-flow: dense`). Landscape photos span 2 columns on desktop. Thumbnails lazy-loaded with a blurred placeholder |
| Lightbox | Native `<dialog>` (focus handled by the browser). ←/→ or swipe to navigate, Esc to close, counter `3 / 12`, caption + photographer credit, and a "Copy link" that deep-links `/garage#track-03` |
| No-JS | Each thumbnail is a link to the 1600 px image; captions are visible under the thumbnails |
| Reduced motion | No zoom or slide transitions; images swap instantly |

The gallery is mostly CSS and native `<dialog>`, and it needs only ~1 KB of JS for swipe and keyboard navigation. It doesn't need a WASM island.

## Portrait usage

- **Recruiter Mode:** a 112 px portrait (radius `lg`) at the top of the identity card. `fetchpriority="high"`, with the 320 px variant for 2× screens.
- **Pit Stop:** a 64 px round avatar next to the heading, so the "Let's talk" invite comes from a person.
- **Open Graph:** 1200 × 630 card generated at build time per locale (portrait + name + tagline on the `bg` color).

## Pit Stop (contact)

- Primary: "Talk on LinkedIn" (opens in a new tab), "Download CV (PDF)".
- **LinkedIn is the only contact channel.** No e-mail, phone or form anywhere on the site.
- Unlocks `full-trace` if all stages were seen.

## Terminal

- Overlay at the bottom 40 % of the viewport (mobile: full screen). Monospace, blinking caret (no blinking under reduced motion).
- Focus is trapped inside while it's open, and Esc closes it and returns focus to the trigger.
- History with ↑/↓ and Tab completion of commands.
- Output is a list of lines with `role="log"`.

## Motion & accessibility rules

| Rule | Implementation |
|---|---|
| Respect `prefers-reduced-motion: reduce` | Disable all transforms and counters; the packet sits next to the current stage; no parallax |
| User override | "Reduce motion" toggle in the menu (persisted) applies the same class as the media query |
| Focus | Visible 2 px focus ring in the accent color with a 2 px offset, never removed |
| Contrast | Body text ≥ 7:1 on the background; UI ≥ 4.5:1 |
| Targets | ≥ 44 × 44 px on touch |
| Landmarks | `header` (HUD), `nav`, `main`, one `section` per stage with `aria-labelledby`, `footer` |
| Language | `<html lang>` per route; `hreflang` alternates in `<head>` |
| No-JS | Everything in L0 is readable; Dyno shows its static result card |

## Responsive breakpoints

| Name | Min width | Layout |
|---|---|---|
| `sm` | 0 | 1 column, spine 20 px, sticky bottom CTA |
| `md` | 768 px | 1 column, wider spine gutter, HUD shows the language |
| `lg` | 1024 px | Spine + 2 columns per stage (story / metric) |
| `xl` | 1440 px | Max content width 1200 px, centered |
