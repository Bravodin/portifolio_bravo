import sys, os
sys.path.insert(0, os.path.dirname(__file__))
from wf_lib import Svg, C, DISPLAY, MONO, BODY

OUT = sys.argv[1]
W, H = 1440, 900
SP = 96  # spine x
STAGES = ["1", "2", "3", "4", "5", "6", "PIT"]


def save(svg, name):
    with open(os.path.join(OUT, name), "w") as f:
        f.write(svg.render())


def base(title, notes_h=170, h=H):
    s = Svg(W, h + notes_h, title)
    s.rect(0, 0, W, h, fill=C["bg"])
    return s


def stage_spine(s, active, packet_y, y1=90, y2=H):
    ys = [140 + i * 100 for i in range(7)]
    s.spine(SP, y1, y2, list(zip(ys, STAGES)), active=active, packet_at=packet_y)


# ---------------------------------------------------------------- 01 hero
def hero():
    s = base("01 — Landing / Ignition (desktop 1440)")
    s.hud(W, gear="N", progress=0)
    s.line(SP, 120, SP, H, C["line"], sw=2)
    s.circle(SP, 200, 9, fill=C["bg"], stroke=C["amber"], sw=2)
    s.text(SP - 20, 204, "N", size=11, fill=C["amber"], family=MONO, anchor="end", weight=600)
    s.label(200, 190, "Lead Software Engineer · Brazil · Open to remote")
    s.text(196, 290, "Manuel Bravo", size=96, weight=700, family=DISPLAY)
    s.text(200, 360, "I modernize critical platforms", size=44, weight=600, family=DISPLAY, fill=C["text"])
    s.text(200, 410, "without stopping production.", size=44, weight=600, family=DISPLAY, fill=C["amber"])
    s.lines(200, 460, ["Full-stack .NET · Test automation & quality · AWS · Team building.",
                       "20+ years in insurance, health and financial services."], size=19, fill=C["text2"])
    bw = s.button(200, 540, "Start the ride  ▸", fill_pct=0.62, w=240, h=56, size=18)
    s.text(200 + 120, 616, "loading engine… 62 %", size=12, fill=C["muted"], family=MONO, anchor="middle")
    s.button(460, 540, "Recruiter mode (30 s)", kind="secondary", w=250, h=56, size=17)
    s.text(730, 574, "Download CV", size=16, fill=C["cyan"], weight=600)
    s.line(730, 580, 830, 580, C["cyan"])
    # quick stats
    stats = [("12–20 s → 1.2 s", "booking journey"), ("−70 %", "security vulnerabilities"),
             ("−30 %", "API platform cost"), ("90 %+", "coverage gate")]
    for i, (v, l) in enumerate(stats):
        x = 200 + i * 190
        s.line(x, 680, x, 740, C["line"], sw=2)
        s.text(x + 16, 710, v, size=26 if len(v) < 8 else 20, fill=C["after"], weight=700, family=DISPLAY)
        s.text(x + 16, 734, l, size=12, fill=C["muted"], family=MONO)
    # phone schematic
    px, py = 1060, 170
    s.rect(px, py, 230, 450, fill=C["surface"], stroke=C["strong"], r=34, sw=2)
    s.rect(px + 85, py + 14, 60, 8, fill=C["line"], r=4)
    s.rect(px + 20, py + 50, 190, 28, fill=C["raised"], r=4)
    s.text(px + 32, py + 69, "Book appointment", size=13, fill=C["text2"])
    for i in range(4):
        s.rect(px + 20, py + 96 + i * 62, 190, 50, fill=C["raised"], r=6)
        s.rect(px + 32, py + 108 + i * 62, 90, 9, fill=C["line"], r=3)
        s.rect(px + 32, py + 124 + i * 62, 140, 7, fill=C["line"], r=3)
    s.rect(px + 20, py + 360, 190, 44, fill=C["amber"], r=6)
    s.text(px + 115, py + 388, "Confirm", size=15, fill=C["bg"], weight=600, anchor="middle")
    s.path(f"M{px+115},{py+450} C{px+115},{820} {SP+500},{790} {SP},{H-30}",
           stroke=C["amber"], sw=1.5, dash="4 8")
    s.circle(px + 115, py + 500, 14, fill=C["amber"], filt="glow", op=0.8)
    s.circle(px + 115, py + 500, 7, fill=C["amber"])
    s.text(px + 140, py + 505, "request packet", size=12, fill=C["amber"], family=MONO)
    s.text(W / 2, 860, "scroll to ride  ↓", size=13, fill=C["muted"], family=MONO, anchor="middle")
    # markers
    s.marker(1, 380, 32); s.marker(2, 452, 548); s.marker(3, 182, 190)
    s.marker(4, SP + 28, 200); s.marker(5, 722, 548); s.marker(6, 186, 690)
    s.notes(H, [
        "HUD always visible. Gear N = parked in the hero. Thin progress bar under it.",
        "Fuel-gauge fill = real WASM runtime download progress. Clickable at any %.",
        "Everything in the hero is prerendered HTML: readable with JS off; this is the LCP.",
        "Spine starts here; the packet travels down it as you scroll (CSS scroll-timeline).",
        "Recruiter mode is reachable in the first viewport (and in the HUD).",
        "Quick stats: before→after teasers that anchor credibility before any scroll.",
    ])
    save(s, "01-landing-desktop.svg")


