# In-game scenarios, run by Pickle

The scenarios of [TEST_SCENARIOS.md](../../TEST_SCENARIOS.md) that a running game is needed for, and only those.
`Mod/` is a companion mod, **Poultry Variety Pack Renew - Pickle tests**, never published. It holds the feature files
and a small step assembly (`Source/`, built into `Mod/Pickle/Assemblies/`), so nothing test-related ships in the
Workshop folder.

## Status

Written 2026-10-28, **never run**. The step assembly compiles; no step has met a game yet, so the first run is also
the first proof of the steps.

## The passes

| Pass | Language | What it establishes |
| --- | --- | --- |
| Minimal, English | English | The mod stands on its own beside Harmony; colour persistence, eggs, save round trip |
| Minimal, French | French | Same, with the French text (`04-language`) |
| With ADS 2 (optional) | English | Not written: the patch is proved in console tests; decided at `playTests` |

Run each pass through the shared launcher, never by hand:

```powershell
powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod PoultryVarietyPackRenew -Language English
powershell.exe -ExecutionPolicy Bypass -File scripts/Run-PickleWsl.ps1 -Mod PoultryVarietyPackRenew -Language French
```

## The features

| Feature | What it stages | Assertions |
| --- | --- | --- |
| `01-loads` | the game's real load | order after Harmony; six races and kinds are this mod's; no error on load |
| `02-marans-colour` | a Marans chick grown to adult, hen and rooster | the drawn texture keeps one of four colours through every stage |
| `03-eggs` | each fertilized egg hatched through `CompHatcher` | the right bird, as a newborn |
| `04-language` | the pass language | labels and descriptions, the Bielefelder hen |
| `05-save-reload` | save and reload | same life stage and same colour |

Steps: every text starts with `Poultry:` (`Source/PoultrySteps.cs`).
