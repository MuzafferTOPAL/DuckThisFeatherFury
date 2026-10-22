<div align="center">

<img src="docs/media/banner.png" alt="DuckThis Feather Fury" width="100%">

# DuckThis Feather Fury

**A pixel-art physics slingshot game for Android, starring nine kinds of ducks.**<br>
*The foxes stole the eggs. Time to take them back.*

[![Unity](https://img.shields.io/badge/Unity-6000.6.6f1-222?logo=unity&logoColor=white)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Android-3DDC84?logo=android&logoColor=white)](#-download)
[![Version](https://img.shields.io/badge/Version-1.0-f0a020)](CHANGELOG.md)
[![Levels](https://img.shields.io/badge/Levels-500-d63c2e)](#-worlds)
[![Languages](https://img.shields.io/badge/Languages-30-4a90e2)](#-localization)

[**Download**](#-download) · [**Game Manifest**](MANIFEST.md) · [**Changelog**](CHANGELOG.md) · [**Build from source**](#-build-from-source)

<br>

<img src="docs/media/gameplay-tnt.gif" alt="Gameplay: a duck hits a TNT crate and the whole fox fort explodes" width="85%">

</div>

---

## 🦆 About

**DuckThis Feather Fury** is a 2D physics game in the spirit of classic slingshot games, drawn in hand-made
16-pixel art.

1. Pull back the slingshot and launch a duck.
2. Topple the foxes' forts of **wood, stone, ice and TNT**.
3. Earn **Golden Eggs** at the end of each level.
4. Spend them in the shop to make your ducks stronger.

Each duck species has its own ability, triggered by tapping mid-flight. Picking the right duck for the right
material is the key to three stars.

## ✨ Features

<table>
<tr>
<td width="50%" valign="top">

**🎯 Physics-based gameplay**<br>
Real rigid-body physics. Damage depends on impact speed, mass and material. TNT crates trigger chain explosions.

**🦆 9 duck species**<br>
Nine ducks with unique abilities: flap, split, dive, ground-pound, explode, egg-bomb, boomerang, shockwave and fox-seeking dive. You start with three; a new species joins every three worlds.

**🥚 Golden Egg economy**<br>
Every level pays out by the damage you deal: **10 Golden Eggs per 100 damage**, plus a first-clear bonus.

</td>
<td width="50%" valign="top">

**🛒 Shop and upgrades**<br>
Damage (+15% per level), **Auto-Aim** (the duck homes in on foxes) and extra ducks for each species.

**🌍 500 levels, 25 worlds**<br>
Each world has 20 levels, its own name and theme. Difficulty climbs from **1%** on level 1 to **15%** on level 15 and **75%** on level 100, then keeps creeping up. Fox counts rise and fall from level to level, and each world ends with a boss fortress.

**🎵 Music & duck voices**<br>
Three chiptune soundtracks, plus a unique voice, ability sound and squawk for every duck species.

**🌐 30 languages**<br>
The 30 most spoken languages, including Chinese, Japanese, Korean, Hindi, Bengali, Arabic, Urdu and Persian (right-to-left), with real common duck names in every language.

</td>
</tr>
</table>

## 🦆 Meet the Ducks

| | Duck | Ability (tap mid-flight) | Strong against | Unlocks |
|:-:|---|---|:-:|:-:|
| <img src="docs/media/ducks/duck.png" width="72"> | **Mallard** | Flaps upward, up to twice | – | Level 1 |
| <img src="docs/media/ducks/mandarin.png" width="72"> | **Mandarin Duck** | Splits into three | 🧊 Ice | Level 1 |
| <img src="docs/media/ducks/merganser.png" width="72"> | **Common Merganser** | Arrow-fast dive | 🪵 Wood | Level 1 |
| <img src="docs/media/ducks/muscovy.png" width="72"> | **Muscovy Duck** | Heavy ground pound | 🪨 Stone | World 4 (level 61) |
| <img src="docs/media/ducks/shelduck.png" width="72"> | **Ruddy Shelduck** | Drops an egg bomb and shoots back up | 💥 Splash | World 7 (level 121) |
| <img src="docs/media/ducks/scoter.png" width="72"> | **Common Scoter** | Kaboom! Explodes on demand | 💣 Everything | World 10 (level 181) |
| <img src="docs/media/ducks/pintail.png" width="72"> | **Northern Pintail** | Boomerang: turns back and hits from behind | 🪵 Wood | World 13 (level 241) |
| <img src="docs/media/ducks/eider.png" width="72"> | **Common Eider** | Shockwave that pushes everything away | 🪨 Stone | World 16 (level 301) |
| <img src="docs/media/ducks/goldeneye.png" width="72"> | **Common Goldeneye** | Locks on to the nearest fox | 🦊 Foxes | World 19 (level 361) |

<div align="center">
<img src="docs/media/gameplay-split.gif" alt="A Mandarin Duck splits into three and shatters an ice tower" width="70%">
<br><sub>The Mandarin Duck splits into three and shatters an ice palace.</sub>
</div>

## 🦊 The Foxes

<img src="docs/media/ducks/fox_idle.png" width="64" align="left">
<img src="docs/media/ducks/foxh_idle.png" width="64" align="left">

There are four kinds of foxes: **kits** (small and fragile), **regular foxes**, **helmeted foxes** (tough)
and the **Fox King** (huge). They blink, panic when a duck is in the air, and flash when hurt.

<br clear="left">

## 🥚 Golden Eggs & Shop

<img src="docs/media/ducks/ui_egg.png" width="48" align="right">

| Source | Golden Eggs |
|---|---|
| Damage dealt in a level | **10 per 100 damage** |
| First time clearing a level | **+20** |
| Losing a level | half of the damage reward |
| Breaking a **Golden Nest** 🪺 (once per nest) | **+25** |

### 🎁 Star gifts

Each world hides **two gift ducks**: one at **20★** and one at **45★** of the world's 60 stars. Claim them on
the level-select screen. Each gift is a random extra duck from the species you have met, so you get at
most **1–2 free ducks per world**.

| Upgrade (per duck) | Levels | Effect | Cost |
|---|:-:|---|---|
| **Damage** | 4 | +15% damage per level, including explosions and egg bombs | 250 · 500 · 900 · 1500 |
| **Auto-Aim** | 3 | Steers toward the nearest fox in flight (25° / 45° / 70° per second) | 400 · 800 · 1400 |
| **Extra Duck** | ∞ | Add a duck mid-level with **+**, or continue after losing. About one every 10–15 levels | 300 – 1000 by species |

<div align="center">
<img src="docs/media/shop.png" width="49%"> <img src="docs/media/add-duck.png" width="49%">
</div>

## 🌍 Worlds

| | |
|:-:|:-:|
| <img src="docs/media/world-level3.png"> **1 · Lakeside** | <img src="docs/media/world-level8.png"> **2 · Autumn Forest** |
| <img src="docs/media/world-level14.png"> **3 · Snowy Peaks** | <img src="docs/media/world-level19.png"> **4 · Sunset Farm** |
| <img src="docs/media/world-level23.png"> **5 · Desert Canyon** | <img src="docs/media/world-level29.png"> **6 · Tropical Beach** |
| <img src="docs/media/world-level31.png"> **7 · Blossom Valley** | <img src="docs/media/world-level40.png"> **8 · Fire Mountain** |
| <img src="docs/media/world-swamp.png"> **9 · Misty Swamp** | <img src="docs/media/world-kingdom.png"> **10 · Fox Kingdom** |

Worlds 11–25 bring night, crimson, golden-harvest, storm and dawn versions of these landscapes:
Moonlit Lake, Crimson Woods, Frozen Night, Golden Harvest, Starry Dunes, Coral Cove, Lantern Night,
Ember Wastes, Stormy Bog, Dawn Citadel, Thunder Peaks, Mirage Oasis, Sunken Lagoon, Twilight Grove and the
Fox Throne.

### 📈 Difficulty curve

The game has **25 worlds × 20 levels = 500 levels**. A world must be finished before the next one opens.
Every level shows its difficulty on the intro card:

| Level | 1 | 15 | 50 | 100 | 200 | 300 | 500 |
|---|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
| Difficulty | 1% | 15% | 40% | 75% | 87% | 93% | 98% |

Difficulty controls fox health and block durability. It also shapes each level: more structures and
floors, more stone, more helmeted foxes and fewer spare ducks. The number of foxes rises and falls from
level to level (every 5th level is a lighter "breather"), but the overall difficulty keeps climbing.
Level 20 of every world is a **boss fortress**. See the [Game Manifest](MANIFEST.md#6-difficulty-curve).

## 📸 Screenshots

| | |
|:-:|:-:|
| <img src="docs/media/splash.png"><br>Splash | <img src="docs/media/menu.png"><br>Main menu |
| <img src="docs/media/level-select.png"><br>Level select | <img src="docs/media/new-duck.png"><br>New duck introduction |
| <img src="docs/media/level-clear.png"><br>Level clear, with Golden Egg reward | <img src="docs/media/shop.png"><br>Shop |
| <img src="docs/media/tier-intro.png"><br>New tier banner & Golden Nest tip | <img src="docs/media/add-duck.png"><br>Add a duck mid-level |
| <img src="docs/media/pause.png"><br>Pause menu with *Restart level* | <img src="docs/media/level-select.png"><br>25 worlds, 20 levels each, with star gifts |
| <img src="docs/media/lang/langs.png"><br>30 languages | <img src="docs/media/lang/m_hi.png"><br>Hindi |
| <img src="docs/media/lang/m_ar.png"><br>Arabic (right-to-left) | <img src="docs/media/lang/pause_ja.png"><br>Japanese |
| <img src="docs/media/lang/intro4.png"><br>Difficulty meter and new-duck card | <img src="docs/media/lang/end_ko.png"><br>Korean: damage → Golden Eggs, labelled buttons |

## 📥 Download

The latest Android build is published on the [**Releases**](../../releases) page.

1. Download `DuckThis_FeatherFury_v1.0.apk` to your Android phone (Android 8.0 or newer, 32- or 64-bit ARM).
2. Open it. If prompted, allow *Install unknown apps* for your browser or file manager.
3. Play!

## 🛠 Build from source

### Requirements

- **Unity 6000.6.6f1**
- The **Android Build Support** module (OpenJDK, Android SDK & NDK), installed from Unity Hub.

### Steps

```bash
git clone https://github.com/MuzafferTOPAL/DuckThisFeatherFury.git
```

1. In Unity Hub, choose **Add → Add project from disk** and select the folder.
2. On first load, `Assets/Editor/GameBuilder.cs` sets everything up automatically:
   - sprite import settings
   - the `GameArt` asset
   - the `Game` scene
   - player settings
3. Open `Assets/Scenes/Game.unity` and press **▶ Play**.
4. To build the Android APK, use **Ördek Oyunu → Android APK Al**. The APK is written to
   `Builds/DuckThis_FeatherFury.apk` (IL2CPP, ARM64).

### Editor tools

| Menu item | What it does |
|---|---|
| Oyunu Yeniden Kur | Rebuild imports, the art asset, the scene and player settings |
| Bölümleri Otomatik Test Et | Play every level automatically; writes `Logs/autotest.txt` |
| Tüm Bölümlerin Kilidini Aç | Unlock all levels |
| İlerlemeyi Sıfırla | Reset progress, eggs and upgrades |
| Android APK Al | Build the Android APK |

## 🗂 Project structure

```
Assets/
├── Art/
│   ├── Pixel/        sprites (16 px = 1 world unit, point filtered)
│   ├── Sounds/       chiptune sound effects
│   └── Fonts/        Jersey 10
├── Editor/
│   └── GameBuilder.cs    one-click setup, editor menu, APK build
├── Resources/
│   ├── GameArt.asset     sprite & sound references (generated)
│   ├── Loc/              one JSON string table per language
│   └── LocAtlas/         pre-shaped text images for non-Latin scripts
tools/loc/                translation tables + build.py (writes Loc/ and LocAtlas/)
├── Scenes/
│   └── Game.unity        single scene, levels are built at runtime
└── Scripts/
    ├── GameManager.cs    game flow: splash, menu, levels, win / lose
    ├── Hud.cs            pixel-art UI, shop, transitions
    ├── Economy.cs        Golden Eggs, upgrades, extra ducks
    ├── Bird.cs           duck physics, abilities, auto-aim
    ├── BirdType.cs       duck stats & material multipliers
    ├── Fox.cs            enemies
    ├── Destructible.cs   damage model for blocks & foxes
    ├── Levels.cs         world/level layout, 40 hand-made levels
    ├── LevelGen.cs       procedural level generator (seeded, stability-tested)
    ├── LevelLoader.cs    level & theme building, difficulty scaling
    ├── Loc.cs            localization (30 languages, device-language detection)
    ├── LocText.cs        draws scripts the pixel font lacks from pre-rendered text atlases
    ├── Music.cs          background music with cross-fades
    ├── PlayerProfile.cs  guest profile & save export / import (groundwork for accounts)
    └── …                 Slingshot, CameraRig, Fx, Sfx, Explosion, EggBomb, AutoTester
```

Want to add a level? Levels are plain C# in `Levels.cs`, built with small helpers: `Frame`, `Beam`, `Col`,
`Cube`, `Tnt`, `Boulder` and `Fox(kind, x, y)`. See the [Game Manifest](MANIFEST.md) for the full design.

## 🌐 Localization

English · Türkçe · Deutsch · Español · Français · Português · Italiano · Polski · Bahasa Indonesia ·
Filipino · Kiswahili · Hausa · Tiếng Việt · Русский · Українська · 简体中文 · 繁體中文 · 日本語 · 한국어 · हिन्दी ·
বাংলা · मराठी · తెలుగు · தமிழ் · ગુજરાતી · ਪੰਜਾਬੀ · ไทย · العربية · اردو · فارسی

The device language is detected automatically, and the language button on the main menu opens a picker
with all 30. Latin text uses the Jersey 10 pixel font. Scripts it does not cover (Cyrillic, Vietnamese,
CJK, Indic, Thai and Arabic-script) are shaped with HarfBuzz at build time and drawn from text atlases, so
conjuncts and right-to-left text render correctly on every phone. To change a translation, edit
`tools/loc/l_*.py` and run `python3 tools/loc/build.py Assets`.

## 🗺 Roadmap

- [x] 500 levels in 25 named worlds with a continuous difficulty curve (1% → 75% → 90%+)
- [x] Star gifts: 2 gift ducks per world
- [x] 9 ducks with unique voices, 4 fox types
- [x] Golden Egg economy, Golden Nests, shop and upgrades
- [x] Background music (3 tracks)
- [x] 30 languages, incl. CJK, Indic and right-to-left scripts
- [ ] Optional accounts (Google / email) with cloud save; playing as a guest stays possible
- [ ] In-app Golden Egg packs (see the [plan](MANIFEST.md#12-accounts--purchases-planned))
- [ ] Google Play release

## 🇹🇷 Türkçe

**DuckThis Feather Fury** is a pixel-art, physics-based slingshot game for Android starring ducks.

You face the foxes' wood, stone, ice and TNT forts across 500 levels and 25 worlds, with 9 duck species that
each have their own ability. At the end of each level you earn **Golden Eggs** based on the damage you dealt
(100 damage = 10 eggs). In the shop you can spend them on **Damage**, **Auto-Aim** and **Extra Duck** for each species.

To open the project, use Unity 6000.6.6f1 and add the folder in Unity Hub. On first load everything is set
up automatically.

---

<div align="center">

Made with 🦆 by **DuckThisGames**<br>
<sub>© 2026 DuckThisGames. All rights reserved.</sub>

</div>
