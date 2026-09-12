# Changelog

Format inspired by [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
This file serves the repository and the writing of Steam patch notes; RimWorld does not display it
in game.

## [1.0.0] — unreleased

On release: add `Mod/About/ModIcon.png` and `Mod/About/Preview.png`, create the `v1.0.0` tag and
the matching GitHub release, then publish to the Workshop.

First release of the 1.6 update of **Poultry Variety Pack**, by SirTalis.

### Added

- **An assembly of this mod's own, replacing `AnimalVariations.dll`.** One `DefModExtension`,
  `PoultryVarietyPack.SkinSetModExt`, and one postfix on `PawnRenderNode_AnimalPart.GraphicFor`,
  about 140 lines together. The library the original went through is third-party, ships no sources,
  and hooked `PawnGraphicSet.nakedGraphic`, which 1.5 removed with the arrival of the render tree.
  Vanilla's `alternateGraphics` could not stand in: it fixes one texture used from chick to adult,
  while each Marans colour has a chick, a rooster and a hen. The original weights are kept, and the
  colour drawn for a chick is frozen and reused for the adult — otherwise a blue chick would grow
  into a black hen, the per-stage lists running in parallel with the index carrying the colour.
  The namespace and the Harmony id are this mod's own, so that nothing collides with the pack this
  port came out of, which still ships the same class as `Bastyon.SkinSetModExt`.
- **`incompatibleWith` naming `sirtalis.poultrypack`**, the id the source meant to declare — its
  `About.xml` spells it `<packageID>`, which RimWorld does not read, so the line may never match,
  which costs nothing.
- **Harmony declared as a dependency**, which the original did not need.

### Fixed

- **A patch RimWorld would have rejected.** `MaransVarietyCoats.xml` replaced the hen's **three**
  life stages with **two** while its race declares three. The patch is dropped and the base def
  kept: it was already correct and already carried `femaleGraphicData`. The chick's yellow tint went
  with it, since it would have coloured all four variants alike.
- **The Bielefelder's egg hatches.** It looked for a `hatcherPawn` named `Bielefelder` where the bird
  is `BielefelderChicken`. An original typo, not a 1.6 change.

### Changed

- **`wildness` moved to `<Wildness>` under `statBases`.** It stopped being a field of
  `RaceProperties` in 1.6 and became a StatDef; the old form is read by nothing and the stat's
  default is `-1`, deliberately outside the range the game uses.

### Notes

No balance value was changed: the birds' stats, their biomes and their egg rates are the author's.
