"""
M2-Bis Sprite Generator for Turtle Path — v2 (improved pixel art)
"""
from PIL import Image, ImageDraw
import math
import os
import random

# === PALETTE ===
SAND_LIGHT    = (255, 238, 173)  # #FFEEAD
SAND_WARM     = (255, 204, 92)   # #FFCC5C
SAND_DARK     = (220, 175, 60)   # darker sand for depth
SHELL_WHITE   = (255, 248, 231)  # #FFF8E7
TURTLE_GREEN  = (76, 175, 80)    # #4CAF50
TURTLE_SHELL  = (139, 105, 20)   # #8B6914
DEEP_BROWN    = (93, 64, 55)     # #5D4037
CORAL_PINK    = (255, 111, 105)  # #FF6F69
CORAL_DARK    = (200, 80, 75)    # darker coral
BABY_PINK     = (255, 182, 193)  # #FFB6C1
BABY_DARK     = (220, 150, 160)  # darker pink
OCEAN_TEAL    = (150, 206, 180)  # #96CEB4
OCEAN_DEEP    = (30, 205, 203)   # #1ECDCB
TRANSPARENT   = (0, 0, 0, 0)

BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART_DIR = os.path.join(BASE_DIR, "My project", "Assets", "Art")
SRC_DIR = os.path.join(ART_DIR, "_SourceAssets")


def ensure_dirs():
    for sub in ["Tiles", "Characters", "Collectibles", "Cells"]:
        os.makedirs(os.path.join(ART_DIR, sub), exist_ok=True)


def a(color, alpha=255):
    """Add alpha to RGB tuple."""
    return color + (alpha,)


# =============================================================
# 1. STRAIGHT TILE (N-S) — 64x64
# =============================================================
def generate_straight():
    S = 64
    img = Image.new("RGBA", (S, S), TRANSPARENT)

    pw = 32  # path width
    left = (S - pw) // 2
    right = left + pw

    random.seed(101)

    for y in range(S):
        for x in range(left - 1, right + 1):
            dx = x - left
            if dx < 0 or dx >= pw:
                # 1px border
                img.putpixel((x, y), a(DEEP_BROWN))
            else:
                # Fill: mostly Sand Warm, subtle center highlight
                img.putpixel((x, y), a(SAND_WARM))

    # Subtle center line (Sand Light, not white — keeps it flat)
    cx = S // 2
    for y in range(S):
        for x in range(cx - 2, cx + 3):
            img.putpixel((x, y), a(SAND_LIGHT))

    # A few sand grain dots for texture
    for _ in range(25):
        gx = random.randint(left + 2, right - 3)
        gy = random.randint(0, S - 1)
        img.putpixel((gx, gy), a(random.choice([SAND_DARK, SAND_LIGHT])))

    path = os.path.join(ART_DIR, "Tiles", "tile_straight.png")
    img.save(path)
    print(f"  Saved {path}")


