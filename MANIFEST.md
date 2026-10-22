<div align="center">

<img src="docs/media/banner.png" width="100%">

# Game Manifest: DuckThis Feather Fury

*Vision, design pillars and technical specification*

| | |
|---|---|
| **Title** | DuckThis Feather Fury |
| **Developer** | DuckThisGames |
| **Genre** | Physics puzzle, slingshot |
| **Content** | 500 levels · 25 worlds × 20 · 9 ducks |
| **Platform** | Android (phone, landscape) |
| **Engine** | Unity 6000.6.6f1 |
| **Version** | 1.0 |
| **Package** | `com.duckthisgames.featherfury` |
| **Art style** | 16 px pixel art |
| **Languages** | 30 (EN, TR, DE, ES, FR, PT, IT, PL, ID, FIL, SW, HA, VI, RU, UK, ZH-Hans, ZH-Hant, JA, KO, HI, BN, MR, TE, TA, GU, PA, TH, AR, UR, FA) |

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
| **Common Merganser** | Dive at ≥ 21 u/s | Wood ×2.2 | Pierces wooden structures. |
| **Muscovy Duck** | Ground pound (−22 u/s, mass ×2.5) | Stone ×2.2 | Heaviest duck (mass 4). |
| **Ruddy Shelduck** | Drops an egg bomb and rockets upward | – | Egg: radius 2.2, damage 20. |
| **Common Scoter** | Explodes | – | Radius 3, damage 28. Auto-explodes 0.8 s after its first hit. |
| **Northern Pintail** | Boomerang: turns back and hits from behind | Wood ×1.4, Fox ×1.2 | For targets hidden behind walls. |
| **Common Eider** | Shockwave (radius 3.8) that shoves blocks and foxes | Stone ×1.5 | Heavy (mass 3.5). |
| **Common Goldeneye** | Locks on to the nearest fox and dives (≥ 20 u/s) | Fox ×1.8 | Fox hunter. |

You start with **Mallard, Mandarin Duck and Common Merganser**. A new species joins every three worlds:
Muscovy (world 4), Ruddy Shelduck (7), Common Scoter (10), Northern Pintail (13), Common Eider (16) and
Common Goldeneye (19). Each species is introduced with its own card on the first level of that world.
After its first hit a duck gets light drag and its upward rebound is capped, so it no longer bounces
unpredictably.

### 4.2 Foxes

| Fox | Health* | Score |
|---|:-:|:-:|
| Kit | 3 | 3,000 |
| Fox | 6 | 5,000 |
| Helmeted fox | 12 | 7,000 |
| Fox King (boss) | 28 | 15,000 |

\* Base value; multiplied by the difficulty curve (§ 6).

## 5. World & levels

The game has **25 worlds** with **20 levels** each, for **500 levels** in total. Worlds unlock in order: you
can't enter world N+1 before finishing world N. Inside a world, levels unlock one by one. Every world has
its own name and theme:

| | | | | |
|---|---|---|---|---|
| 1 Lakeside | 2 Autumn Forest | 3 Snowy Peaks | 4 Sunset Farm | 5 Desert Canyon |
| 6 Tropical Beach | 7 Blossom Valley | 8 Fire Mountain | 9 Misty Swamp | 10 Fox Kingdom |
| 11 Moonlit Lake | 12 Crimson Woods | 13 Frozen Night | 14 Golden Harvest | 15 Starry Dunes |
| 16 Coral Cove | 17 Lantern Night | 18 Ember Wastes | 19 Stormy Bog | 20 Dawn Citadel |
| 21 Thunder Peaks | 22 Mirage Oasis | 23 Sunken Lagoon | 24 Twilight Grove | 25 Fox Throne |

Worlds 11–25 are night, crimson, golden, storm, dusk and dawn variants of the first ten landscapes.

