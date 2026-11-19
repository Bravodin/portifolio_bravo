import sys, os
sys.path.insert(0, os.path.dirname(__file__))
from wf_lib import Svg, C, DISPLAY, MONO

OUT = sys.argv[1]
PW, PH, GAP, TOP = 390, 844, 48, 90
N = 4
W = N * PW + (N + 1) * GAP
H = TOP + PH + 40
s = Svg(W, H + 200, "08 — Mobile (390 × 844): Landing · Stage 1 · Recruiter · Terminal")
s.rect(0, 0, W, H, fill="#08090A")
titles = [("08a Landing", "prerendered, CTA above fold"), ("08b Stage 1 + Dyno", "lanes stacked, compact"),
          ("08c Recruiter mode", "no motion, sticky CTA"), ("08d Terminal", "full screen, soft keyboard")]


def phone(i):
    x = GAP + i * (PW + GAP)
    s.frame_title(x, 40, *titles[i])
    s.rect(x, TOP, PW, PH, fill=C["bg"], stroke=C["strong"], r=28, sw=2)
    return x, TOP


def sticky_cta(x, y):
    s.rect(x, y + PH - 76, PW, 76, fill=C["surface"])
    s.line(x, y + PH - 76, x + PW, y + PH - 76, C["line"])
    s.button(x + 16, y + PH - 62, "CV (PDF)", w=170, h=48)
    s.button(x + 204, y + PH - 62, "LinkedIn ↗", kind="secondary", w=170, h=48)


# a — landing
x, y = phone(0)
s.hud(PW, gear="N", mobile=True, x0=x, y0=y + 8)
s.line(x + 20, y + 80, x + 20, y + PH - 30, C["line"], sw=2)
s.circle(x + 20, y + 120, 7, fill=C["bg"], stroke=C["amber"], sw=2)
s.label(x + 44, y + 124, "Lead SW Engineer · Brazil", size=11)
s.text(x + 42, y + 190, "Manuel", size=64, weight=700, family=DISPLAY)
s.text(x + 42, y + 250, "Bravo", size=64, weight=700, family=DISPLAY)
s.text(x + 44, y + 300, "I modernize critical platforms", size=26, weight=600, family=DISPLAY)
s.text(x + 44, y + 330, "without stopping production.", size=26, weight=600, family=DISPLAY, fill=C["amber"])
s.lines(x + 44, y + 370, ["Full-stack .NET · Test automation", "& quality · AWS · Team building."], size=15)
s.button(x + 44, y + 430, "Start the ride ▸", w=310, h=52, fill_pct=0.4)
s.button(x + 44, y + 494, "Recruiter mode (30 s)", kind="secondary", w=310, h=52)
for k, (v, l) in enumerate([("~1.2 s", "booking, from 12–20 s"), ("−70 %", "vulnerabilities"), ("−30 %", "platform cost")]):
    yy = y + 600 + k * 56
    s.text(x + 44, yy, v, size=28, weight=700, family=DISPLAY, fill=C["after"])
    s.text(x + 150, yy - 4, l, size=12, fill=C["muted"], family=MONO)
s.text(x + PW / 2, y + PH - 30, "scroll to ride ↓", size=12, fill=C["muted"], family=MONO, anchor="middle")

# b — stage 1
x, y = phone(1)
s.hud(PW, gear="1", progress=0.1, mobile=True, x0=x, y0=y + 8)
s.line(x + 20, y + 80, x + 20, y + PH, C["line"], sw=2)
s.line(x + 20, y + 80, x + 20, y + 120, C["amber"], sw=2)
s.circle(x + 20, y + 120, 10, fill=C["amber"], filt="glow", op=0.8)
s.circle(x + 20, y + 120, 5, fill=C["amber"])
s.label(x + 44, y + 124, "Gear 1 · Client", size=11)
s.text(x + 44, y + 166, "The app that made", size=32, weight=700, family=DISPLAY)
s.text(x + 44, y + 200, "people wait", size=32, weight=700, family=DISPLAY)
s.text(x + 44, y + 250, "12–20 s", size=20, fill=C["before"], weight=600, family=DISPLAY, deco="line-through")
s.text(x + 120, y + 250, "→", size=20, fill=C["muted"])
s.text(x + 150, y + 262, "~1.2 s", size=60, fill=C["after"], weight=700, family=DISPLAY)
s.chips(x + 44, y + 286, ["Swift", "Kotlin", "XCUITest"], size=12)
s.rect(x + 44, y + 336, 330, 380, fill=C["raised"], stroke=C["line"], r=12)
s.label(x + 60, y + 364, "Dyno · book appointment", fill=C["amber"], size=11)
for k, (n, f, col, t, st) in enumerate([("LEGACY", 0.8, C["before"], "09.8 s", "loading…"),
                                        ("NATIVE", 0.2, C["after"], "01.2 s", "✓ Booked")]):
    yy = y + 384 + k * 140
    s.rect(x + 60, yy, 298, 124, fill=C["surface"], r=8)
    s.tach(x + 120, yy + 66, 42, f, col)
    s.text(x + 184, yy + 40, n, size=13, fill=col, family=MONO, weight=700)
    s.text(x + 184, yy + 78, t, size=30, family=MONO, weight=700)
    s.text(x + 184, yy + 104, st, size=13, fill=col, weight=600)
