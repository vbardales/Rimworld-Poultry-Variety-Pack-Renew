# Functional scenarios - Poultry Variety Pack Renew (unofficial)

Status of every scenario: see its column. Target: RimWorld 1.6. `U` scenarios run in a console process with no game
(`Tests/`), `P` scenarios run in game under Pickle (`Tests/Pickle/`). A green console test proves the logic, not
what the game draws: the `P` scenarios are written and not yet run (they run at `playTests`).

Figures checked against the code and the shipped XML: the Marans weights are 0.3, 0.4, 0.1 and 0.3 (blue copper,
black copper, birchen, silver cuckoo), which sum to 1.1, so the shares are 27.3 %, 36.4 %, 9.1 % and 27.3 %. The
Marans race has three life stages, so three skin-set blocks.

## What the mod promises

Six birds and their eggs: three chickens (Marans, Cinnamon Queen, Bielefelder), three ducks (runner, Muscovy, Rouen).
The Marans comes in four colours, each with its own chick, rooster and hen. A bird keeps its colour as it grows. Both
languages are covered. Optional integration: A Dog Said... Animal Prosthetics 2 sees the six birds in the right
categories. No balance value was changed.

## Console tests (`Tests/`, `scripts/Run-UnitTests.ps1 -ModRoot . -PreBuild Source/PoultryVarietyPack.csproj`)

| ID | Behaviour | Expected result | Status |
|---|---|---|---|
| U01 | Life stage lookup, `SkinSetModExt.StageFor` | Clamps above and below range, `appliesToAll` uses the first block, no blocks gives null | passed 2026-10-28 |
| U02 | Gender lookup, `SkinSetLifeStage.ListFor` | Females use their own list when it exists, else the shared list | passed 2026-10-28 |
| U03 | Weighted colour choice | In range, same seed gives the same colour, never a zero weight, all zero gives 0 | passed 2026-10-28 |
| U04 | Weights followed over 20000 birds | Shares within 2 points of weight / 1.1 | passed 2026-10-28 |
| U05 | Colour persistence rule | One seed draws the same index in two lists with the same weights (chick and adult) | passed 2026-10-28 |
| U06 | Shipped Marans XML | One block per race life stage, same number of colours and same weights in every block, every texture file exists | passed 2026-10-28 |
| U07 | ADS 2 patch, ADS 2 present (fixture and the installed copy of its real file) | Six birds in `ADS_Cat1`, three ducks in `ADS_Cat2`, none in `ADS_Cat3`, existing entries kept | passed 2026-10-28 |
| U08 | ADS 2 patch, ADS 2 absent or list renamed | Document untouched, no failure | passed 2026-10-28 |
| U09 | The expectations can fail | An unpatched document is rejected by the assertions | passed 2026-10-28 |
| U10 | XML structure and references | `Check-DefRefs.ps1` and `Check-XmlFields.ps1` pass | passed 2026-10-27 |

## In-game scenarios (`Tests/Pickle/`, run by Pickle, written and not run)

| ID | Feature | Action | Expected result | Status |
|---|---|---|---|---|
| P01 | `01-loads` | Load Core, Harmony, the mod | Mod active, loads after Harmony | not run |
| P02 | `01-loads` | Look up the six races and kinds; load a save | Each is this mod's def; no error, no warning naming a bird | not run |
| P03 | `02-marans-colour` | A chick generated in life stage 0 (hen and rooster) | Drawn in one of the four colours, with a chick texture | not run |
| P04 | `02-marans-colour` | Grow it to stage 1, then 2 | Same colour at each stage, hen or rooster texture at the end | not run |
| P05 | `03-eggs` | Hatch each fertilized egg through `CompHatcher` | The expected bird, as a newborn (the Bielefelder egg included) | not run |
| P06 | `04-language` | English pass, French pass | Labels, descriptions and the Bielefelder hen in the language of the pass | not run |
| P07 | `05-save-reload` | Save and reload with a Marans chick, then grow it | Same life stage and same colour after the reload and after growth | not run |

## Not applicable, and why

- **Settings.** The mod has none (`settings_audit: not_applicable`, sources read).
- **Vanilla behaviour left untouched** (feeding, taming, egg laying cycle, crossbreeding, nocturnal habits): the mod
  changes none of it, so nothing is tested (AUDIT.md, "On ne teste pas ce que mod ne modifie pas").
- **Integrations without a patch** (Nocturnal Animals, Dogs mate, Better Crossbreeding): no patch exists, so there is
  nothing of ours to test; the reasons are in `STATUS.md`.
- **ADS 2 in game.** The patch is proved against the real file of ADS 2 in console tests. A Pickle pass with ADS 2
  loaded (`@requires:SamBucher.ADogSaidAnimalProsthetics2`) is optional and decided at `playTests`.
- **Fresh, rotting and dessicated corpses.** The postfix leaves them alone by construction (it returns unless the
  render is fresh and the pawn alive); a scenario would test the game, not the mod. To read by eye in `playTests`.
