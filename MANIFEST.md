<div align="center">

<img src="docs/media/banner.png" width="100%">

# Game Manifest: DuckThis Feather Fury

*Vision, design pillars and technical specification*

| | |
|---|---|
| **Title** | DuckThis Feather Fury |
| **Developer** | DuckThisGames |
| **Genre** | Physics puzzle, slingshot |
| **Content** | 200 levels · 10 worlds × 20 · 6 ducks |
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

The game has **10 worlds** with **20 levels** each, for **200 levels** in total. Worlds unlock in order: you
can't enter world N+1 before finishing world N. Inside a world, levels unlock one by one.

| World | Theme | Levels | Introduces |
|---|---|:-:|---|
| 1 | Lakeside | 1–20 | Mallard (1), Mandarin Duck (12) |
| 2 | Autumn Forest | 21–40 | Merganser (21) |
| 3 | Snowy Peaks | 41–60 | Muscovy Duck (41), Ruddy Shelduck (47) |
| 4 | Sunset Farm | 61–80 | Black Scoter (61) |
| 5 | Desert Canyon | 81–100 | Golden Nests |
| 6 | Tropical Beach | 101–120 | |
| 7 | Blossom Valley | 121–140 | |
| 8 | Fire Mountain | 141–160 | |
| 9 | Misty Swamp | 161–180 | |
| 10 | Fox Kingdom | 181–200 | Final boss |

**Hand-made and generated levels.** In worlds 1–8, the 40 hand-designed levels sit at fixed slots: levels
1, 4, 7, 12 and 20 of each world. Duck introductions and boss levels are hand-made. All other levels come
from a **seeded procedural generator** (`LevelGen.cs`), so level *N* always has the same layout.

The generator assembles forts from modules that are proven to stand: towers of 1–3 floors, stone
pedestals, a wall with a frame, a boulder shelf, a bridge, twin towers, a pyramid and a boss bunker.
Difficulty (§ 6) controls:
- the number and size of modules
- the share of stone and ice
- the share of helmeted foxes, TNT placement and spare ducks

The duck line-up is weighted toward the counters for that fort: Muscovy for stone, Mandarin for ice,
Merganser for wood and Scoter for bosses and TNT.

**Every one of the 200 levels is checked by the AutoTester**: it must stand still with no shot fired.

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

Difficulty **D** rises by **5 points across each world**. Each new world starts **1 point below** where
the previous one ended, so a new world opens with a short breather:

```
D = 4 × (world − 1) + 5 × (level in world − 1) / 19

World 1:  0 ──────────▶ 5
World 2:       4 ──────────▶ 9
World 3:            8 ──────────▶ 13
  ...
World 10:                                36 ──────────▶ 41
```

| Effect | Formula | D = 0 | D = 20 | D = 41 |
|---|---|:-:|:-:|:-:|
| Fox health | × (1 + 0.02·D) | ×1.00 | ×1.40 | ×1.82 |
| Block durability | × (1 + 0.01·D) | ×1.00 | ×1.20 | ×1.41 |
| Helmeted fox chance | 2% + 1.7%·D | 2% | 36% | 60% |
| Stone share | 8% + 1.4%·D | 8% | 36% | 55% |
| Spare ducks | | 2 | 1 | 2 |

The first level of every world shows a **"World N: foxes are X% tougher!"** banner. Level 20 of every world
is a **boss fortress**. Upgrades (Damage up to ×1.6, Auto-Aim), Golden Nests and extra ducks are there so
committed players can keep pace, while players who skip the shop feel the climb.

**Balancing:** the AutoTester can run with an *expected upgrade profile*: the Damage and Auto-Aim levels a
player could afford with the eggs they have earned by that world. Late worlds are tuned so they stay
winnable under that profile.

### Star gifts 🎁

Each world has **two gift ducks**, at **20★** and **45★** of the world's 60 stars. Claim them on the
level-select screen: each gives one random extra duck from the species you have met. That makes **at most
2 gift ducks per world** (20 in the whole game). A gift icon on the world tab and a line on the level-clear
screen show when one is ready.

### Golden Nests

Golden Nests appear in levels 10 and 20 of every world, in the hand-made boss levels and in some hand-made
levels from world 5 on. Breaking one pays **+3 Golden Eggs**, once per nest, forever.

## 7. Economy: Golden Eggs 🥚

The foxes stole the eggs, so recovering them *is* the currency.

### Income

| Event | Reward |
|---|---|
| Win | `stars × 10 + round(destruction % × 20)` |
| First clear of a level | `+15` |
| Loss | `round(destruction % × 8)` |

A perfect first clear pays **65 eggs**. 200 levels pay roughly 10,000 eggs on a first playthrough, plus about **80** from Golden Nests.

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
- **Pause menu:** resume, **restart level**, levels, main menu, quit.
- **HUD:** pause, restart, **+** (extra duck with an owned-count badge), score and best, fox counter,
  remaining duck icons.