# =============================================================
# 2. CURVE TILE (N-E) — 64x64
# =============================================================
def generate_curve():
    S = 64
    img = Image.new("RGBA", (S, S), TRANSPARENT)

    pw = 32   # path width (same as straight)
    half = pw // 2

    # Pivot point for the curve: bottom-left corner of the path channel.
    # North opening is centered at x = S//2, East opening at y = S//2.
    # The pivot sits where the inner edges of the two straight sections meet.
    px = S // 2 - half  # 16
    py = S // 2 + half  # 48

    random.seed(202)

    for y in range(S):
        for x in range(S):
            in_path = False
            on_border = False

            # --- North straight section (above curve zone) ---
            if y < py and S // 2 - half - 1 <= x <= S // 2 + half:
                if x == S // 2 - half - 1 or x == S // 2 + half:
                    on_border = True
                else:
                    in_path = True

            # --- East straight section (right of curve zone) ---
            if x >= px and S // 2 - half - 1 <= y <= S // 2 + half:
                if y == S // 2 - half - 1 or y == S // 2 + half:
                    on_border = True
                else:
                    in_path = True

            # --- Curved corner: quarter circle from pivot ---
            dist = math.sqrt((x - px) ** 2 + (y - py) ** 2)
            if x >= px and y <= py:
                if half <= dist <= half + pw:
                    in_path = True
                elif half - 1 <= dist < half or half + pw < dist <= half + pw + 1:
                    on_border = True

            # --- Clean up: remove anything outside the quarter ---
            # The path should only exist in the top-right quadrant relative to pivot
            if x < px - 1 and y > py + 1:
                in_path = False
                on_border = False

            if on_border and not in_path:
                img.putpixel((x, y), a(DEEP_BROWN))
            elif in_path:
                img.putpixel((x, y), a(SAND_WARM))

    # --- Center line highlight (follows the curve center) ---
    center_r = half + pw // 2  # radius of center of path
    # Curved section
    for deg in range(0, 91):
        rad = math.radians(deg)
        cx = int(px + center_r * math.cos(rad))
        cy = int(py - center_r * math.sin(rad))
        for ddx in range(-1, 2):
            for ddy in range(-1, 2):
                nx, ny = cx + ddx, cy + ddy
                if 0 <= nx < S and 0 <= ny < S and img.getpixel((nx, ny))[3] > 0:
                    img.putpixel((nx, ny), a(SAND_LIGHT))

    # North straight center line
    ncx = S // 2
    for y in range(0, py - center_r + 5):
        for x in range(ncx - 1, ncx + 2):
            if img.getpixel((x, y))[3] > 0:
                img.putpixel((x, y), a(SAND_LIGHT))

    # East straight center line
    ecy = S // 2
    for x in range(px + center_r - 5, S):
        for y in range(ecy - 1, ecy + 2):
            if img.getpixel((x, y))[3] > 0:
                img.putpixel((x, y), a(SAND_LIGHT))

    # Sand grains
    for _ in range(25):
        gx = random.randint(0, S - 1)
        gy = random.randint(0, S - 1)
        if img.getpixel((gx, gy))[3] > 0:
            r, g, b, _ = img.getpixel((gx, gy))
            if (r, g, b) == SAND_WARM:
                img.putpixel((gx, gy), a(random.choice([SAND_DARK, SAND_LIGHT])))

    path = os.path.join(ART_DIR, "Tiles", "tile_curve.png")
    img.save(path)
    print(f"  Saved {path}")


# =============================================================
# 3. TURTLE SPRITE — 64x64 (from Oiboo spritesheet)
# =============================================================
def generate_turtle():
    sheet = Image.open(os.path.join(SRC_DIR, "Turtle_TopDown_64x64_SpriteSheet.png")).convert("RGBA")
    frame = sheet.crop((0, 0, 64, 64))

    out = Image.new("RGBA", (64, 64), TRANSPARENT)
    for y in range(64):
        for x in range(64):
            r, g, b, alpha = frame.getpixel((x, y))
            if alpha < 10:
                continue

            brightness = (r + g + b) / 3.0

            if brightness < 30:
                out.putpixel((x, y), DEEP_BROWN + (alpha,))
            elif r > 150 and g > 150 and b > 100:
                t = min(1.0, brightness / 220.0)
                nr = int(SAND_LIGHT[0] * t + TURTLE_GREEN[0] * (1 - t))
                ng = int(SAND_LIGHT[1] * t + TURTLE_GREEN[1] * (1 - t))
                nb = int(SAND_LIGHT[2] * t + TURTLE_GREEN[2] * (1 - t))
                out.putpixel((x, y), (nr, ng, nb, alpha))
            elif r > g:
                t = min(1.0, brightness / 150.0)
                nr = int(TURTLE_GREEN[0] * t + TURTLE_SHELL[0] * (1 - t))
                ng = int(TURTLE_GREEN[1] * t + TURTLE_SHELL[1] * (1 - t))
                nb = int(TURTLE_GREEN[2] * t + TURTLE_SHELL[2] * (1 - t))
                out.putpixel((x, y), (nr, ng, nb, alpha))
            else:
                t = min(1.0, brightness / 200.0)
                nr = int(TURTLE_GREEN[0] * t + DEEP_BROWN[0] * (1 - t))
                ng = int(TURTLE_GREEN[1] * t + DEEP_BROWN[1] * (1 - t))
                nb = int(TURTLE_GREEN[2] * t + DEEP_BROWN[2] * (1 - t))
                out.putpixel((x, y), (nr, ng, nb, alpha))

    path = os.path.join(ART_DIR, "Characters", "turtle.png")
    out.save(path)
    print(f"  Saved {path}")


