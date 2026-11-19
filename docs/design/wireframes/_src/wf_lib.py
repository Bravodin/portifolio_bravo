"""Tiny SVG helper library for TRACE mid-fidelity wireframes."""
from xml.sax.saxutils import escape

C = dict(bg="#0E0F11", surface="#16181B", raised="#1F2226", line="#2E3238",
         strong="#6B717A", text="#ECEAE4", text2="#A8ABB0", muted="#8A8E95",
         amber="#FFB020", cyan="#3DD6F5", after="#4ADE80", before="#FF6B66",
         note="#FF4FD8")
DISPLAY = "Barlow Condensed, Arial Narrow, DejaVu Sans Condensed, sans-serif"
BODY = "Barlow, DejaVu Sans, sans-serif"
MONO = "JetBrains Mono, DejaVu Sans Mono, monospace"


class Svg:
    def __init__(self, w, h, title):
        self.w, self.h, self.title, self.parts = w, h, title, []

    def add(self, s):
        self.parts.append(s)
        return self

    def render(self):
        return (f'<svg xmlns="http://www.w3.org/2000/svg" width="{self.w}" height="{self.h}" '
                f'viewBox="0 0 {self.w} {self.h}" font-family="{BODY}">\n'
                f'<title>{escape(self.title)}</title>\n'
                f'<defs><filter id="glow" x="-200%" y="-200%" width="500%" height="500%">'
                f'<feGaussianBlur stdDeviation="6"/></filter></defs>\n'
                + "\n".join(self.parts) + "\n</svg>\n")

    # primitives -------------------------------------------------------
    def rect(self, x, y, w, h, fill="none", stroke=None, r=0, sw=1, dash=None, op=None):
        a = f' stroke="{stroke}" stroke-width="{sw}"' if stroke else ""
        a += f' stroke-dasharray="{dash}"' if dash else ""
        a += f' opacity="{op}"' if op is not None else ""
        return self.add(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" rx="{r}" fill="{fill}"{a}/>')

    def line(self, x1, y1, x2, y2, stroke, sw=1, dash=None):
        d = f' stroke-dasharray="{dash}"' if dash else ""
        return self.add(f'<line x1="{x1}" y1="{y1}" x2="{x2}" y2="{y2}" stroke="{stroke}" stroke-width="{sw}"{d}/>')

    def circle(self, cx, cy, r, fill="none", stroke=None, sw=1, filt=None, op=None):
        a = f' stroke="{stroke}" stroke-width="{sw}"' if stroke else ""
        a += f' filter="url(#{filt})"' if filt else ""
        a += f' opacity="{op}"' if op is not None else ""
        return self.add(f'<circle cx="{cx}" cy="{cy}" r="{r}" fill="{fill}"{a}/>')

    def path(self, d, stroke="none", fill="none", sw=1, cap="round", dash=None):
        da = f' stroke-dasharray="{dash}"' if dash else ""
        return self.add(f'<path d="{d}" stroke="{stroke}" fill="{fill}" stroke-width="{sw}" stroke-linecap="{cap}"{da}/>')

    def text(self, x, y, s, size=16, fill=C["text"], weight=400, family=BODY,
             anchor="start", ls=None, italic=False, deco=None):
        a = f' letter-spacing="{ls}"' if ls else ""
        a += ' font-style="italic"' if italic else ""
        a += f' text-decoration="{deco}"' if deco else ""
        return self.add(f'<text x="{x}" y="{y}" font-size="{size}" fill="{fill}" font-weight="{weight}" '
                        f'font-family="{family}" text-anchor="{anchor}"{a}>{escape(s)}</text>')

    def lines(self, x, y, items, size=16, fill=C["text2"], lh=1.55, **kw):
        for i, s in enumerate(items):
            self.text(x, y + i * size * lh, s, size=size, fill=fill, **kw)
        return self

    # components -------------------------------------------------------
    def label(self, x, y, s, fill=C["cyan"], size=12, anchor="start"):
        return self.text(x, y, s.upper(), size=size, fill=fill, weight=600, family=MONO, ls="0.08em", anchor=anchor)

    def chip(self, x, y, s, fill=C["raised"], color=C["cyan"], size=13):
        w = len(s) * size * 0.62 + 20
        self.rect(x, y, w, 26, fill=fill, r=2)
        self.text(x + w / 2, y + 17.5, s, size=size, fill=color, family=MONO, anchor="middle")
        return w

    def chips(self, x, y, items, gap=8, maxw=10_000, size=13):
        cx, cy = x, y
        for s in items:
            w = len(s) * size * 0.62 + 20
            if cx + w > x + maxw:
                cx, cy = x, cy + 34
            self.chip(cx, cy, s, size=size)
            cx += w + gap
        return cy + 26

    def button(self, x, y, s, kind="primary", w=None, h=48, size=16, fill_pct=None):
        w = w or len(s) * size * 0.58 + 48
        if kind == "primary":
            self.rect(x, y, w, h, fill=C["amber"], r=6)
            col = C["bg"]
            if fill_pct is not None:  # fuel gauge: thin track at the bottom of the button
                self.rect(x + 10, y + h - 9, w - 20, 4, fill=C["bg"], r=2, op=0.3)
                self.rect(x + 10, y + h - 9, (w - 20) * fill_pct, 4, fill=C["bg"], r=2)
        elif kind == "secondary":
            self.rect(x, y, w, h, fill="none", stroke=C["strong"], r=6, sw=1.5)
            col = C["text"]
        else:
            col = C["cyan"]
        self.text(x + w / 2, y + h / 2 + size * 0.35, s, size=size, fill=col, weight=600, anchor="middle")
        return w

    def metric(self, x, y, before, before_lbl, after, after_lbl, delta, big=96, compact=False):
        bs = 28 if not compact else 18
        self.text(x, y, before, size=bs, fill=C["before"], weight=600, family=DISPLAY, deco="line-through")
        self.text(x, y + (22 if not compact else 16), before_lbl, size=13 if not compact else 11, fill=C["muted"], family=MONO)
        bw = max(len(before) * bs * 0.5, len(before_lbl) * (13 if not compact else 11) * 0.62) + 24
        self.text(x + bw, y, "→", size=bs, fill=C["muted"], weight=600)
        ax = x + bw + bs * 1.3
        self.text(ax, y + (big * 0.18 if not compact else 4), after, size=big, fill=C["after"], weight=700, family=DISPLAY)
        self.text(ax, y + (big * 0.18 + 26 if not compact else 22), after_lbl, size=13 if not compact else 11, fill=C["muted"], family=MONO)
        if delta:
            aw = len(after) * big * 0.5
            self.chip(ax + aw + 16, y - big * 0.45 if not compact else y - 16, delta, color=C["after"])

    def spine(self, x, y1, y2, nodes, active=None, packet_at=None, labels=True):
        """nodes: list of (y, label). active: index."""
        self.line(x, y1, x, y2, C["line"], sw=2)
        if active is not None:
            self.line(x, y1, x, nodes[active][0], C["amber"], sw=2)
        for i, (ny, lbl) in enumerate(nodes):
            on = active is not None and i <= active
            self.circle(x, ny, 9, fill=C["bg"], stroke=C["amber"] if on else C["strong"], sw=2)
            if labels:
                self.text(x - 20, ny + 4, lbl, size=11, fill=C["amber"] if on else C["muted"],
                          family=MONO, anchor="end", weight=600)
        if packet_at is not None:
            self.circle(x, packet_at, 12, fill=C["amber"], filt="glow", op=0.8)
            self.circle(x, packet_at, 6, fill=C["amber"])

    def hud(self, w, gear="N", progress=0.0, recruiter=True, lang=True, mobile=False, x0=0, y0=0):
        h = 56 if mobile else 64
        self.rect(x0, y0, w, h, fill=C["surface"], op=0.95)
        self.line(x0, y0 + h, x0 + w, y0 + h, C["line"])
        pad = 16 if mobile else 32
        self.rect(x0 + pad, y0 + h / 2 - 16, 32, 32, fill="none", stroke=C["amber"], r=2, sw=1.5)
        self.text(x0 + pad + 16, y0 + h / 2 + 5.5, "MB", size=14, fill=C["amber"], weight=700, family=DISPLAY, anchor="middle")
        if not mobile:
            self.label(x0 + pad + 48, y0 + h / 2 + 4.5, "Trace", fill=C["text2"])
        gx = x0 + (pad + 64 if mobile else pad + 140)
        self.label(gx, y0 + h / 2 + 4.5, "Gear", fill=C["muted"], size=11)
        self.rect(gx + 42, y0 + h / 2 - 16, 44, 32, fill=C["raised"], stroke=C["amber"], r=2, sw=1.2)
        self.text(gx + 64, y0 + h / 2 + 8, gear, size=22 if len(gear) < 3 else 16, fill=C["amber"], weight=700, family=MONO, anchor="middle")
        # progress
        self.rect(x0, y0 + h - 2, w, 2, fill=C["line"])
        if progress:
            self.rect(x0, y0 + h - 2, w * progress, 2, fill=C["amber"])
        rx = x0 + w - pad
        # menu
        for i in range(3):
            self.line(rx - 22, y0 + h / 2 - 7 + i * 7, rx, y0 + h / 2 - 7 + i * 7, C["text"], sw=2)
        rx -= 40
        if recruiter:
            if mobile:
                self.rect(rx - 64, y0 + h / 2 - 16, 64, 32, fill="none", stroke=C["amber"], r=6)
                self.text(rx - 32, y0 + h / 2 + 5, "⏱ 30 s", size=13, fill=C["amber"], weight=600, anchor="middle")
                rx -= 80
            else:
                bw = 196
                self.rect(rx - bw, y0 + h / 2 - 18, bw, 36, fill="none", stroke=C["amber"], r=6, sw=1.5)
                self.text(rx - bw / 2, y0 + h / 2 + 5, "Recruiter mode (30 s)", size=14, fill=C["amber"], weight=600, anchor="middle")
                rx -= bw + 20
        if lang and not mobile:
            for i, l in enumerate(["PT", "EN", "ES"][::-1]):
                lx = rx - (i + 1) * 40
                on = l == "EN"
                self.rect(lx, y0 + h / 2 - 14, 36, 28, fill=C["raised"] if on else "none", r=2)
                self.text(lx + 18, y0 + h / 2 + 4.5, l, size=12, fill=C["text"] if on else C["muted"], family=MONO, anchor="middle", weight=600)
        return h

    def tach(self, cx, cy, r, frac, color, label=None):
        import math
        start, sweep = 150, 240

        def pt(a, rr):
            rad = math.radians(a)
            return cx + rr * math.cos(rad), cy + rr * math.sin(rad)

        def arc(a0, a1, rr):
            x0, y0 = pt(a0, rr)
            x1, y1 = pt(a1, rr)
            large = 1 if (a1 - a0) > 180 else 0
            return f"M{x0:.1f},{y0:.1f} A{rr},{rr} 0 {large} 1 {x1:.1f},{y1:.1f}"
        self.path(arc(start, start + sweep, r), stroke=C["line"], sw=8, cap="butt")
        self.path(arc(start + sweep * 0.78, start + sweep, r), stroke=C["before"], sw=8, cap="butt")
        self.path(arc(start, start + sweep * frac, r), stroke=color, sw=8, cap="butt")
        for i in range(9):
            a = start + sweep * i / 8
            x0, y0 = pt(a, r - 14)
            x1, y1 = pt(a, r - 6)
            self.line(x0, y0, x1, y1, C["strong"], sw=1.5)
        nx, ny = pt(start + sweep * frac, r - 18)
        self.line(cx, cy, nx, ny, C["text"], sw=2.5)
        self.circle(cx, cy, 5, fill=C["text"])
        if label:
            self.text(cx, cy + r * 0.62, label, size=10, fill=C["muted"], family=MONO, anchor="middle")

    def marker(self, n, x, y):
        self.circle(x, y, 13, fill=C["note"])
        self.text(x, y + 5, str(n), size=14, fill="#000", weight=700, anchor="middle")

    def notes(self, y, items, x=32, cols=2, colw=None):
        colw = colw or (self.w - 64) / cols
        self.rect(0, y, self.w, self.h - y, fill="#0A0A0B")
        self.line(0, y, self.w, y, C["note"], sw=2, dash="6 6")
        self.label(x, y + 30, "Annotations", fill=C["note"])
        rows = (len(items) + cols - 1) // cols
        for i, s in enumerate(items):
            cx = x + (i // rows) * colw
            cy = y + 62 + (i % rows) * 30
            self.marker(i + 1, cx + 13, cy - 5)
            self.text(cx + 36, cy, s, size=14, fill=C["text2"])

    def frame_title(self, x, y, s, sub=None):
        self.text(x, y, s, size=18, fill=C["text"], weight=600)
        if sub:
            self.text(x, y + 22, sub, size=13, fill=C["muted"], family=MONO)


# ---------------------------------------------------------------- photos
import base64, io, json, os
_HERE = os.path.dirname(os.path.abspath(__file__))
MEDIA = os.environ.get("TRACE_MEDIA", os.path.normpath(os.path.join(_HERE, "../../../../content/media")))
_clip_id = [0]


def manifest():
    with open(os.path.join(MEDIA, "gallery.json")) as f:
        return {i["id"]: i for i in json.load(f)["items"]}


def _crop_b64(item_id, w, h, scale=1.25, quality=70):
    from PIL import Image
    it = manifest()[item_id]
    im = Image.open(os.path.join(MEDIA, it["file"])).convert("RGB")
    fx, fy = (it.get("focalPoint") or {"x": .5, "y": .5}).values()
    iw, ih = im.size
    target = w / h
    if iw / ih > target:
        cw, ch = ih * target, ih
    else:
        cw, ch = iw, iw / target
    cx = min(max(fx * iw - cw / 2, 0), iw - cw)
    cy = min(max(fy * ih - ch / 2, 0), ih - ch)
    im = im.crop((int(cx), int(cy), int(cx + cw), int(cy + ch)))
    im = im.resize((max(1, int(w * scale)), max(1, int(h * scale))), Image.LANCZOS)
    buf = io.BytesIO()
    im.save(buf, "JPEG", quality=quality, optimize=True)
    return base64.b64encode(buf.getvalue()).decode()


def photo(self, x, y, w, h, item_id, r=0, round_=False, border=True):
    _clip_id[0] += 1
    cid = f"clip{_clip_id[0]}"
    shape = (f'<circle cx="{x + w / 2}" cy="{y + h / 2}" r="{w / 2}"/>' if round_
             else f'<rect x="{x}" y="{y}" width="{w}" height="{h}" rx="{r}"/>')
    self.add(f'<clipPath id="{cid}">{shape}</clipPath>')
    self.add(f'<image x="{x}" y="{y}" width="{w}" height="{h}" preserveAspectRatio="xMidYMid slice" '
             f'clip-path="url(#{cid})" href="data:image/jpeg;base64,{_crop_b64(item_id, w, h)}"/>')
    if border:
        if round_:
            self.circle(x + w / 2, y + h / 2, w / 2, stroke=C["line"], sw=1)
        else:
            self.rect(x, y, w, h, stroke=C["line"], r=r)


def slot(self, x, y, w, h, label="your next photo", r=8):
    self.rect(x, y, w, h, fill="none", stroke=C["strong"], r=r, dash="6 6")
    self.text(x + w / 2, y + h / 2 - 4, "+", size=28, fill=C["muted"], anchor="middle")
    self.text(x + w / 2, y + h / 2 + 20, label, size=12, fill=C["muted"], family=MONO, anchor="middle")


def blueprint(self, x, y, w, h, kind, r=8):
    """Line-art placeholder for categories without a photo."""
    self.rect(x, y, w, h, fill=C["raised"], r=r)
    for i in range(1, 8):
        self.line(x + i * w / 8, y, x + i * w / 8, y + h, C["line"], sw=0.6)
    for i in range(1, 6):
        self.line(x, y + i * h / 6, x + w, y + i * h / 6, C["line"], sw=0.6)
    cx, cy = x + w / 2, y + h / 2
    s = C["cyan"]
    if kind == "wrench":
        self.path(f"M{cx-70},{cy+50} L{cx+30},{cy-50}", stroke=s, sw=14)
        self.circle(cx + 45, cy - 65, 28, stroke=s, sw=3)
        self.rect(cx + 32, cy - 100, 26, 34, fill=C["raised"])
        self.circle(cx - 70, cy + 50, 12, stroke=s, sw=3)
    else:  # gamepad
        self.path(f"M{cx-90},{cy-30} Q{cx},{cy-55} {cx+90},{cy-30} Q{cx+120},{cy+60} {cx+70},{cy+55} "
                  f"Q{cx+40},{cy+20} {cx},{cy+20} Q{cx-40},{cy+20} {cx-70},{cy+55} Q{cx-120},{cy+60} {cx-90},{cy-30} Z",
                  stroke=s, sw=3)
        self.path(f"M{cx-60},{cy-8} h24 M{cx-48},{cy-20} v24", stroke=s, sw=3)
        for dx, dy in [(48, -14), (62, 0), (48, 14), (34, 0)]:
            self.circle(cx + dx, cy + dy - 6, 5, stroke=s, sw=2)
    self.text(x + w - 12, y + h - 12, "photo coming soon", size=11, fill=C["muted"], family=MONO, anchor="end")


Svg.photo = photo
Svg.slot = slot
Svg.blueprint = blueprint