- **Accessibility of input:** a big drag area, with ability use on any tap that doesn't land on a UI button.

## 9. Audio

All audio is procedurally synthesized chiptune, generated by Python scripts and imported by `GameBuilder`.

### Duck voices

Each species has its own **voice** (on launch), **ability sound** and **hit squawk**:

| Duck | Voice | Ability |
|---|---|---|
| Mallard | Classic two-note "QUACK-quack" (sawtooth + formant filter) | Wing flutter |
| Mandarin Duck | High, bird-like whistle trill | Sparkly arpeggio (split) |
| Merganser | Low, gravelly "krrr-ak" | Rising jet whoosh |
| Muscovy Duck | Hiss and grunt (real Muscovies hiss) | Falling whistle and heavy thump |
| Ruddy Shelduck | Nasal, goose-like "ang-ang" honk | Egg "plop" and spring "boing" |
| Black Scoter | Melodic, descending whistle (real scoters whistle) | Fuse hiss, then the explosion |

### Music

Three seamless loops: square-wave lead, arpeggio, triangle bass and noise drums.

| Track | Key / tempo | Where it plays |
|---|---|---|
| `music_menu` | C major, 112 BPM | Menus, level select, shop |
| `music_world` | G major, 128 BPM | Worlds 1–5 |
| `music_world2` | A minor, 140 BPM | Worlds 6–10 (more driving) |

Tracks cross-fade when you move between menus and levels. The music ducks while the game is paused or on
the lose screen. Music and sound effects have separate toggles in the main menu.

### Sound effects

Launch, slingshot stretch, hits, wood/stone/ice breaking, explosions, fox pop, stars, win/lose and
transitions.

## 10. Technical specification

| Area | Choice |
|---|---|
| Render pipeline | Built-in, orthographic camera, pixel-perfect sprites (16 PPU, point filtering) |
| Physics | Box2D (Rigidbody2D). Continuous collision for ducks. Custom physics materials per material. |
| Level data | Plain C# (`Levels.cs`), built at runtime by `LevelLoader` |
| Persistence | `PlayerPrefs` (stars, best scores, eggs, upgrades, inventory, language) |
| Build | IL2CPP, ARM64, minimum API 26 (Android 8.0), landscape auto-rotate |
| Tooling | `GameBuilder` one-click setup; `AutoTester` checks all 200 levels automatically (stability mode and play mode) for stability and winnability checks |

## 11. Quality bar

- **Stable structures:** every level must stand still for 4 s with no shots fired. This is checked by the AutoTester.
- **Winnable:** every level must be beatable with its given ducks and no upgrades.
- **Pixel-perfect UI:** text must stay legible at every resolution, and font sizes are quantized to keep the glyph atlas stable.

## 12. Accounts & purchases (planned)

These are **not in the game yet**; this section is the agreed direction so the code can grow toward it.

### Accounts

- **Playing never requires an account.** Every player starts as a **guest**.
- Players can **optionally** sign in later with **Google** (Google Play Games Services on Android) or **email**
  (e.g. Firebase Auth or Unity Authentication).
- Signing in **links** the guest profile, so the player keeps all progress. Progress then syncs to the cloud
  for backup and device changes.
- **Merge rule** when a cloud save already exists: the higher value always wins (stars, best score, eggs,
  upgrades, collected nests), so nobody loses progress.
- **Already in the code:** `PlayerProfile.cs` gives every install a stable guest `PlayerId`. It has an
  account type (Guest / Google / Email) and `Export()` / `Import()` methods that serialize the whole save
  (stars, best scores, eggs, upgrades, inventory, nests) to JSON with the merge rule above.
- The login UI and backend are still to come.

### In-app purchases

- **Golden Eggs stay the single currency.** You earn them by playing, and later you'll also be able to buy
  them in packs. A second "premium" currency would add complexity without value at this scale.
- **Candidates:**
  - Egg packs (small / medium / large).
  - A one-time **Starter Pack** with eggs and extra ducks.
  - **Remove ads**, if ads are ever added.
- **Fairness:** every level stays winnable without paying. Purchases speed things up; they don't gate content.
- **Tech:**
  - Unity IAP / Google Play Billing.
  - Server-side receipt validation.
  - Purchases tied to the account, so they restore on a new device. Guests are prompted to sign in before
    buying.
- **Compliance:** privacy policy, KVKK / GDPR consent, Google Play Families and age rating, and clear prices.

## 13. Roadmap

| Version | Goals |
|---|---|
| **1.0** ✅ | 200 levels in 10 worlds, stepped difficulty curve, 6 ducks with unique voices, Golden Eggs & Nests, shop, background music, 7 languages |
| Next | Optional Google / email accounts with cloud save, then in-app egg packs; daily challenges; Google Play release |

---

<div align="center"><sub>© 2026 DuckThisGames. All rights reserved.</sub></div>
