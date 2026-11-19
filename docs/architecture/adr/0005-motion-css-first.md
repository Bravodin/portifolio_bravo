# 0005 — Motion: CSS scroll-driven animations first, Blazor for state only

- Status: Accepted
- Date: 2026-11-19

## Context

The "ride" (packet travel, stage reveals, gear changes, progress) must feel smooth from the first paint, before the WASM runtime is ready, and must respect `prefers-reduced-motion`.

## Decision

- **L1 motion = CSS.** `animation-timeline: scroll()` / `view()` for the packet, reveals and progress. Supported browsers get it with zero JS.
- **Fallback = `motion.js` (≤ 5 KB)** using `IntersectionObserver` for browsers without scroll-driven animations. It also emits `stage-entered` `CustomEvent`s.
- **Blazor never animates per frame.** Islands only react to events (achievements, Dyno state) and toggle classes. Per-frame JS ↔ .NET interop is forbidden.
- **Reduced motion:** the media query plus a user toggle set `html.reduce-motion`, which removes the timelines and sets the durations to 0.

## Alternatives considered

- GSAP / ScrollTrigger: powerful, but ~50 KB and a licensing question for commercial use. Not needed.
- Three.js / WebGL scenes: impressive, but heavy and hard to make accessible. Parked for v2.
- Animating from C# via interop: chatty, janky, and it ties the motion to the WASM boot.

## Consequences

- ✅ Motion works before (and without) WASM
- ✅ Tiny JS footprint
- ⚠️ Two code paths (CSS and the IO fallback) to test. Playwright covers both by toggling feature detection
