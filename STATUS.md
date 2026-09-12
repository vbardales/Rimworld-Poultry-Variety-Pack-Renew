---
mod:          Poultry Variety Pack Renew
packageId:    nelim.poultryvarietypackrenew
repo:         Rimworld-Poultry-Variety-Pack-Renew
visibility:   public
detached:     yes
stage:        preTest
licence:      silent
licence_at:   no licence anywhere, and the source stopped at 1.3 in February 2022
dependencies: declared
showcase:     missing
tested_on:
workshop:
remaining:
  - unverified: never seen running
  - feature: ModIcon.png and Preview.png are not in the repository
session:      local_fefdc0a0-c53d-45f3-b38c-7deeab47cdd4
updated:      2026-09-12, by the session that split it out of the pack
---

# Poultry Variety Pack Renew — status

One sheet per mod, readable by a sweep across every mod rather than by asking each thread in turn. It
lives at the root, never inside `Mod/`, so Steam never receives it.

## Where each value comes from

- **`stage`** — one of `port`, `showcase`, `preTest`, `done`, `tested`, `published`. `preTest` here:
  the mod is complete, the assembly builds clean, the validators pass, and the game has never loaded
  it.
- **`tested_on`** — the date of the last run in game. Empty means never, which is the case.
- **`dependencies`** — Harmony, declared in the About's `modDependencies`. It is genuinely needed: the
  Marans colours come from a postfix this mod installs. An undeclared dependency is not cosmetic — on
  2026-09-11 Reequilibrage animaux took 47 vanilla animals down with it, Muffalo included, because the
  class it injects belongs to a mod that was neither declared nor loaded.
- **`licence`** — `open` an explicit licence, `silent` no licence and a dead source, `alive` no licence
  but a living source, `forbidden` a written refusal, `original` owing nothing to anyone. `silent`
  here: SirTalis states no licence in a file, in the About, in a repository or on the Workshop page,
  and the mod stopped at 1.3. `ATTRIBUTION.md` names the four places and the words each was searched
  for.
- **`showcase`** — `missing`: `Mod/About/ModIcon.png` and `Mod/About/Preview.png` are deliberately
  absent from this repository and are added by hand before the Workshop upload.
- **`remaining`** — `feature` for something missing from a first release, `defect` for a known fault
  left unfixed, `unverified` for what could not be checked.

## What the remaining lines mean

- **Never seen running.** Everything known about this mod was established without the game: the build,
  the def and field validators, and a diff against the source. What only a run can settle is whether a
  Marans chick keeps its colour into adulthood, which is the one thing the new assembly is for.
- **No icon and no preview.** Everything else is in place; these two are the owner's to add.

## Where it came from

Split out of the private pack Nelim's Animal Ark, where these six birds were one source among two
dozen. It was one of the last three to leave, because each of those three needed a piece of the pack's
assembly to go with it. This one needed the SkinSet extension and its render postfix, renamed from
`Bastyon` to `PoultryVarietyPack` on the way out so that two assemblies never expose one fully
qualified name.