# ---------------------------------------------------------------- 02 stage 1 + dyno
def stage1():
    s = base("02 — Trace stage 1 · Client + Dyno (desktop)")
    s.hud(W, gear="1", progress=0.10)
    stage_spine(s, 0, 140)
    x = 200
    s.label(x, 140, "Gear 1 · Client  —  handlebars & dashboard")
    s.text(x, 200, "The app that made people wait", size=42, weight=700, family=DISPLAY)
    s.lines(x, 244, ["A 2016 Ionic app. Booking a medical appointment",
                     "took 12 to 20 seconds, on the most critical journey."], size=18)
    s.metric(x, 370, "12–20 s", "Ionic app, 2016", "~1.2 s", "native iOS + Android", "", big=104)
    s.chip(x, 430, "~10–16× faster", color=C["after"])
    s.chips(x, 480, ["Swift", "Kotlin", "Compose Testing", "XCUITest", "Selenium → native"], maxw=520)
    s.rect(x, 560, 520, 52, fill=C["surface"], stroke=C["line"], r=6)
    s.text(x + 20, 592, "▸  Open case: problem · action · result · my role", size=15, fill=C["text"])
    s.lines(x, 660, ["Also on this layer: WPF POS apps at Linx, +15 % responsiveness."], size=14, fill=C["muted"])
    # dyno
    dx, dy, dw, dh = 780, 120, 610, 700
    s.rect(dx, dy, dw, dh, fill=C["raised"], stroke=C["line"], r=12)
    s.label(dx + 28, dy + 40, "Dyno · Book an appointment", fill=C["amber"])
    s.text(dx + dw - 28, dy + 40, "simulation", size=12, fill=C["muted"], family=MONO, anchor="end")
    for i, (name, sub, frac, col, t, status) in enumerate([
            ("LEGACY", "Ionic · 2016", 0.86, C["before"], "07.4 s", "loading…"),
            ("NATIVE", "Swift / Kotlin", 0.22, C["after"], "01.2 s", "✓ Booked")]):
        lx = dx + 28 + i * 290
        s.rect(lx, dy + 70, 266, 470, fill=C["surface"], stroke=C["line"], r=8)
        s.text(lx + 20, dy + 104, name, size=16, weight=700, family=MONO, fill=col)
        s.text(lx + 20, dy + 124, sub, size=12, fill=C["muted"], family=MONO)
        s.tach(lx + 133, dy + 240, 80, frac, col, label="load")
        s.text(lx + 133, dy + 360, t, size=44, weight=700, family=MONO, fill=C["text"], anchor="middle")
        s.text(lx + 133, dy + 390, status, size=15, fill=col, anchor="middle", weight=600)
        for k in range(3):
            s.rect(lx + 24, dy + 420 + k * 34, 218, 24, fill=C["raised"] if i == 0 else C["line"], r=4,
                   stroke=C["line"] if i == 0 else None)
    s.button(dx + 28, dy + 570, "Run again", w=180)
    s.text(dx + 232, dy + 600, "Skip — you get the idea", size=15, fill=C["cyan"])
    s.lines(dx + 28, dy + 660, ["Based on measured production timings. No real systems are called."],
            size=12, fill=C["muted"], family=MONO)
    s.marker(1, SP + 30, 140); s.marker(2, 760, 140); s.marker(3, dx + 300, dy + 280)
    s.marker(4, dx + 210, dy + 594); s.marker(5, x + 540, 586); s.marker(6, x - 18, 370)
    s.notes(H, [
        "Active node + packet glow; gear shows 1 once ≥ 60 % of the stage is in view.",
        "Dyno is an L2 (Blazor) island. Without WASM it shows the static result card.",
        "Tachometers: native finishes ~1.2 s; legacy random 12–20 s; aria-live announces both.",
        "'Skip' appears after 3 s. Finishing unlocks the 'Latency Slayer' achievement.",
        "Case expander is a native <details> enhanced by Blazor. One open at a time on mobile.",
        "Before → after metric: the after value counts down from 20 s on reveal (900 ms).",
    ])
    save(s, "02-stage-client-dyno-desktop.svg")


