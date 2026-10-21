<div align="center">

<img src="docs/media/banner.png" alt="DuckThis Feather Fury" width="100%">

# DuckThis Feather Fury

**A pixel-art physics slingshot game for Android, starring six kinds of ducks.**<br>
*The foxes stole the eggs. Time to take them back.*

[![Unity](https://img.shields.io/badge/Unity-6000.6.6f1-222?logo=unity&logoColor=white)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Android-3DDC84?logo=android&logoColor=white)](#-download)
[![Version](https://img.shields.io/badge/Version-1.0-f0a020)](CHANGELOG.md)
[![Levels](https://img.shields.io/badge/Levels-200-d63c2e)](#-worlds)
[![Languages](https://img.shields.io/badge/Languages-7-4a90e2)](#-localization)

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

**🦆 6 duck species**<br>
Six ducks with unique abilities: flap, split, dive, ground-pound, egg-bomb and explode.

**🥚 Golden Egg economy**<br>
Rewards based on stars and destruction, plus a first-clear bonus.

</td>
<td width="50%" valign="top">

**🛒 Shop and upgrades**<br>
Damage (+15% per level), **Auto-Aim** (the duck homes in on foxes) and extra ducks for each species.

**🌍 200 levels, 10 worlds**<br>
Each world has 20 levels and its own theme. Difficulty climbs inside every world and steps up from world to world, and each world ends with a boss fortress. Worlds unlock in order.

**🎵 Music & duck voices**<br>
Three chiptune soundtracks, plus a unique voice, ability sound and squawk for every duck species.

**🌐 7 languages**<br>
Turkish, English, German, Spanish, French, Portuguese and Italian.

</td>
</tr>
</table>

## 🦆 Meet the Ducks

| | Duck | Ability (tap mid-flight) | Strong against | Unlocks |
|:-:|---|---|:-:|:-:|
| <img src="docs/media/ducks/duck.png" width="72"> | **Mallard** | Flaps upward, up to twice | – | Level 1 |
| <img src="docs/media/ducks/mandarin.png" width="72"> | **Mandarin Duck** | Splits into three | 🧊 Ice | Level 4 |
| <img src="docs/media/ducks/merganser.png" width="72"> | **Merganser** | Arrow-fast dive | 🪵 Wood | Level 6 |
| <img src="docs/media/ducks/muscovy.png" width="72"> | **Muscovy Duck** | Heavy ground pound | 🪨 Stone | Level 11 |
| <img src="docs/media/ducks/shelduck.png" width="72"> | **Ruddy Shelduck** | Drops an egg bomb and shoots back up | 💥 Splash | Level 13 |
| <img src="docs/media/ducks/scoter.png" width="72"> | **Black Scoter** | Kaboom! Explodes on demand | 💣 Everything | Level 16 |

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
| Each star earned | **+10** |
| Destruction rate (0–100%) | **up to +20** |
| First time clearing a level | **+15** |
| Breaking a **Golden Nest** 🪺 (once per nest) | **+3** |

### 🎁 Star gifts

Each world hides **two gift ducks**: one at **20★** and one at **45★** of the world's 60 stars. Claim them on
the level-select screen. Each gift is a random extra duck from the species you have met, so you get at
most **1–2 free ducks per world**.
| Losing a level | up to +8 (consolation) |

| Upgrade (per duck) | Levels | Effect | Cost |
|---|:-:|---|---|
| **Damage** | 4 | +15% damage per level, including explosions and egg bombs | 40 · 80 · 140 · 220 |
| **Auto-Aim** | 3 | Steers toward the nearest fox in flight (25° / 45° / 70° per second) | 60 · 120 · 200 |
| **Extra Duck** | ∞ | Add a duck mid-level with **+**, or continue after losing | 20 – 35 |

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

### 📈 Difficulty curve

The game has **10 worlds × 20 levels**. A world must be finished before the next one opens.

Difficulty (**D**) rises by 5 points across each world. The next world starts a little below where the
previous one ended, which gives you a breather:

| World | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 |
|---|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|
| D (first → last level) | 0 → 5 | 4 → 9 | 8 → 13 | 12 → 17 | 16 → 21 | 20 → 25 | 24 → 29 | 28 → 33 | 32 → 37 | 36 → 41 |

D controls fox health (×1 + 0.02·D, up to ×1.8) and block durability (×1 + 0.01·D, up to ×1.4). It also
shapes each level: more structures and floors, more stone, more helmeted foxes and fewer spare ducks.
Level 20 of every world is a **boss fortress**. Upgrades and extra ducks are tuned to keep up with the
curve. See the [Game Manifest](MANIFEST.md#6-difficulty-curve) for details.

## 📸 Screenshots

| | |
|:-:|:-:|
| <img src="docs/media/splash.png"><br>Splash | <img src="docs/media/menu.png"><br>Main menu |
| <img src="docs/media/level-select.png"><br>Level select | <img src="docs/media/new-duck.png"><br>New duck introduction |
| <img src="docs/media/level-clear.png"><br>Level clear, with Golden Egg reward | <img src="docs/media/shop.png"><br>Shop |
| <img src="docs/media/tier-intro.png"><br>New tier banner & Golden Nest tip | <img src="docs/media/add-duck.png"><br>Add a duck mid-level |
| <img src="docs/media/pause.png"><br>Pause menu with *Restart level* | <img src="docs/media/level-select.png"><br>10 worlds, 20 levels each, with star gifts |

## 📥 Download

The latest Android build is published on the [**Releases**](../../releases) page.

1. Download `DuckThis_FeatherFury_v1.0.apk` to your Android phone (Android 8.0 or newer, 64-bit ARM).
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
│   └── GameArt.asset     sprite & sound references (generated)
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
    ├── Loc.cs            localization (7 languages)
    ├── Music.cs          background music with cross-fades
    ├── PlayerProfile.cs  guest profile & save export / import (groundwork for accounts)
    └── …                 Slingshot, CameraRig, Fx, Sfx, Explosion, EggBomb, AutoTester
```

Want to add a level? Levels are plain C# in `Levels.cs`, built with small helpers: `Frame`, `Beam`, `Col`,
`Cube`, `Tnt`, `Boulder` and `Fox(kind, x, y)`. See the [Game Manifest](MANIFEST.md) for the full design.

## 🌐 Localization

🇹🇷 Türkçe · 🇬🇧 English · 🇩🇪 Deutsch · 🇪🇸 Español · 🇫🇷 Français · 🇵🇹 Português · 🇮🇹 Italiano

The device language is detected automatically. You can switch languages from the main menu at any time.

## 🗺 Roadmap

- [x] 200 levels in 10 worlds with a stepped difficulty curve
- [x] Star gifts: 2 gift ducks per world
- [x] 6 ducks with unique voices, 4 fox types
- [x] Golden Egg economy, Golden Nests, shop and upgrades
- [x] Background music (3 tracks)
- [x] 7 languages
- [ ] Optional accounts (Google / email) with cloud save; playing as a guest stays possible
- [ ] In-app Golden Egg packs (see the [plan](MANIFEST.md#12-accounts--purchases-planned))
- [ ] Google Play release

## 🇹🇷 Türkçe

**DuckThis Feather Fury** is a pixel-art, physics-based slingshot game for Android starring ducks.

You face the foxes' wood, stone, ice and TNT forts across 200 levels and 10 worlds, with 6 duck species that
each have their own ability. At the end of each level you earn **Golden Eggs** based on your stars and
destruction. In the shop you can spend them on **Damage**, **Auto-Aim** and **Extra Duck** for each species.

To open the project, use Unity 6000.6.6f1 and add the folder in Unity Hub. On first load everything is set
up automatically.

---

<div align="center">

Made with 🦆 by **DuckThisGames**<br>
<sub>© 2026 DuckThisGames. All rights reserved.</sub>

</div>
