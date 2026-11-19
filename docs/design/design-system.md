# Design system — "Garage at Night"

A dark, warm and technical look: the light of a workbench lamp on asphalt, the amber of a turn signal, and the cyan of an instrument cluster. The tokens live in [`tokens/tokens.json`](tokens/tokens.json) (W3C DTCG format; import into Figma with *Tokens Studio* → *Export to Figma Variables*). In code they become CSS custom properties (`--color-bg`, `--space-4`…).

## Color

| Token | Hex | Use | Contrast on `bg` |
|---|---|---|---|
| `color.bg` | `#0E0F11` | Page background (asphalt) | — |
| `color.surface` | `#16181B` | Sections, cards | — |
| `color.raised` | `#1F2226` | Popovers, terminal, case panels | — |
| `color.line` | `#2E3238` | Decorative dividers, spine track | 1.5:1 (decorative only) |
| `color.line-strong` | `#6B717A` | Borders that carry meaning (inputs, focusable outlines) | 3.9:1 ✓ (≥ 3:1 non-text) |
| `color.text` | `#ECEAE4` | Primary text (warm paper white) | 15.9:1 ✓ AAA |
| `color.text-2` | `#A8ABB0` | Secondary text | 8.3:1 ✓ AAA |
| `color.muted` | `#8A8E95` | Captions, footnotes | 5.8:1 ✓ AA |
| `color.amber` | `#FFB020` | **Primary accent**: CTAs, packet, focus ring, gear | 10.5:1 ✓ |
| `color.cyan` | `#3DD6F5` | **Data accent**: HUD, gauges, chips, links in body | 11.1:1 ✓ |
| `color.after` | `#4ADE80` | The "after" value of metrics | 11.0:1 ✓ |
| `color.before` | `#FF6B66` | The "before" value of metrics (always paired with a label, never color alone) | 6.9:1 ✓ |

The primary button has `bg` text on `amber`: 10.5:1 ✓.

**Rules:** amber marks *action and progress*, cyan marks *information*. Never put amber and cyan next to each other at large sizes. The before/after colors always come with an arrow and a text label.

## Typography

| Role | Family | Weights | Why |
|---|---|---|---|
| Display / numbers | **Barlow Condensed** | 600, 700 | Condensed and mechanical, close to gauges and motorsport livery; reads well for big figures |
| Body / UI | **Barlow** | 400, 500, 600 | Same family as the display, warm and very legible |
| Mono (HUD, terminal, code) | **JetBrains Mono** | 400, 600 | Code credibility, tabular numerals for timers |

All of them are OFL-licensed and **self-hosted** as subset `woff2` (Latin + Latin-Ext covers PT/ES diacritics), with `font-display: swap` and the display face preloaded.

Fluid type scale (ratio 1.25, `clamp()` between a 360 px and a 1440 px viewport):

| Token | Min → Max | Use |
|---|---|---|
| `font.size.metric` | 56 → 112 px | Big after-value in a stage |
| `font.size.h1` | 40 → 72 px | Name in the hero |
| `font.size.h2` | 30 → 48 px | Stage / section title |
| `font.size.h3` | 22 → 28 px | Card title |
| `font.size.body` | 17 → 18 px | Body (line-height 1.6) |
| `font.size.small` | 14 → 15 px | Captions, chips |
| `font.size.hud` | 12 → 13 px | HUD labels, uppercase, letter-spacing 0.08em |

Line length: max `68ch` for body text.

## Space, radius, elevation

- **Space:** 4 px base: `1=4 · 2=8 · 3=12 · 4=16 · 6=24 · 8=32 · 12=48 · 16=64 · 24=96 · 32=128`. Stages are separated by `space.32` on desktop and `space.16` on mobile.
- **Radius:** `sm=2` (chips, HUD) · `md=6` (buttons, inputs) · `lg=12` (cards, terminal). The look is machined rather than bubbly.
- **Elevation:** no drop shadows on dark. Instead, `raised` surfaces + a 1 px `line` border + an optional amber/cyan *glow* (`0 0 24px color/25%`) only for the packet and the active gear.