# ---------------------------------------------------------------- 03 stage 4 + case
def stage4():
    s = base("03 — Trace stage 4 · Services, case open + achievement toast (desktop)")
    s.hud(W, gear="4", progress=0.52)
    stage_spine(s, 3, 440)
    x = 200
    s.label(x, 130, "Gear 4 · Services  —  engine block rebuild")
    s.text(x, 188, "Rebuilding the engine while it runs", size=42, weight=700, family=DISPLAY)
    s.metric(x, 300, "legacy", ".NET / SOAP services", "−70 %", "security vulnerabilities", "~80 % migrated", big=104)
    s.chips(x, 360, ["Node.js", "TypeScript", "C#", ".NET", "SOAP → REST", "Microservices"], maxw=560)
    # case open
    cy = 430
    s.rect(x, cy, 560, 380, fill=C["surface"], stroke=C["strong"], r=8)
    s.text(x + 20, cy + 34, "▾  Case: Porto Saúde service modernization", size=16, weight=600)
    blocks = [("Problem", ["Legacy .NET/SOAP services with a wide", "security-vulnerability surface."]),
              ("Action", ["Modernized the services to Node.js across", "Porto Saúde's digital platforms."]),
              ("Result", ["~80 % of legacy services migrated,", "vulnerabilities reduced ~70 %."]),
              ("My role", ["Lead Engineer: led the modernization", "(final copy written by Manuel)."])]
    for i, (h, ls) in enumerate(blocks):
        bx = x + 20
        by = cy + 72 + i * 76
        s.label(bx, by, h, fill=C["amber"] if h == "Result" else C["cyan"])
        s.lines(bx + 100, by, ls, size=14, fill=C["text2"])
    # schematic: exploded engine
    ex, ey = 870, 160
    s.label(ex, ey, "exploded view", fill=C["muted"])
    for i in range(5):
        yy = ey + 40 + i * 92
        s.rect(ex, yy, 180, 64, fill=C["surface"], stroke=C["strong"], r=4, dash="5 4")
        s.text(ex + 90, yy + 38, f"SOAP svc {i+1}", size=14, fill=C["muted"], family=MONO, anchor="middle")
        if i < 4:
            s.path(f"M{ex+186},{yy+32} L{ex+300},{yy+32}", stroke=C["amber"], sw=1.5)
            s.path(f"M{ex+292},{yy+26} L{ex+302},{yy+32} L{ex+292},{yy+38}", stroke=C["amber"], sw=1.5)
            s.rect(ex + 310, yy, 200, 64, fill=C["raised"], stroke=C["after"], r=4)
            s.text(ex + 410, yy + 38, f"node-svc-{i+1}", size=14, fill=C["after"], family=MONO, anchor="middle")
        else:
            s.text(ex + 410, yy + 38, "in progress…", size=14, fill=C["muted"], family=MONO, anchor="middle")
    # toast
    tx, ty = 200, 830
    s.rect(tx, ty - 6, 360, 60, fill=C["raised"], stroke=C["amber"], r=8)
    s.text(tx + 26, ty + 31, "★", size=26, fill=C["amber"])
    s.text(tx + 62, ty + 16, "ACHIEVEMENT UNLOCKED", size=11, fill=C["amber"], family=MONO, weight=600)
    s.text(tx + 62, ty + 38, "Engine Rebuild", size=17, weight=600)
    s.marker(1, x + 580, 450); s.marker(2, ex - 20, 150); s.marker(3, tx + 380, ty + 20)
    s.notes(H, [
        "Case open: 4 short blocks, always Problem · Action · Result · My role (≤ 2 lines each).",
        "Schematic line-art per stage (blueprint style), decorative: aria-hidden.",
        "Achievement toast: polite live region, 4 s, bottom-left, never blocks content.",
    ], cols=1)
    save(s, "03-stage-services-case-desktop.svg")


if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    hero(); stage1(); stage4()


