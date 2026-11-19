# User flows

All flows are in Mermaid, so GitHub renders them inline.

## F0 — Entry and routing by intent

```mermaid
flowchart TD
    A([Visitor arrives]) --> B{Entry point}
    B -- "LinkedIn / CV link / Google" --> C["/ (locale from path)"]
    B -- "Direct /en/recruiter link" --> R["Recruiter Mode"]
    C --> D["Prerendered hero is visible<br/>(LCP < 2 s, no JS needed)"]
    D --> L{"Browser language ≠ page locale?"}
    L -- yes --> LB["Show language suggestion banner<br/>(dismissible, remembered)"]
    L -- no --> E
    LB --> E{Intent}
    E -- "Short on time" --> R
    E -- "Curious" --> F["Start the ride → F2"]
    E -- "Just scrolls" --> F
    E -- "Technical reviewer" --> G["Footer → Colophon / GitHub / Terminal → F4"]
    R --> X([Conversion: LinkedIn · PDF])
    F --> X
    G --> X
```

## F1 — Recruiter Mode (P1, goal ≤ 30 s)

```mermaid
flowchart LR
    A["Tap 'Recruiter mode (30 s)'<br/>in the HUD or hero"] --> B["/recruiter<br/>Single screen:<br/>role · years · stack · languages<br/>location · 6 key metrics"]
    B --> C{Next}
    C -- "Download CV" --> D[("PDF in the page locale")]
    C -- "LinkedIn" --> E(["linkedin.com/in/manuelbravoc"])
    C -- "Print" --> G["Print stylesheet (1 page A4)"]
    C -- "Take the ride instead" --> H["/ #start"]
```

**Acceptance:** works without WASM loaded · zero animation · fits a 360 × 740 viewport with the CTAs above the fold · one A4 page when printed.

## F2 — The ride (P2, P4)

```mermaid
sequenceDiagram
    autonumber
    actor V as Visitor
    participant P as Page (prerendered HTML)
    participant W as Blazor WASM runtime
    participant H as HUD

    V->>P: Lands on /
    P-->>V: Hero readable immediately
    P->>W: Start downloading runtime (background, preloaded)
    W-->>H: Load progress → fuel gauge on "Start the ride"
    V->>H: Clicks "Start the ride" (or just scrolls)
    alt WASM ready
        H-->>V: Gear N → 1, packet leaves the phone
    else WASM still loading
        H-->>V: Smooth-scroll to #client, gauge keeps filling<br/>(scroll animations are CSS-only, so they already work)
    end
    loop For each stage 1…6, PIT
        V->>P: Scrolls
        P->>H: Stage ≥ 60 % in view
        H-->>V: Gear shifts · progress updates · achievement toast
        opt Visitor expands the case
            V->>P: "Open case"
            P-->>V: Problem · Action · Result · Role
        end
    end
    V->>P: Reaches Pit Stop
    P-->>V: Contact CTAs + "Full Trace" achievement
```

## F3 — Dyno demo (stage 1)

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Running: "Run the dyno"
    Running --> NativeDone: t ≈ 1.2 s
    NativeDone --> BothDone: t = random(12…20) s
    NativeDone --> Skipped: "Skip — you get the idea" (visible after 3 s)
    Skipped --> BothDone
    BothDone --> Idle: "Run again"
    BothDone --> [*]

    note right of Running
        Two lanes in parallel:
        Legacy (Ionic, 2016) and Native (Swift/Kotlin).
        A tachometer per lane; the elapsed time counts up.
    end note
    note right of BothDone
        Result card: computed ratio, e.g. "~12× faster".
        Unlocks the "Latency Slayer" achievement.
        Disclaimer: simulation of reported values.
    end note
```

**Reduced motion:** the lanes don't animate; the result card is shown with the static timings and a "Replay with motion" option.

## F4 — Terminal easter egg (P3, P4)

```mermaid
stateDiagram-v2
    [*] --> Closed
    Closed --> Open: press ` or ~ · footer link "open terminal"
    Open --> Open: command → output
    Open --> Closed: Esc · exit · click outside
    Open --> Navigating: trace · cv · contact · recruiter
    Navigating --> Closed: navigate to section / route
```

| Command | Output |
|---|---|
| `help` | Lists the commands |
| `whoami` | Name, role, location, languages |
| `trace` | Closes the terminal and scrolls to the stage the visitor reached last |
| `git log --oneline` | Service Log as fake commits (`a1b2c3 Porto: Lead Engineer`) |
| `skills [drawer]` | Toolbox as a tree |
| `dotnet --info` | .NET version, build hash and date, Lighthouse score from the last CI run |
| `achievements` | Unlocked / locked list |
| `lang pt\|en\|es` | Switches the locale |
| `cv` | Downloads the PDF |
| `contact` | LinkedIn URL + "open" hint (the only contact channel) |
| `sudo hire manuel` | `[sudo] password for recruiter: ******` → `Permission granted. Opening LinkedIn…` + achievement |
| `clear`, `exit` | — |
| *unknown* | `command not found — try 'help'` |

## F5 — Language switch

```mermaid
flowchart LR
    A["Language menu / terminal 'lang en'"] --> B["Navigate to the same anchor<br/>in the target locale route"]
    B --> C["Persist the choice (localStorage, try/catch)"]
    C --> D["Unlock 'Polyglot' on the 2nd locale"]
```

## F6 — Achievements

```mermaid
flowchart LR
    E["Event (stage seen, dyno run, command…)"] --> S["AchievementService<br/>(pure C#, unit-tested)"]
    S -->|new| T["Toast (polite live region, 4 s)"]
    S --> P["Persist set in localStorage<br/>(try/catch; in-memory fallback)"]
    S --> D["Drawer: 12 slots, locked ones show a hint"]
```

| ID | Name | Trigger |
|---|---|---|
| `ignition` | First Ignition | Pressed "Start the ride" or reached stage 1 |
| `latency-slayer` | Latency Slayer | Finished a Dyno run |
| `immobilizer` | Immobilizer | Saw stage 2 |
| `engine-rebuild` | Engine Rebuild | Opened the stage 4 case |
| `no-deadlocks` | No Deadlocks | Saw stage 6 |
| `pit-crew` | Pit Crew | Reached PIT |
| `full-trace` | Full Trace | Saw all 7 stages |
| `speedrunner` | Speedrunner | Opened Recruiter Mode within 10 s of landing |
| `polyglot` | Polyglot | Used 2 locales |
| `root-access` | Root Access | Opened the terminal |
| `hired` | sudo hire | Ran `sudo hire manuel` |
| `wheelie` | Wheelie | Konami code (↑↑↓↓←→←→BA): the HUD bike does a wheelie |