**Hand-made and generated levels.** The first 40 levels (worlds 1–2) are hand-designed and named. All
other levels come from a **seeded procedural generator** (`LevelGen.cs`), so level *N* always has the same
layout. Ducks that are not yet unlocked are swapped for an available counter-duck (with one spare).

The generator assembles forts from modules that are proven to stand: towers of 1–3 floors, stone
pedestals, a wall with a frame, a boulder shelf, a bridge, twin towers, a pyramid and a boss bunker.
Difficulty (§ 6) controls the number and size of modules, the share of stone and ice, helmeted foxes, TNT
placement and spare ducks. The fox count follows a wave: boss levels ×1.3, every 5th level ×0.7
(a breather), the rest 0.75–1.3.

**Every one of the 500 levels is checked by the AutoTester**: it must stand still with no shot fired.

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

Each level has a difficulty percentage, shown with a meter on the level's intro card:

```
P(n) = n                              for n ≤ 15      (level 1 = 1%, level 15 = 15%)
P(n) = 15 + (n − 15) · 60 / 85        for n ≤ 100     (level 100 = 75%)
P(n) = 75 + 25 · (1 − e^(−(n−100)/150))  after that   (level 200 ≈ 87%, 300 ≈ 93%, 500 ≈ 98%)
```

Internally D = 0.6 · P drives:

| Effect | Formula | Level 1 | Level 100 | Level 500 |
|---|---|:-:|:-:|:-:|
| Fox health | × (1 + 0.02·D) | ×1.01 | ×1.90 | ×2.18 |
| Block durability | × (1 + 0.01·D) | ×1.01 | ×1.45 | ×1.59 |
| Structure modules | 1 → 5 | 1 | 3–5 | 3–5 |

Fox counts go up and down from level to level, but the difficulty itself never drops. Level 20 of every
world is a **boss fortress**. World 1 gets one spare Mallard on every level so new players settle in.

**Balancing:** the AutoTester plays every level with an *expected upgrade profile* (the Damage and
Auto-Aim levels a player could afford by that world). With a random-aim bot: world 1–2 ≈ 60% wins,
level 100 ≈ 30%, level 400 ≈ 45% (more species and upgrades). A human aims far better than the bot.

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
| Win | **10 eggs per 100 damage dealt** (damage points = HP removed × 5) |
| First clear of a level | `+20` |
| Loss | half of the damage reward |
| Golden Nest (once per nest) | `+25` |

A typical early level pays 30–90 eggs, a late level 100–200.

### Spending (per duck species)

| Item | Levels | Effect | Cost |
|---|:-:|---|---|
| Damage | 4 | +15% per level (×1.6 at max). Applies to impacts, explosions and egg bombs. | 250 / 500 / 900 / 1500 |
| Auto-Aim | 3 | After 0.25 s of flight and before the first hit, the duck steers toward the nearest fox ahead (within 13 units) at 25° / 45° / 70° per second. | 400 / 800 / 1400 |
| Extra Duck | ∞ | Adds one duck to the queue mid-level, or continues the level after losing. | Mallard 300, Mandarin 350, Merganser 400, Muscovy 500, Shelduck 550, Scoter 650, Pintail 750, Eider 850, Goldeneye 1000 |

An extra duck costs roughly **10–15 levels** of income, and fully upgrading one duck costs **5,750** eggs, so
the shop is a long-term goal. Saves from the old economy are converted once (eggs ×10, level progress reset
for the new 500-level layout; upgrades and extra ducks are kept).

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
| Tooling | `GameBuilder` one-click setup; `AutoTester` checks all 500 levels automatically (stability mode and play mode) for stability and winnability checks |

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
| **1.0** ✅ | 500 levels in 25 worlds, continuous difficulty curve, 9 ducks with unique voices, damage-based Golden Egg economy, shop, background music, 30 languages, labelled buttons |
| Next | Optional Google / email accounts with cloud save, then in-app egg packs; daily challenges; Google Play release |

---

<div align="center"><sub>© 2026 DuckThisGames. All rights reserved.</sub></div>