# ---------------------------------------------------------------- 04 service log
LOG = [(2008, 2009, "Nunes Imóveis", "Software Engineer", "Batch 8 h → 2 h"),
       (2009, 2013, "DIMEP", "Software Engineer", "Device cost −73 %"),
       (2013, 2016, "COREON", "Software Engineer", "+20 % performance"),
       (2016, 2017, "Linx", "Enterprise SW Engineer", "+15 % POS responsiveness"),
       (2017, 2018, "Indra", "Senior SW Engineer", "SLA 70 % → 98 %"),
       (2018, 2019, "Indra", "Lead SW Engineer", "Overtime −75 %"),
       (2019, 2022, "Indra", "Project Manager", "+30 % delivery speed"),
       (2022, 2022, "Digisystem", "Operation Manager", "BR + LATAM pre-sales"),
       (2022, 2026, "Porto", "Lead Engineer", "Trace stages 1–7")]


def service_log():
    s = base("04 — Service Log · career timeline (desktop)")
    s.hud(W, gear="—", progress=1.0)
    s.line(SP, 90, SP, H, C["line"], sw=2)
    x = 200
    s.label(x, 130, "Service log  —  the bike's revision history")
    s.text(x, 188, "Service history since 2008", size=48, weight=700, family=DISPLAY)
    # odometer ruler
    rx0, rx1, ry = x, 1360, 270
    s.rect(rx0 - 10, ry - 34, rx1 - rx0 + 20, 92, fill=C["surface"], stroke=C["line"], r=8)
    yr = lambda y: rx0 + 20 + (y - 2008) / 18 * (rx1 - rx0 - 40)
    for y in range(2008, 2027):
        X = yr(y)
        major = y % 2 == 0
        s.line(X, ry, X, ry + (16 if major else 8), C["strong"], sw=1.5)
        if major:
            s.text(X, ry + 36, str(y), size=12, fill=C["muted"], family=MONO, anchor="middle")
    sel = (2017, 2022)
    s.rect(yr(sel[0]), ry - 22, yr(sel[1]) - yr(sel[0]), 14, fill=C["amber"], r=2, op=0.85)
    for a, b, *_ in LOG:
        s.circle(yr(a), ry - 15, 4, fill=C["cyan"])
    s.label(rx1, ry - 44, "odometer · drag or click a year", fill=C["muted"], size=11, anchor="end")
    # cards
    shown = [LOG[4], LOG[5], LOG[6]]
    for i, (a, b, shop, role, hl) in enumerate(shown):
        cx = x + i * 390
        cy = 380
        s.rect(cx, cy, 370, 300, fill=C["surface"], stroke=C["amber"] if i == 1 else C["line"], r=12)
        s.rect(cx + 20, cy + 22, 120, 30, fill=C["raised"], r=2)
        s.text(cx + 80, cy + 43, f"{a} → {b}", size=14, fill=C["amber"], family=MONO, anchor="middle", weight=600)
        s.text(cx + 20, cy + 98, shop, size=32, weight=700, family=DISPLAY)
        s.text(cx + 20, cy + 126, role, size=16, fill=C["text2"])
        s.line(cx + 20, cy + 150, cx + 350, cy + 150, C["line"])
        s.text(cx + 20, cy + 196, hl, size=26, weight=700, family=DISPLAY, fill=C["after"])
        s.text(cx + 20, cy + 268, "▸ Service notes", size=15, fill=C["cyan"])
    s.text(x, 740, "‹  newer", size=15, fill=C["muted"], family=MONO)
    s.text(x + 1150, 740, "older  ›", size=15, fill=C["muted"], family=MONO, anchor="end")
    s.label(x, 800, "Education")
    s.lines(x, 830, ["MBA in AI Engineering · UNIPDS · 2026–27 (in progress)   ·   MBA in Project Management · Impacta · 2020–21   ·   Systems Analysis & Dev. · FIEO · 2010–13"], size=14)
    s.marker(1, rx1 + 30, ry); s.marker(2, x + 390 + 380, 400); s.marker(3, x + 150, 648); s.marker(4, x - 22, 800)
    s.notes(H, [
        "Odometer ruler 2008 → 2026. Selecting a range filters/scrolls the cards (keyboard: ←/→ by year).",
        "Revision card: years, shop, role, one highlight metric. Current selection outlined amber.",
        "'Service notes' expands the full bullet list from the CV (same content model as Recruiter Mode).",
        "Education sits under the log as a single strip, not as cards.",
    ], cols=1)
    save(s, "04-service-log-desktop.svg")