# =============================================================
# 4. SHELL COLLECTIBLE — 32x32
# =============================================================
def generate_shell():
    S = 32
    img = Image.new("RGBA", (S, S), TRANSPARENT)
    cx, cy = S // 2, S // 2 + 1

    # Larger spiral shell
    for y in range(S):
        for x in range(S):
            dx, dy = x - cx, y - cy
            dist = math.sqrt(dx * dx + dy * dy)
            angle = math.atan2(dy, dx)

            # Outer shell shape (oval, taller than wide)
            ox = dx / 10.0
            oy = dy / 12.0
            if ox * ox + oy * oy > 1:
                continue

            # Spiral pattern
            spiral = (angle + dist * 0.5) % (math.pi * 2)

            if dist > 10:
                # Outer rim
                img.putpixel((x, y), a(CORAL_DARK))
            elif spiral < 1.0 and dist > 3:
                # Spiral ridge
                img.putpixel((x, y), a(CORAL_DARK))
            elif dist < 3:
                # Center
                img.putpixel((x, y), a(DEEP_BROWN))
            else:
                img.putpixel((x, y), a(CORAL_PINK))

    # Highlight
    for pos in [(cx - 3, cy - 5), (cx - 2, cy - 5), (cx - 3, cy - 4)]:
        if 0 <= pos[0] < S and 0 <= pos[1] < S:
            img.putpixel(pos, a(SHELL_WHITE))

    path = os.path.join(ART_DIR, "Collectibles", "collectible_shell.png")
    img.save(path)
    print(f"  Saved {path}")


# =============================================================
# 5. BABY TURTLE COLLECTIBLE — 32x32
# =============================================================
def generate_baby():
    S = 32
    img = Image.new("RGBA", (S, S), TRANSPARENT)
    draw = ImageDraw.Draw(img)
    cx, cy = S // 2, S // 2

    # Body/shell (oval)
    draw.ellipse([cx - 7, cy - 5, cx + 7, cy + 6], fill=a(BABY_PINK))
    # Shell pattern
    draw.ellipse([cx - 5, cy - 3, cx + 5, cy + 4], fill=a(BABY_DARK))
    draw.ellipse([cx - 3, cy - 1, cx + 3, cy + 2], fill=a(BABY_PINK))

    # Head (circle, facing up)
    draw.ellipse([cx - 3, cy - 9, cx + 3, cy - 4], fill=a(BABY_PINK))
    # Eyes
    img.putpixel((cx - 2, cy - 7), a(DEEP_BROWN))
    img.putpixel((cx + 2, cy - 7), a(DEEP_BROWN))

    # Flippers
    fl = [(cx - 8, cy - 2, cx - 6, cy + 2),   # left front
          (cx + 6, cy - 2, cx + 8, cy + 2),    # right front
          (cx - 7, cy + 3, cx - 5, cy + 6),    # left back
          (cx + 5, cy + 3, cx + 7, cy + 6)]    # right back
    for coords in fl:
        draw.ellipse(coords, fill=a(BABY_PINK))

    # Tail
    draw.rectangle([cx - 1, cy + 6, cx + 1, cy + 8], fill=a(BABY_PINK))

    # Shell highlight
    for pos in [(cx - 2, cy - 2), (cx - 1, cy - 2)]:
        img.putpixel(pos, a(SHELL_WHITE))

    path = os.path.join(ART_DIR, "Collectibles", "collectible_baby.png")
    img.save(path)
    print(f"  Saved {path}")


