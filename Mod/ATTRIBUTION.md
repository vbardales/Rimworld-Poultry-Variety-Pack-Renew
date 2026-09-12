# Poultry Variety Pack — where the content comes from, and what had to be changed

Everything in this mod is **SirTalis's** work: the six birds, their stats, their textures, their
colours. This repository holds the port to RimWorld 1.6 and the small assembly that replaces a dead
third-party library, described below.

## The source

| | |
|---|---|
| Mod | Poultry Variety Pack |
| Author | SirTalis |
| Workshop | [2749575684](https://steamcommunity.com/sharedfiles/filedetails/?id=2749575684) |
| Last version supported | 1.3 |
| Last updated | 13 February 2022 |
| Licence | none stated |

**Abandoned, not withdrawn.** The item is still on the Workshop and still downloadable; it stopped at
1.3, missing 1.4, 1.5 and 1.6. Nobody else has picked it up: Mlie has no continuation of it, a
Workshop search filtered on the 1.6 tag returns nothing related, and no installed mod declares
`MaranChicken` or `BielefelderChicken`.

Its `packageId` is spelled `<packageID>` in its own `About.xml`, with a capital D, which is not the
field RimWorld reads — the same slip as in this author's Squirrel Variety Pack.

## The licence, looked for in four places

"None stated" is a verdict, not an absence of checking. A refusal never presents itself as a licence,
so each place was searched for the refusal rather than for the permission — `prohibit`, `forbid`,
`do not redistribute`, `no reupload`, `all rights reserved`, `without permission`, and the Japanese
and Chinese forms 禁止, 転載, 無断, 二次配布, 不得.

| Where | What it says |
|---|---|
| A `LICENSE` or `COPYING` file in the mod | there is none |
| The `<description>` of its `About.xml` | nothing about reuse |
| A linked repository | there is none |
| The Workshop page description | nothing about reuse |

Silence grants nothing and forbids nothing. This port rests on the Workshop's own custom for abandoned
mods: named credit, and a takedown on request.

## The colours, and why this mod has an assembly at all

The original drew its Marans colours through **`AnimalVariations.dll`**: a `thingClass` of
`AnimalVariations.AnimalMultiSkins` plus a `MaransVarietyCoats.xml` under `Textures/`. That library is
third-party, ships without sources, and does not survive the render tree RimWorld moved to in 1.5,
where `PawnGraphicSet` and its `nakedGraphic` field — what it hooked — no longer exist.

**Vanilla could not absorb this one.** `PawnKindDef.alternateGraphics` has done weighted colour
variants natively since 1.4, and this author's Squirrel Variety Pack was converted that way, because
one squirrel colour holds for every age. A Marans does not: each of the four colours has a chick
texture, a rooster one and a hen one, and `alternateGraphics` fixes a single `texPath` used from chick
to adult.

So this port carries **one `DefModExtension` and one Harmony postfix**, about 140 lines, which
reproduce what the `*_SkinSet.xml` files described:

- `PoultryVarietyPack.SkinSetModExt`, a list of variants per life stage, with an optional female list.
- a postfix on `PawnRenderNode_AnimalPart.GraphicFor`, the one point where vanilla resolves an
  animal's graphic, touching only the living, fresh render — corpse, rotting and dessicated have their
  own branches and must not be overwritten.
- the original weights, unchanged.
- **the drawn index frozen on the first life stage** and reused afterwards. Without that a blue chick
  would grow into a black hen: the per-stage lists run in parallel and the index is what carries the
  colour.

The namespace is this mod's own, not the pack's. The private pack this port came out of ships the same
class as `Bastyon.SkinSetModExt`, and two assemblies exposing one fully qualified name leave `GenTypes`
to pick between them. The Harmony id is its own for the same reason.

## What else the port changed

- **A patch that RimWorld would have rejected is gone.** `MaransVarietyCoats.xml` replaced the hen's
  **three** life stages with **two**, while the race declares three. The patch is dropped and the base
  def kept: it was already correct and already carried `femaleGraphicData`. The only thing lost with it
  is the chick's yellow tint, which would have coloured all four variants alike.
- **The Bielefelder's egg hatches.** It looked for a `hatcherPawn` named `Bielefelder` where the bird
  is `BielefelderChicken`, so it produced nothing. An original typo, not a 1.6 change.
- **`wildness` moved to `<Wildness>` under `statBases`.** It became a StatDef in 1.6 and the old form
  is read by nothing, the stat's own default being `-1`, deliberately out of range.

## What was left alone

- **The birds' own stats, biomes and egg rates**, untouched.
- **The dessicated corpses** use the base game's chicken texture, as the author wrote them.
- **No balance value was touched.**

## Where this came from

The port was done inside a private pack that had gathered two dozen abandoned animal mods, where these
six were one source among them. They leave the pack to stand on their own, because the rule that pack
follows is that a mod which is dead **and** states nothing gets republished with credit rather than
kept back. This one left last of the fifteen, with the two other sources whose C# had to be split out
of the pack's assembly along with them.
