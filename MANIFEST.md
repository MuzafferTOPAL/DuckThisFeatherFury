<div align="center">

<img src="docs/media/banner.png" width="100%">

# Game Manifest: DuckThis Feather Fury

*Vision, design pillars and technical specification*

| | |
|---|---|
| **Title** | DuckThis Feather Fury |
| **Developer** | DuckThisGames |
| **Genre** | Physics puzzle, slingshot |
| **Platform** | Android (phone, landscape) |
| **Engine** | Unity 6000.6.6f1 |
| **Version** | 1.0 |
| **Package** | `com.duckthisgames.featherfury` |
| **Art style** | 16 px pixel art |
| **Languages** | TR · EN · DE · ES · FR · PT · IT |

</div>

---

## 1. Vision

> *"One more shot."*

DuckThis Feather Fury is about a satisfying second of chaos: the slingshot snaps, a duck flies and a fox
fort comes crashing down. Every level is a small physics puzzle that takes seconds to read and a few tries
to master. The game is warm, funny and readable at a glance. Ducks are the heroes, foxes are the
lovable villains, and nobody gets hurt worse than a puff of feathers.

**The story in one line:** the foxes stole the ducks' eggs, and the ducks want them back, preferably with
style.

## 2. Design pillars

| Pillar | What it means in practice |
|---|---|
| **Readable chaos** | Clear silhouettes, strong colors, crack overlays and score pop-ups. You always see *why* something broke. |
| **Every duck matters** | Each species has one ability and one material it's best against. Choosing the right duck is the puzzle. |
| **Fast retries** | Turns are short: the camera follow ends quickly and the next duck hops onto the slingshot in under half a second. Restarting is one tap. |
| **Fair progression** | Difficulty ramps up slowly. Golden Eggs let a stuck player upgrade instead of quitting. |
| **Handmade charm** | All art is hand-placed 16 px pixel art, with chiptune SFX and pixel transitions between levels. |

## 3. Core loop

```
 ┌───────────┐   aim & launch   ┌────────────┐   use ability   ┌──────────────┐
 │  Choose   │ ───────────────▶ │   Flight   │ ──────────────▶ │  Destruction │
 │   level   │                  │  (tap!)    │                 │  & scoring   │
 └───────────┘                  └────────────┘                 └──────┬───────┘
       ▲                                                              │
       │      ┌──────────────┐    spend eggs    ┌───────────────┐     │
       └──────│  Shop & next │ ◀─────────────── │ Stars + Golden│ ◀───┘
              │    level     │                  │     Eggs      │
              └──────────────┘                  └───────────────┘
```

## 4. Characters

### 4.1 Ducks

| Duck | Ability (tap in flight) | Material bonus | Notes |
|---|---|---|---|
| **Mallard** | Flap upward (×2) | – | Starter duck. Corrects bad aim. |
| **Mandarin Duck** | Splits into 3 ducks (±9°) | Ice ×2.5 | Light (mass 0.9). Wide coverage. |
| **Merganser** | Dive at ≥ 21 u/s | Wood ×2.2 | Pierces wooden structures. |
| **Muscovy Duck** | Ground pound (−22 u/s, mass ×2.5) | Stone ×2.2 | Heaviest duck (mass 4). |
| **Ruddy Shelduck** | Drops an egg bomb and rockets upward | – | Egg: radius 2.2, damage 20. |
| **Black Scoter** | Explodes | – | Radius 3, damage 28. Auto-explodes 0.8 s after its first hit. |

### 4.2 Foxes

| Fox | Health* | Score |
|---|:-:|:-:|
| Kit | 3 | 3,000 |
| Fox | 6 | 5,000 |
| Helmeted fox | 12 | 7,000 |
| Fox King (boss) | 28 | 15,000 |

\* Base value; multiplied by the difficulty curve (§ 6).

## 5. World & levels

| World | Theme | Levels | New duck(s) |
|---|---|:-:|---|
| 1 | Lakeside | 1–5 | Mallard, Mandarin Duck |
| 2 | Autumn Forest | 6–10 | Merganser |
| 3 | Snowy Peaks | 11–15 | Muscovy Duck, Ruddy Shelduck |
| 4 | Sunset Farm | 16–20 | Black Scoter |

Each world introduces a new duck with an intro card. Levels mix materials so the new duck shines. Each
world has its own parallax background, color palette and pixel transition style (dissolve, iris, blinds,
diagonal).

