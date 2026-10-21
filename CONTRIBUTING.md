# Contributing to DuckThis Feather Fury

Thanks for your interest! 🦆 Bug reports, level ideas and fixes are welcome.

## Reporting bugs & ideas

Use the [issue templates](../../issues/new/choose). For bugs, include the level number, the duck you
used and your device.

## Working on the project

1. Install **Unity 6000.6.6f1** (with Android Build Support if you want to build the APK).
2. Clone the repo and open it in Unity Hub. On first load, `GameBuilder` sets everything up.
3. Make your change on a branch: `git checkout -b feature/my-change`.
4. Before opening a pull request, run **Ördek Oyunu → Bölümleri Otomatik Test Et**. Every level must stay
   `SAĞLAM` (stable) and be winnable.
5. Open a pull request with a short description and, ideally, a screenshot or GIF.

## Guidelines

- **Art:** 16 px = 1 unit, point filtering, palette consistent with the existing sprites.
- **Code:** keep gameplay data in code (`Levels.cs`, `BirdType.cs`, `Economy.cs`). New UI goes in `Hud.cs`.
- **Text:** every player-facing string goes through `Loc.T()`, with all 7 languages filled in.
- **Version:** stays at **1.0** until the maintainers decide otherwise.