## Motion

| Token | Value | Use |
|---|---|---|
| `motion.duration.shift` | 150 ms | Gear change, toggles |
| `motion.duration.pulse` | 300 ms | Packet reaching a node, toast in |
| `motion.duration.reveal` | 600 ms | Stage content reveal (when not scroll-linked) |
| `motion.duration.counter` | 900 ms | Metric counters |
| `motion.easing.out` | `cubic-bezier(0.16, 1, 0.3, 1)` | Default (expo-out) |
| `motion.easing.inout` | `cubic-bezier(0.65, 0, 0.35, 1)` | Packet travel |

Under `prefers-reduced-motion` all durations become `0ms` and the scroll-linked animations are removed (see the [interaction spec](../ux/interaction-spec.md#motion--accessibility-rules)).

## Iconography & imagery

- **Icons:** 24 px line icons, 1.5 px stroke, square caps (Lucide-compatible set + 7 custom stage icons: phone, key, injector, engine block, gearbox, oil can, wrench + scanner).
- **Illustration:** schematic line art only (blueprint / exploded-view style, in `line-strong` + one accent). No 3D, no stock imagery.
- **Photos:** only Manuel's own photos ([`content/media`](../../content/media/README.md)). Originals are untouched. On the page they sit on `raised` with a 1 px `line` border and a bottom gradient (`bg` → transparent, 40 %) behind captions. Event photos always show the photographer credit.
- **Texture:** an optional 2 % noise overlay on `bg` (CSS, < 1 KB) for an asphalt feel. Off under `prefers-reduced-transparency`.

## Components (inventory for v1)

| Component | Variants | Notes |
|---|---|---|
| `Button` | primary (amber) · secondary (outline `line-strong`) · ghost · icon | Min 44 px height; loading state shows the fuel-gauge fill |
| `Hud` | desktop · mobile | Sticky, `surface` at 85 % opacity + `backdrop-filter: blur(8px)` |
| `GearIndicator` | N · 1–6 · PIT · — | Mono 600, amber glow when shifting |
| `ProgressBar` | — | 2 px, amber on `line` |
| `StageSection` | — | Spine node + header + hook + `MetricBeforeAfter` + `CaseExpander` + chips |
| `MetricBeforeAfter` | default · compact (Recruiter) | before (`before` color, struck-through label) → after (`after`, display size) + Δ chip |
| `CaseExpander` | — | `<details>` baseline, enhanced by Blazor |
| `Chip` | tech · Δ · year | `raised` bg, `cyan` text, radius `sm` |
| `Tachometer` | — | SVG arc 240°, needle, redline zone in `before` color |
| `DynoLane` | legacy · native | Tachometer + timer + skeleton booking UI |
| `ServiceCard` | — | Odometer year, shop, role, highlight, notes |
| `Toolbox` | tabs · accordion | Drawer + `Chip` popovers |
| `GarageCard` | photo · illustration | Featured photo (focal-point crop) or line art + story + "link to work" |
| `Filmstrip` | — | 4 thumbnails + "See all N photos" |
| `GalleryGrid` | — | Category filter chips + dense grid |
| `Lightbox` | — | Native `<dialog>`, caption, credit, counter, prev/next |
| `Avatar` | 64 (round) · 112 (radius `lg`) | Portrait; `alt` from the manifest |
| `Toast` | achievement · info | Bottom-left, polite live region, 4 s |
| `AchievementDrawer` | — | 12 slots, 3 × 4 grid |
| `Terminal` | — | `raised` bg, mono, `role="log"` output |
| `LanguageSwitch` | segmented · menu | PT · EN · ES |
| `LanguageBanner` | — | Suggests the visitor's browser locale |