### Materials

| Material | Health | Score | Notes |
|---|:-:|:-:|---|
| Wood | 14 | 500 | All-rounder |
| Stone | 30 | 800 | Heavy, high density |
| Ice | 6 | 300 | Brittle |
| TNT | 4 | 600 | Explodes (radius 2.6, damage 22) and chains |
| Boulder | 30 | 800 | Rolls |

**Damage model:** `impact speed × clamp(√(mass / 2), 0.4, 1.8) × material bonus × upgrade bonus`.
Blocks get a 0.8 s spawn grace period so structures settle safely.

## 6. Difficulty curve

The difficulty factor `d` goes from 0 at level 1 to 1 at level 20 (`d = level index / 19`).

- **Fox health** × (1 + 0.5 · d), reaching ×1.5 at level 20.
- **Block durability** × (1 + 0.3 · d), reaching ×1.3 at level 20.

**Stars** are based on the score:

- **1★:** clear the level.
- **2★:** fox score + 25% of possible destruction + bonus for unused ducks.
- **3★:** fox score + 45% of possible destruction + bonus for unused ducks.

Each unused duck is worth **10,000** points at the end of a level.

## 7. Economy: Golden Eggs 🥚

The foxes stole the eggs, so recovering them *is* the currency.

### Income

| Event | Reward |
|---|---|
| Win | `stars × 10 + round(destruction % × 20)` |
| First clear of a level | `+15` |
| Loss | `round(destruction % × 8)` |

A perfect first clear pays **65 eggs**. Twenty levels pay roughly 1,000 eggs on a first playthrough.

### Spending (per duck species)

| Item | Levels | Effect | Cost |
|---|:-:|---|---|
| Damage | 4 | +15% per level (×1.6 at max). Applies to impacts, explosions and egg bombs. | 40 / 80 / 140 / 220 |
| Auto-Aim | 3 | After 0.25 s of flight and before the first hit, the duck steers toward the nearest fox ahead (within 13 units) at 25° / 45° / 70° per second. | 60 / 120 / 200 |
| Extra Duck | ∞ | Adds one duck to the queue mid-level, or continues the level after losing. | 20–35 |

Fully upgrading one duck costs **1,860** eggs, so players have to make meaningful choices.

## 8. User interface

All UI is pixel-art IMGUI, scaled from a 1080 px reference height.

- **Screens:** Splash → Main menu → Level select → Level intro → HUD → Pause / Result → Shop / Add a duck.
- **HUD:** pause, restart, **+** (extra duck with an owned-count badge), score and best, fox counter,
  remaining duck icons.
- **Accessibility of input:** a big drag area, with ability use on any tap that doesn't land on a UI button.

## 9. Audio

Procedurally generated chiptune SFX: launch, flap, pop, dash, hit, thud, wood/stone/ice breaking,
explosion, fox pop, click, star, slingshot stretch, win, lose and whoosh. Sound can be toggled from the
main menu.

## 10. Technical specification

| Area | Choice |
|---|---|
| Render pipeline | Built-in, orthographic camera, pixel-perfect sprites (16 PPU, point filtering) |
| Physics | Box2D (Rigidbody2D). Continuous collision for ducks. Custom physics materials per material. |
| Level data | Plain C# (`Levels.cs`), built at runtime by `LevelLoader` |
| Persistence | `PlayerPrefs` (stars, best scores, eggs, upgrades, inventory, language) |
| Build | IL2CPP, ARM64, minimum API 26 (Android 8.0), landscape auto-rotate |
| Tooling | `GameBuilder` one-click setup; `AutoTester` plays all 20 levels automatically for stability and winnability checks |

## 11. Quality bar

- **Stable structures:** every level must stand still for 4 s with no shots fired. This is checked by the AutoTester.
- **Winnable:** every level must be beatable with its given ducks and no upgrades.
- **Pixel-perfect UI:** text must stay legible at every resolution, and font sizes are quantized to keep the glyph atlas stable.

## 12. Roadmap

| Version | Goals |
|---|---|
| **1.0** ✅ | 20 levels, 6 ducks, Golden Eggs & shop, 7 languages |
| Next | Background music, more worlds, daily challenges, Google Play release |

---

<div align="center"><sub>© 2026 DuckThisGames. All rights reserved.</sub></div>
