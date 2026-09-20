# Poultry Variety Pack Renew

Six birds, brought forward to RimWorld 1.6.

**I am not the author of this mod.** All six are SirTalis's; all I did was the work needed to make them
run on 1.6. Credit goes to them, mistakes in the update are mine.

Original mod: https://steamcommunity.com/sharedfiles/filedetails/?id=2749575684 — last supporting 1.3,
last updated in February 2022. Abandoned, not withdrawn.

## What the mod does

Six birds and their eggs.

- **Marans chicken**, in four colours — blue copper, black copper, birchen, silver cuckoo — each with
  its own chick, rooster and hen texture.
- **Cinnamon Queen** and **Bielefelder**.
- Three ducks: **runner**, **muscovy**, **rouen**.

Requires **Harmony**. No DLC, no framework, no other mod.

Content mod: removing it mid-save will lose any of these birds, and any of their eggs, already in play.

## What changed in the 1.6 update

**The colours had to change hands, and that is the whole of the work.**

They came from `AnimalVariations.dll`, a third-party library shipped without sources that hooked
`PawnGraphicSet.nakedGraphic` — gone in 1.5 with the arrival of the render tree.

Vanilla can do weighted colour variants natively since 1.4, and this author's squirrel pack was
converted that way. **Not this one:** `alternateGraphics` fixes a single texture used from chick to
adult, and a Marans needs three per colour. So this mod carries a small assembly of its own — one def
extension and one render postfix, about 140 lines — reproducing what the library's SkinSet files
described, with the original weights. **The colour a chick is drawn with is the colour it keeps as a
hen**, which is the part that needed care: the per-stage lists run in parallel and the index is what
carries the colour.

Two defects of the original are fixed:

- **A patch RimWorld would have rejected.** It replaced the hen's three life stages with two while the
  race declares three. Dropped; the base def was already correct.
- **The Bielefelder's egg never hatched.** It named a hatcher `Bielefelder` where the bird is
  `BielefelderChicken`.

And `wildness` moved to `<Wildness>` under `statBases`, having become a StatDef in 1.6 where the old
form is read by nothing.

No balance value was changed.

## Terms

The original **states no licence anywhere** — no file in the mod, nothing in its `About.xml`, no linked
repository, and nothing on its Workshop page, which was read looking for a refusal rather than for a
permission. Silence grants nothing and forbids nothing.

This port rests on the Workshop's own custom for abandoned mods: named credit, and a takedown on
request. If SirTalis comes back to it, or asks for this to be taken down, it comes down.

If I do not answer within a reasonable time after being contacted, anyone may freely update this or any
other of my mods, including publishing a continuation of it. All credit must be preserved.

## Credits

- **SirTalis** — the mod, all six birds, and their textures.
- 1.6 update by Nelim. Written with the help of Claude (Anthropic).

See [ATTRIBUTION.md](ATTRIBUTION.md) for the licence check, the colour machinery and the port in
detail.