# ---------------------------------------------------------------- 05 toolbox / garage / pit stop
def lower_page():
    h = 1640
    s = base("05 — Toolbox · Garage · Pit Stop · Footer (desktop, scroll frame)", notes_h=260, h=h)
    s.hud(W, gear="—", progress=1.0)
    s.line(SP, 90, SP, h, C["line"], sw=2)
    x = 200
    # toolbox
    s.label(x, 130, "Toolbox")
    s.text(x, 186, "The tools in the drawer", size=44, weight=700, family=DISPLAY)
    tabs = ["Engine", "Diagnostics", "Electronics", "Scanner", "Fluids", "Power tools", "Workshop"]
    tx = x
    for i, t in enumerate(tabs):
        w = len(t) * 9 + 36
        on = i == 1
        s.rect(tx, 220, w, 40, fill=C["raised"] if on else "none", stroke=C["amber"] if on else C["line"], r=6)
        s.text(tx + w / 2, 245, t, size=15, fill=C["text"] if on else C["text2"], anchor="middle", weight=600 if on else 400)
        tx += w + 8
    s.rect(x, 276, 1160, 190, fill=C["surface"], stroke=C["line"], r=12)
    s.label(x + 24, 310, "Diagnostics · testing & quality", fill=C["muted"])
    s.chips(x + 24, 330, ["Selenium", "JMeter", "Postman", "BDD / TDD", "Given-When-Then", "Sonar 90 % gate",
                          "Jetpack Compose Testing", "XCUITest"], maxw=740, size=14)
    px, py = x + 790, 300
    s.rect(px, py, 350, 150, fill=C["raised"], stroke=C["cyan"], r=8)
    s.text(px + 20, py + 34, "JMeter", size=20, weight=700, family=DISPLAY, fill=C["cyan"])
    s.text(px + 20, py + 62, "~8 years · Indra, Porto", size=14, fill=C["text2"], family=MONO)
    s.lines(px + 20, py + 92, ["Load & stress tests before go-live, designed", "with the Quality CoE to mirror real usage."], size=14)
    # garage
    gy = 540
    s.label(x, gy, "Garage")
    s.text(x, gy + 56, "Off the clock", size=44, weight=700, family=DISPLAY)
    cards = [("Motorcycles", "Track days: the feel of a well-tuned machine.", "→ why 1.2 s matters", "photo"),
             ("Mechanics", "Take it apart, understand it, improve it.", "→ legacy modernization", "wrench"),
             ("Games", "Progression and feedback loops.", "→ why this site has gears", "pad")]
    for i, (t, d, l, kind) in enumerate(cards):
        cx = x + i * 390
        s.rect(cx, gy + 90, 370, 420, fill=C["surface"], stroke=C["line"], r=12)
        if kind == "photo":
            s.photo(cx + 16, gy + 106, 338, 230, "track-03", r=8)
            s.rect(cx + 16, gy + 296, 338, 40, fill=C["bg"], op=0.55)
            s.text(cx + 28, gy + 321, "Performance is knowing exactly where the limit is.", size=12, fill=C["text"])
        else:
            s.blueprint(cx + 16, gy + 106, 338, 230, kind)
        s.text(cx + 20, gy + 380, t, size=28, weight=700, family=DISPLAY)
        s.text(cx + 20, gy + 410, d, size=15, fill=C["text2"])
        s.text(cx + 20, gy + 470, l, size=14, fill=C["amber"], family=MONO)
    # filmstrip
    fy0 = gy + 540
    s.photo(x, fy0, 180, 120, "track-01", r=6)
    s.photo(x + 196, fy0, 180, 120, "track-02", r=6)
    s.slot(x + 392, fy0, 180, 120, "next photo", r=6)
    s.slot(x + 588, fy0, 180, 120, "next photo", r=6)
    s.text(x + 800, fy0 + 66, "See all 3 photos  →", size=17, fill=C["cyan"], weight=600)
    s.text(x + 800, fy0 + 90, "/garage · track · workbench · games", size=12, fill=C["muted"], family=MONO)
    # pit stop
    py2 = 1250
    s.rect(0, py2 - 30, W, 290, fill=C["surface"])
    s.line(0, py2 - 30, W, py2 - 30, C["line"])
    s.photo(x, py2 - 2, 64, 64, "profile", round_=True)
    s.label(x + 84, py2 + 22, "Pit stop", fill=C["amber"])
    s.text(x + 84, py2 + 48, "Manuel Bravo · Lead Software Engineer", size=14, fill=C["muted"], family=MONO)
    s.text(x, py2 + 120, "Let's talk about your next modernization.", size=44, weight=700, family=DISPLAY)
    s.button(x, py2 + 150, "Talk on LinkedIn  ↗", w=240, h=56, size=17)
    s.button(x + 260, py2 + 150, "Download CV (PDF)", kind="secondary", w=220, h=56, size=17)
    s.text(x + 510, py2 + 185, "linkedin.com/in/manuelbravoc", size=15, fill=C["muted"], family=MONO)
    s.text(x, py2 + 240, "★ Full Trace: 7/7 stages seen  ·  achievements 6/12  ▸", size=14, fill=C["muted"], family=MONO)
    # footer
    fy = 1540
    s.line(0, fy, W, fy, C["line"])
    s.text(x, fy + 46, "© 2026 Manuel Bravo  ·  How this site was built  ·  Source on GitHub  ·  Press ` to open the terminal",
           size=14, fill=C["text2"])
    s.text(W - 40, fy + 46, ".NET 10 · build a1b2c3d · Lighthouse 98/100/100/100", size=12, fill=C["muted"], family=MONO, anchor="end")
    s.marker(1, x - 22, 240); s.marker(2, px + 370, py + 20); s.marker(3, x + 1180, gy + 120)
    s.marker(4, x + 1000, fy0 + 60); s.marker(5, x + 790, py2 + 179); s.marker(6, x + 640, fy + 20)
    s.notes(h, [
        "Toolbox: drawers as tabs (desktop) / accordions (mobile). No skill bars.",
        "Chip popover: years, where, one line of context. Opens on click/focus, Esc closes.",
        "Garage cards: featured photo cropped at its focal point; categories without a photo use line art.",
        "Filmstrip: next thumbnails + 'See all N photos' → /garage. Dashed slots = room for photos added later.",
        "Pit Stop: portrait avatar + LinkedIn (the only contact channel) + PDF. No e-mail, no form, no backend.",
        "Footer: colophon, source, terminal hint, and build hash + last CI Lighthouse score.",
    ], cols=1)
    save(s, "05-toolbox-garage-pitstop-desktop.svg")