# =============================================================
# 6. NEST CELL — 64x64
# =============================================================
def generate_nest():
    S = 64
    img = Image.new("RGBA", (S, S), TRANSPARENT)
    cx, cy = S // 2, S // 2

    random.seed(303)

    # Sand base (full cell)
    for y in range(S):
        for x in range(S):
            img.putpixel((x, y), a(SAND_LIGHT))

    # Nest depression (circular gradient)
    for y in range(S):
        for x in range(S):
            dx, dy = x - cx, y - cy
            dist = math.sqrt(dx * dx + dy * dy)

            if dist < 26:
                t = dist / 26.0
                if t < 0.7:
                    img.putpixel((x, y), a(SAND_DARK))
                else:
                    img.putpixel((x, y), a(SAND_WARM))

    # Inner sandy area
    draw = ImageDraw.Draw(img)
    draw.ellipse([cx - 18, cy - 18, cx + 18, cy + 18], fill=a(SAND_WARM))

    # Eggshell fragments
    shells = [(cx - 8, cy - 6, 5), (cx + 5, cy - 8, 4),
              (cx - 4, cy + 5, 4), (cx + 7, cy + 3, 5),
              (cx + 1, cy - 2, 3)]
    for sx, sy, sr in shells:
        draw.ellipse([sx - sr, sy - sr, sx + sr, sy + sr], fill=a(TURTLE_SHELL))
        # Crack
        draw.line([(sx - 1, sy - 1), (sx + 2, sy + 1)], fill=a(DEEP_BROWN), width=1)
        # Highlight
        img.putpixel((sx - 1, sy - sr + 1), a(SHELL_WHITE))

    # Sand texture dots
    for _ in range(30):
        gx = random.randint(4, S - 5)
        gy = random.randint(4, S - 5)
        img.putpixel((gx, gy), a(random.choice([SAND_DARK, SHELL_WHITE])))

    path = os.path.join(ART_DIR, "Cells", "cell_nest.png")
    img.save(path)
    print(f"  Saved {path}")


# =============================================================
# 7. SEA CELL — 64x64
# =============================================================
def generate_sea():
    S = 64
    img = Image.new("RGBA", (S, S), TRANSPARENT)

    for y in range(S):
        t = y / (S - 1)

        if t < 0.2:
            c = SAND_LIGHT
        elif t < 0.35:
            blend = (t - 0.2) / 0.15
            c = tuple(int(SAND_LIGHT[i] + (OCEAN_TEAL[i] - SAND_LIGHT[i]) * blend) for i in range(3))
        elif t < 0.6:
            c = OCEAN_TEAL
        else:
            blend = (t - 0.6) / 0.4
            c = tuple(int(OCEAN_TEAL[i] + (OCEAN_DEEP[i] - OCEAN_TEAL[i]) * blend) for i in range(3))

        for x in range(S):
            img.putpixel((x, y), c + (255,))

    # Wave patterns
    for wave_y in [24, 34, 44, 54]:
        for x in range(S):
            wy = wave_y + int(1.5 * math.sin(x * 0.4 + wave_y * 0.3))
            if 0 <= wy < S:
                r, g, b, _ = img.getpixel((x, wy))
                # Lighter wave crest
                lr = min(255, r + 30)
                lg = min(255, g + 30)
                lb = min(255, b + 30)
                img.putpixel((x, wy), (lr, lg, lb, 255))
                if wy + 1 < S:
                    img.putpixel((x, wy + 1), (min(255, r + 15), min(255, g + 15), min(255, b + 15), 255))

    # Foam line at shoreline
    for x in range(S):
        fy = 14 + int(2 * math.sin(x * 0.6))
        if 0 <= fy < S:
            img.putpixel((x, fy), (255, 255, 255, 200))
            if fy + 1 < S:
                img.putpixel((x, fy + 1), (255, 255, 255, 120))

    path = os.path.join(ART_DIR, "Cells", "cell_sea.png")
    img.save(path)
    print(f"  Saved {path}")


# =============================================================
# MAIN
# =============================================================
def main():
    ensure_dirs()
    print("=== M2-Bis Sprite Generation v2 ===\n")

    print("1. Tile sprites (custom drawn)...")
    generate_straight()
    generate_curve()

    print("2. Turtle sprite (Oiboo palette remap)...")
    generate_turtle()

    print("3. Shell collectible...")
    generate_shell()

    print("4. Baby turtle collectible...")
    generate_baby()

    print("5. Nest cell...")
    generate_nest()

    print("6. Sea cell...")
    generate_sea()

    print("\n=== Done! 7 sprites generated. ===")


if __name__ == "__main__":
    main()
