# Changelog

## 1.0

### Fixes from test report v3

- Hidden-fox fix tightened: the camera now shows 1.2 units more on the right, and a fox is eliminated as soon as
  it passes maxX + 0.3 (or slides behind the slingshot), so a fox can never be alive but invisible.
- Towers are 3–7 floors, with stone columns below and light wood above; no roof fox on very tall towers
  (it used to wobble off before the first shot). Stability test now waits 8 s on tower levels: 500/500 stable.
- Walls softened: lighter pyramid template (wood base, fewer foxes), wooden roof on boss bunkers, helmeted foxes
  capped at 45%. World 3 bot win rate 32% → 52%, boss L60 0/9 → 2/3.
- Bot test v3 (before these fixes): 62.6% wins overall, 0/3 levels 145 → 91. Report: `docs/balance/TEST_RAPORU_v3.md`.


### Fixes: hidden foxes, tall towers

- **Fix:** a fox pushed off the right edge of the level (or below the ground) stayed alive off-screen, so the level
  could never be finished (e.g. L13). Such foxes are now counted as defeated.
- **Skyscrapers:** levels 4, 11 and 18 of every generated world have a 3–8 floor tower, independent of difficulty.
  The camera now zooms out to fit tall structures. All 500 levels still pass the stability test.


### New game logo & icon

- Pixel-art game logo (angry Mallard, scared fox, golden egg, "DUCKTHIS / FEATHER FURY") on the main menu.
- Opening screen: the badge logo grows from small to full size in ~2 s, with "MADE BY DUCKTHISGAMES" (3 s total).
- New pixel-art app icon (legacy, round and adaptive layers). Logo generator: `tools/make_logo.py`.


### DuckThisGames branding

- New DuckThisGames logo on the opening screen (white background, pop-in animation) and as the app icon
  (legacy, round and adaptive Android icons).
- Unity splash screen and "Made with Unity" logo turned off: the game opens straight on the DuckThisGames logo.


### Balance pass (playtested 2 × 1500 bot games)

- Gentler transition into generated levels (L41–100), first level of each world and levels 17–19 lighter,
  at most 4 seven-duck forts per world and never more than 2 in a row, a light level after two hard ones.
- At least 4 ducks per generated level (fixes unwinnable L43/L99); extra duck for L13, L18, L40 and every boss.
- Late game keeps getting harder: fox/block toughness grows after L100; gold per damage tapers from L161
  (×1) to L400+ (×0.65). 2★/3★ thresholds lowered on big levels.
- Bot win rate 41.9% → 49.3%, zero-win levels 188 → 145 (before the last rule fixes). Reports:
  `docs/balance/ZORLUK_RAPORU.md`, `docs/balance/ZORLUK_RAPORU_v2.md`.
- Language pass 2: 63 more fixes; natural duck names in every language (TR: YEŞİL BAŞLI ÖRDEK,
  ANGIT ÖRDEĞİ, KILKUYRUK ÖRDEK …). Report: `tools/loc/review/REPORT_v2.md`.


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
