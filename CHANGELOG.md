# Changelog

## 1.0

### Compatibility & performance

- **Runs on 32-bit phones too:** the APK now contains ARMv7 and ARM64 code (many budget / Android Go
  phones, including some Android 12 devices, are 32-bit only). Minimum Android 8.0 (API 26, the oldest
  Unity 6.6 supports), target = latest SDK.
- Graphics: OpenGL ES 3 first, Vulkan as fallback (avoids buggy Vulkan drivers on cheap devices).
- Performance: resolution capped at 1080p (720p on phones with < 3 GB RAM or ≤ 4 cores), vSync off at
  60 fps with optimized frame pacing, UI text drawn only in the repaint pass, GUILayout pass skipped,
  text atlases stored as 16-bit textures without a CPU copy (half the memory).
- **Language review:** every language was reviewed and scored (grammar, local usage, duck names);
  70 fixes applied. Report: `tools/loc/review/REPORT.md`.


### 500 levels, 9 ducks, 30 languages

- **25 worlds × 20 levels = 500 levels**, each world with its own name and theme (15 new worlds, from
  Moonlit Lake to the Fox Throne).
- **New difficulty curve:** 1% on level 1, 15% on level 15, 75% on level 100, ~98% on level 500. Shown with
  a meter on every level's intro card. Fox counts rise and fall, difficulty keeps climbing.
- **3 new ducks:** Northern Pintail (boomerang), Common Eider (shockwave) and Common Goldeneye (fox-seeking
  dive), each with its own voice and sounds. You start with 3 species; a new one joins every 3 worlds.
- **30 languages**, including Chinese, Japanese, Korean, Hindi, Bengali, Marathi, Telugu, Tamil, Gujarati,
  Punjabi, Thai, Arabic, Urdu and Persian. Non-Latin scripts are shaped at build time and drawn from text
  atlases (correct conjuncts and right-to-left order). New language picker; device language detected.
- **Correct duck names** in every language (e.g. Turkish *Yeşilbaş Ördek*, *Büyük Tarakdiş*, *Angut*).
- **Labelled buttons:** pause, restart, add duck, levels, next, home, back, sound and music now say what
  they do.
- **New economy:** 10 Golden Eggs per 100 damage, +20 first clear, half pay on a loss. Upgrades and extra
  ducks cost more (extra duck 300–1000 by species). Old saves are converted once.
- **Slingshot:** pull from anywhere on the left side of the screen (your finger no longer hides the duck),
  longer aim preview that turns orange at full power, small pulls cancel.
- **Fix:** ducks bounced unpredictably after a hit. They now get light drag after the first impact and
  their upward rebound is capped. Grazing the ground at launch no longer counts as a hit.
- First world is more forgiving (one spare Mallard per level).
- All 500 levels pass the stability test.


### 200 levels

- **10 worlds × 20 levels = 200 levels.** Worlds unlock in order. New worlds: Misty Swamp and Fox Kingdom.
- **New difficulty curve.** D goes 0 → 5 in world 1, 4 → 9 in world 2, and so on up to 36 → 41 in world 10.
  D drives fox and block toughness and the structure of every level.
- **Procedural level generator** (seeded): fills the levels around the 40 hand-made ones. All 200 levels
  pass the stability test.
- Level 20 of every world is a boss fortress. Golden Nests appear in levels 10 and 20.
- New level select: 10 world tabs and a 2 × 10 level grid, with a message when a world is locked.
- New pause menu with labeled buttons, including **Restart level**.
- **Star gifts:** 2 gift ducks per world, at 20★ and 45★.
- Late-world balance: more spare ducks and a gentler toughness curve.
- Turkish UI: "Bölüm" now means world and "Level" means level.

### Content update

- **20 new levels**, for a total of **40 levels in 8 worlds**. New worlds: Desert Canyon, Tropical Beach,
  Blossom Valley and Fire Mountain, each with its own pixel-art theme.
- **Tiered difficulty.** Each world is one tier, with a breather, a build-up and a boss. Every world starts a
  step tougher and shows a tier banner.
- **Golden Nests:** breakable bonus nests that pay +3 Golden Eggs, once each.
- **Duck voices:** every species has its own launch voice, ability sound and hit squawk.
- **Background music:** a menu theme and two world themes, with cross-fades and a separate music toggle.
- The level select now shows 8 world tabs.
- Groundwork for future accounts: a guest `PlayerProfile` with save export / import.

### Initial release

- 20 levels across 4 worlds, with 4 different pixel transition effects.
- 6 duck species with abilities: Mallard, Mandarin Duck, Merganser, Muscovy Duck, Ruddy Shelduck and Black Scoter.
- Golden Egg economy, a shop (Damage, Auto-Aim, Extra Duck), adding extra ducks during a level, and continuing after losing.
- Gradual difficulty.
- 7 languages.
- DuckThisGames splash screen.
- Pause menu: resume, retry, levels, main menu, quit.
- Fix: text could disappear or turn pale because of dynamic font atlas rebuilds. Font sizes are now quantized.
- Tooling: `DevRecorder` records gameplay frames for trailers and GIFs.