s.button(x + 60, y + 670, "Run again", w=130, h=36, size=14)
s.text(x + 206, y + 693, "Skip →", size=14, fill=C["cyan"])
sticky_cta(x, y)

# c — recruiter
x, y = phone(2)
s.rect(x, y + 8, PW, 56, fill=C["surface"])
s.text(x + 16, y + 42, "‹ Ride", size=14, fill=C["cyan"])
s.label(x + PW / 2, y + 41, "Recruiter · 30 s", fill=C["amber"], size=11, anchor="middle")
s.text(x + PW - 16, y + 42, "EN ▾", size=13, fill=C["text2"], family=MONO, anchor="end")
s.photo(x + 20, y + 80, 68, 68, "profile", r=10)
s.text(x + 102, y + 112, "Manuel Bravo", size=34, weight=700, family=DISPLAY)
s.text(x + 102, y + 138, "Lead SW Engineer · Porto", size=14, fill=C["amber"], weight=600)
s.lines(x + 20, y + 174, ["20+ yrs · Insurance · Health · Finance", "Brazil · remote · PT · ES · EN"], size=14)
for k, (b, a, l) in enumerate([("12–20 s", "~1.2 s", "booking"), ("", "−70 %", "vulnerabilities"),
                               ("", "−30 %", "API cost"), ("", "−40 %", "fraud"),
                               ("70 %", "98 %", "SLA"), ("", "+40 %", "productivity")]):
    cx = x + 20 + (k % 2) * 180
    cy = y + 230 + (k // 2) * 96
    s.rect(cx, cy, 170, 84, fill=C["surface"], stroke=C["line"], r=8)
    if b:
        s.text(cx + 12, cy + 22, b, size=11, fill=C["before"], family=MONO, deco="line-through")
    s.text(cx + 12, cy + 56, a, size=32, fill=C["after"], weight=700, family=DISPLAY)
    s.text(cx + 12, cy + 74, l, size=11, fill=C["muted"], family=MONO)
s.label(x + 20, y + 540, "Core stack", size=11)
s.chips(x + 20, y + 552, ["C#", ".NET", "Node.js", "TypeScript", "AWS", "Redis", "SQL Server", "Selenium", "JMeter"], maxw=350, size=12)
sticky_cta(x, y)

# d — terminal
x, y = phone(3)
s.rect(x + 2, y + 2, PW - 4, PH - 4, fill=C["raised"], r=26)
s.rect(x, y + 8, PW, 48, fill=C["surface"])
s.text(x + 16, y + 38, "~/trace", size=13, fill=C["muted"], family=MONO)
s.text(x + PW - 16, y + 38, "✕ close", size=13, fill=C["text2"], family=MONO, anchor="end")
L = [("$ help", C["text"]), ("whoami  trace  git log  skills", C["text2"]), ("cv  contact  lang  achievements", C["text2"]),
     ("sudo …  clear  exit", C["text2"]), ("$ skills diagnostics", C["text"]), ("├─ Selenium", C["text2"]), ("├─ JMeter", C["text2"]),
     ("├─ Postman", C["text2"]), ("└─ Sonar 90 % gate", C["text2"]), ("$ █", C["amber"])]
for k, (t, col) in enumerate(L):
    s.text(x + 16, y + 90 + k * 26, t, size=14, fill=col, family=MONO)
# quick command chips above keyboard
s.chips(x + 12, y + PH - 330, ["help", "whoami", "cv", "sudo hire manuel"], size=12)
s.rect(x + 2, y + PH - 290, PW - 4, 288, fill="#2A2D31", r=0)
for r_ in range(3):
    for c_ in range(10):
        s.rect(x + 9 + c_ * 37.5, y + PH - 278 + r_ * 62, 33, 48, fill="#3A3E44", r=6)
s.rect(x + 95, y + PH - 92, 200, 48, fill="#3A3E44", r=6)
s.text(x + PW / 2, y + PH - 62, "on-screen keyboard", size=12, fill=C["muted"], family=MONO, anchor="middle")

for i, (mx, my) in enumerate([(GAP + 360, TOP + 452), (GAP + PW + GAP + 360, TOP + 360),
                              (GAP + 2 * (PW + GAP) + 360, TOP + PH - 40), (GAP + 3 * (PW + GAP) + 360, TOP + PH - 318)]):
    s.marker(i + 1, mx, my)
s.notes(H, [
    "Landing: both CTAs above the fold at 390 × 844; spine shrinks to a 20 px gutter.",
    "Dyno lanes stack vertically; tachometers shrink; same states as desktop.",
    "Recruiter: portrait + identity first. Sticky bottom CTA (CV + LinkedIn) on every page after the hero.",
    "Terminal goes full screen; quick-command chips avoid typing on a phone keyboard.",
], cols=2)
os.makedirs(OUT, exist_ok=True)
open(os.path.join(OUT, "08-mobile-key-screens.svg"), "w").write(s.render())
