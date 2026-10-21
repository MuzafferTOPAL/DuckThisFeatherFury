# Changelog

## 1.0

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