# ---------------------------------------------------------------- 06 recruiter mode
def recruiter():
    s = base("06 — Recruiter Mode (desktop, also prints to 1 × A4)")
    s.rect(0, 0, W, 64, fill=C["surface"])
    s.line(0, 64, W, 64, C["line"])
    s.text(32, 38, "‹  Take the ride instead", size=15, fill=C["cyan"])
    s.label(W / 2, 37, "Recruiter mode · 30 s read", fill=C["amber"], anchor="middle")
    s.text(W - 32, 38, "PT  EN  ES   ⎙ Print", size=14, fill=C["text2"], family=MONO, anchor="end")
    x = 120
    # identity card
    s.rect(x, 100, 440, 740, fill=C["surface"], stroke=C["line"], r=12)
    s.photo(x + 32, 124, 104, 104, "profile", r=12)
    s.text(x + 156, 172, "Manuel Bravo", size=40, weight=700, family=DISPLAY)
    s.text(x + 156, 204, "Lead Software Engineer", size=18, fill=C["amber"], weight=600)
    rows = [("Now", "Porto · Lead Engineer · since 2022"), ("Experience", "20+ years"),
            ("Sectors", "Insurance · Health · Financial"), ("Location", "Brazil · open to remote"),
            ("Languages", "PT native · ES native · EN full prof."), ("Education", "MBA AI Eng. (2026–27) · MBA PM")]
    for i, (k, v) in enumerate(rows):
        yy = 260 + i * 56
        s.label(x + 32, yy, k, fill=C["muted"], size=11)
        s.text(x + 32, yy + 24, v, size=16)
    s.button(x + 32, 610, "Download CV (PDF)", w=376, h=52)
    s.button(x + 32, 674, "Talk on LinkedIn  ↗", kind="secondary", w=376, h=52)
    s.text(x + 32, 760, "Contact via LinkedIn only", size=14, fill=C["muted"], family=MONO)
    # metrics grid
    mx = 600
    s.label(mx, 120, "Key results (measured)")
    ms = [("12–20 s", "~1.2 s", "booking journey"), ("baseline", "−70 %", "vulnerabilities"),
          ("CA GW", "−30 %", "API platform cost"), ("baseline", "−40 %", "reimbursement fraud"),
          ("70 %", "98 %", "SLA compliance"), ("baseline", "+40 %", "dev productivity (AI)")]
    for i, (b, a, l) in enumerate(ms):
        cx = mx + (i % 3) * 245
        cy = 140 + (i // 3) * 150
        s.rect(cx, cy, 230, 135, fill=C["surface"], stroke=C["line"], r=8)
        s.text(cx + 18, cy + 34, b, size=15, fill=C["before"], family=MONO, deco="line-through")
        s.text(cx + 18, cy + 86, a, size=46, fill=C["after"], weight=700, family=DISPLAY)
        s.text(cx + 18, cy + 116, l, size=13, fill=C["muted"], family=MONO)
    s.label(mx, 470, "Core stack")
    s.chips(mx, 488, ["C#", ".NET", "ASP.NET", "Node.js", "TypeScript", "Angular", "AWS", "Redis",
                      "SQL Server", "Oracle", "Selenium", "JMeter", "Postman", "Sonar", "Copilot / Claude"], maxw=720, size=14)
    s.label(mx, 620, "Last 3 roles")
    for i, (a, b, shop, role, hl) in enumerate([LOG[8], LOG[7], LOG[6]]):
        yy = 656 + i * 50
        s.text(mx, yy, f"{a}–{b if b != a else ''}".rstrip("–"), size=14, fill=C["amber"], family=MONO)
        s.text(mx + 110, yy, f"{shop} · {role}", size=16, weight=600)
        s.text(mx + 480, yy, hl, size=14, fill=C["text2"], family=MONO)
    s.marker(1, x + 420, 120); s.marker(2, mx + 750, 140); s.marker(3, W - 220, 38); s.marker(4, 260, 20)
    s.notes(H, [
        "Portrait + everything a recruiter screens for, above the fold at 1440 × 900 and 360 × 740. Zero animation.",
        "Six headline metrics, compact before → after. Same content source as the Trace.",
        "Print stylesheet: light background, 1 × A4, URLs printed after links.",
        "Exit back to the ride keeps the language. Opening within 10 s unlocks 'Speedrunner'.",
    ], cols=1)
    save(s, "06-recruiter-mode-desktop.svg")


# ---------------------------------------------------------------- 07 terminal
def terminal():
    s = base("07 — Terminal easter egg overlay (desktop)")
    s.hud(W, gear="3", progress=0.4)
    stage_spine(s, 2, 340)
    s.label(200, 130, "Gear 3 · API Gateway  —  fuel injection")
    s.text(200, 188, "Same fuel, 30 % cheaper", size=42, weight=700, family=DISPLAY)
    s.rect(0, 64, W, H - 64, fill="#000", op=0.55)
    ty = 470
    s.rect(40, ty, W - 80, H - ty - 20, fill=C["raised"], stroke=C["strong"], r=12)
    s.rect(40, ty, W - 80, 36, fill=C["surface"], r=12)
    for i, col in enumerate([C["before"], C["amber"], C["after"]]):
        s.circle(66 + i * 22, ty + 18, 6, fill=col)
    s.text(W / 2, ty + 23, "guest@manuelbravo.com.br: ~/trace", size=13, fill=C["muted"], family=MONO, anchor="middle")
    s.text(W - 60, ty + 23, "Esc to close", size=12, fill=C["muted"], family=MONO, anchor="end")
    L = [("$ ", "whoami", C["text"]),
         ("", "Manuel Bravo — Lead Software Engineer @ Porto · Brazil · PT/ES/EN", C["text2"]),
         ("$ ", "git log --oneline -3", C["text"]),
         ("", "a91f2c0  (HEAD -> career) Porto: Lead Engineer — modernized Porto Saúde", C["text2"]),
         ("", "7c3e1b4  Digisystem: Operation Manager — BR + LATAM pre-sales", C["text2"]),
         ("", "52d0a9e  Indra: Project Manager — Waterfall → Scrum, +30 % delivery", C["text2"]),
         ("$ ", "sudo hire manuel", C["text"]),
         ("", "[sudo] password for recruiter: ********", C["muted"]),
         ("", "Permission granted. Opening LinkedIn…   ★ achievement unlocked: sudo hire", C["after"])]
    for i, (p, t, col) in enumerate(L):
        yy = ty + 72 + i * 28
        if p:
            s.text(70, yy, p, size=16, fill=C["amber"], family=MONO, weight=600)
        s.text(70 + (22 if p else 0), yy, t, size=16, fill=col, family=MONO)
    s.text(70, ty + 72 + len(L) * 28, "$ ", size=16, fill=C["amber"], family=MONO, weight=600)
    s.rect(94, ty + 58 + len(L) * 28, 10, 18, fill=C["text"])
    s.marker(1, 60, ty - 20); s.marker(2, W - 180, ty + 18); s.marker(3, 930, ty + 72 + 8 * 28 - 5)
    s.notes(H, [
        "Opens with ` or ~ (or the footer link). Bottom 40 % on desktop, full screen on mobile; page dimmed.",
        "Focus trapped; Esc / 'exit' closes and returns focus. ↑/↓ history, Tab completion.",
        "Output is role=log. Commands are listed in user-flows.md (F4). 'sudo hire manuel' is the punchline.",
    ], cols=1)
    save(s, "07-terminal-overlay-desktop.svg")


if __name__ == "__main__":
    service_log(); lower_page(); recruiter(); terminal()


# ---------------------------------------------------------------- 09 gallery
def gallery():
    s = base("09 — Garage gallery /garage (desktop)")
    s.hud(W, gear="—", progress=0)
    x = 120
    s.text(x, 112, "‹  Back to the ride", size=15, fill=C["cyan"])
    s.label(x, 160, "Garage · gallery")
    s.text(x, 214, "Off the clock", size=48, weight=700, family=DISPLAY)
    cx = x
    for i, (t, n) in enumerate([("All", 3), ("Track", 3), ("Workbench", 0), ("Games", 0)]):
        w = len(t) * 9 + 64
        on = i == 0
        s.rect(cx, 244, w, 36, fill=C["raised"] if on else "none", stroke=C["amber"] if on else C["line"], r=18)
        s.text(cx + w / 2, 267, f"{t}  {n}", size=14, fill=C["text"] if on else (C["text2"] if n else C["muted"]),
               anchor="middle", weight=600 if on else 400, family=MONO)
        cx += w + 10
    gx, gy, cw, gh, g = x, 310, 280, 240, 16
    # row 1: landscape spanning 2 cols + 2 singles
    s.photo(gx, gy, cw * 2 + g, gh, "track-03", r=8)
    s.photo(gx + 2 * (cw + g), gy, cw, gh, "track-02", r=8)
    s.photo(gx + 3 * (cw + g), gy, cw, gh, "track-01", r=8)
    for k, (cx2, it) in enumerate([(gx, "Leaning into a corner"), (gx + 2 * (cw + g), "Track day"), (gx + 3 * (cw + g), "Check behind")]):
        s.rect(cx2, gy + gh - 36, (cw * 2 + g) if k == 0 else cw, 36, fill=C["bg"], op=0.55)
        s.text(cx2 + 12, gy + gh - 13, it, size=13, fill=C["text"])
    # row 2: future slots
    for k in range(4):
        s.slot(gx + k * (cw + g), gy + gh + g, cw, 200, "drop in content/media + 1 JSON entry" if k == 0 else "your next photo")
    s.marker(1, x + 470, 262); s.marker(2, gx + cw * 2 + g - 10, gy + 14); s.marker(3, gx + cw * 4 + 3 * g + 22, gy + gh + 40)
    s.notes(H, [
        "Category filter chips with counts; empty categories stay visible but muted (they fill as photos are added).",
        "Dense CSS grid: landscape/featured photos span 2 columns; focal-point crops; blurred placeholders while loading.",
        "Dashed slots only exist in the wireframe: they show where new photos land. Adding one = file + manifest entry + push.",
    ], cols=1)
    save(s, "09-garage-gallery-desktop.svg")


# ---------------------------------------------------------------- 10 lightbox
def lightbox():
    s = base("10 — Gallery lightbox (desktop, native <dialog>)")
    s.rect(0, 0, W, H, fill="#050506")
    s.text(40, 48, "2 / 3", size=15, fill=C["text2"], family=MONO)
    s.text(W - 40, 48, "Copy link   ✕ Close (Esc)", size=14, fill=C["text2"], family=MONO, anchor="end")
    iw, ih = 1050, 700
    ix, iy = (W - iw) / 2, 80
    s.photo(ix, iy, iw, ih, "track-02", r=4, border=False)
    for side, ax in [("‹", 60), ("›", W - 60)]:
        s.circle(ax, iy + ih / 2, 28, fill=C["raised"], stroke=C["line"])
        s.text(ax, iy + ih / 2 + 10, side, size=30, fill=C["text"], anchor="middle")
    s.text(ix, iy + ih + 44, "Track day. The best way to clear my head.", size=20, fill=C["text"])
    s.text(ix + iw, iy + ih + 44, "Photo: B.S Racing · MotoGP Cerezer", size=14, fill=C["muted"], family=MONO, anchor="end")
    s.marker(1, 110, 42); s.marker(2, 60, iy + ih / 2 - 50); s.marker(3, ix + iw - 20, iy + ih + 20)
    s.notes(H, [
        "Counter + deep link (/garage#track-02). Esc, click outside or ✕ closes; focus returns to the thumbnail.",
        "←/→ keys, buttons or swipe. Next image is preloaded. No zoom animation under reduced motion.",
        "Caption (localized) + photographer credit from gallery.json, always visible for event photos.",
    ], cols=1)
    save(s, "10-gallery-lightbox-desktop.svg")


if __name__ == "__main__":
    gallery(); lightbox()
