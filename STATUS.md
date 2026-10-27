---
localization: complete
translation_en: partial
translation_fr: partial
mod:          Poultry Variety Pack Renew
packageId:    nelim.poultryvarietypackrenew
repo:         Rimworld-Poultry-Variety-Pack-Renew
visibility:   public
detached:     yes
workflow_stage: setupRepo
licence:      silent
licence_at:   no licence anywhere, and the source stopped at 1.3 in February 2022
dependencies: verified
showcase:     partial
settings_audit: not_applicable
tested_on:
workshop:
remaining:
  - defect: 1.d suffix ' (unofficial)' absent from About name, README title, STATUS mod field
  - defect: 1.h STATUS.md not built from templates/STATUS.template.md (remote, upstream_mod_remotes absent)
  - unverified: never seen running
  - defect: RouenDuck juvenile dessicatedBodyGraphicData.drawSize is 065
  - defect: public silent naming and UNOFFICIAL notice are missing
  - defect: About description lacks the required final Source code on GitHub link
  - defect: French coverage is absent for 75 owned text fields
  - defect: English Bielefelder labelFemale is CBielefelder hen; description has spelling errors
  - feature: written functional scenarios and automated behavior tests are absent
  - unverified: in-game scenarios, logs, EN/FR display and save compatibility
session:      local_fefdc0a0-c53d-45f3-b38c-7deeab47cdd4
updated:      2026-10-27, workflow re-audit
protocols_read_sha: 43dd90d4b7873c500347fba1be01275a48d5795b
---

# Poultry Variety Pack Renew — status

One sheet per mod, readable by a sweep across every mod rather than by asking each thread in turn. It
lives at the root, never inside `Mod/`, so Steam never receives it.

## Historical split-out record (2026-09-12)

The following original account is preserved as history; current findings below supersede its
claims of completeness, stage semantics and passing validators.

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


## Workflow audit — 2026-09-13

Audited HEAD: `6f2f818202cdd4e39ffa547763a280f7ca48b36a`, also the remote HEAD.
Standalone Git root: `C:/Users/nelim/Documents/rimworld/PoultryVarietyPackRenew`;
distribution root: `Mod/`. Initial local changes: only STATUS.md, adding the three
unchecked localization fields. Those fields are retained and updated with this audit.
During the audit, untracked `output/imagegen/poultry-mascot-orange.png` and its prompt
appeared; they were not created, altered or visually reviewed by this audit. They are
outside the distribution and do not satisfy installation in Mod/About. Build and
inspection outputs are isolated under ignored `.build/audit/`. No game launch, content
fix, image generation, commit, push or publication was performed.

Rules read: parent AGENTS.md, PUBLISHING.md, STYLE_RIMWORLD.md, MOD_SETTINGS.md and
TRANSLATIONS.md. The supplied audit prompt overrides the older settings runtime gate.
`stage` now uses the workflow's literal names, not the historical abbreviated codes:
`dansMonoRepo -> horsMonoRepo -> ModIcon générée -> Preview générée -> preOptions ->
options -> l10n -> preTest -> done -> tested`. The old `preTest` claim is preserved above
as history. Current cumulative state: **horsMonoRepo**.

### Ordered transition results

| Transition | Result | Evidence or blocking criterion |
| --- | --- | --- |
| dansMonoRepo -> horsMonoRepo | Validated | Independent .git root, configured GitHub remote, PUBLIC repository and pushed HEAD confirmed with git ls-remote and gh repo view. PackageId, Renew title, repository slug and folder are coherent. English README, CHANGELOG and attribution exist; root and distributed attribution are byte-identical. |
| horsMonoRepo -> ModIcon générée | Defect found | Mod/About/ModIcon.png appeared during the audit but is oversized (1254x1254, 1120519 bytes). The XML value 065 in RouenDuck.xml:130 also prevents claiming development finished. Isolated compilation and shipped code correspondence passed, as detailed below. |
| ModIcon générée -> Preview générée | Defect found | Mod/About/Preview.png is absent. Dimensions, PNG validity, size and appearance cannot be inspected for an absent artifact. |
| Preview générée -> preOptions | Defect found | English description exists, but the exact final Steam-formatted Source code on GitHub link is absent. Public silent suffix (unofficial) and the prescribed opening UNOFFICIAL notice are absent from About/README/status naming. Renew itself is coherent. Preview palette and title treatment remain unverified because no installed preview exists. |
| preOptions -> options | Not applicable, justified independently | Source and Def inventory establishes no useful mod settings, empty settings page or MainButtons shortcut. See Settings audit. |
| options -> l10n | Defect found | Native translatable Def fields exist; French resources are absent. English contains Bielefelder spelling/label defects. See Translation audit. |
| l10n -> preTest | Validated independently | Harmony is used by the startup patch, declared with correct packageId and ordered before this mod. Core-only reference validation passes. No LoadFolders, conditional patches, DLC-specific references or optional settings integration is present. DLC loadAfter entries impose no DLC dependency. |
| preTest -> done | Not established | No written functional scenario suite or automated behavior tests is present. XML validators were executed successfully during this audit, but do not establish skin-selection behavior or game loading. Behavior-test non-applicability is not justified: weighted choice and stage/gender mapping are substantive logic. |
| done -> tested | Unverified | No in-game execution evidence for the delivered revision. EN/FR, logs, new game, existing save, growth, eggs and colour persistence remain to be exercised. No settings persistence/shortcut tests apply. RimWorld was not launched. |

### Rights and publication metadata

`silent` is retained under the operational workflow definition, not treated as an open
licence or author consent. The installed original at
`C:/Program Files (x86)/Steam/steamapps/workshop/content/294100/2749575684` declares only
1.3, has no LICENSE/COPYING file and no reuse terms in About.xml. The public original
Workshop page was fetched on this date: it still declares 1.3, and its description
contains the birds, Camera+ recommendation and a planned CE patch, with no reuse terms.
Snapshot: `.build/audit/upstream-workshop.html`. ATTRIBUTION.md retains the earlier
four-location rights review and takedown commitment. This audit does not claim an
exhaustive fresh review of all historical comments. No third-party licence was invented;
the absence of a LICENSE is not by itself a defect for this unlicensed source.

The missing unofficial suffix/notice belongs to the naming/description gate
Preview générée -> preOptions; it does not negate the verified Git extraction.
The description's raw GitHub URL and its correct <url> field do not replace the required
final `[url=https://github.com/vbardales/Rimworld-Poultry-Variety-Pack-Renew]Source code on GitHub[/url]`.

### Build and XML checks actually executed

- .NET SDK 8.0.424, Release/net48: build succeeded with 0 warnings and 0 errors.
  Command: `dotnet build Source/PoultryVarietyPack.csproj -c Release --nologo`
  with BaseIntermediateOutputPath and MSBuildProjectExtensionsPath redirected to
  `.build/audit/obj/`, OutputPath to `.build/audit/bin/`, RestoreIgnoreFailedSources=true.
  Restored Krafs.Rimworld.Ref 1.6.4871 and Lib.Harmony 2.3.6. Initial sandbox SDK access
  failed; the same isolated build succeeded with the required environment access.
- Shipped DLL SHA256: `BA3893F2856BA58A70C91EB16390F8ABAD6EE8822CE250EE4BB449930D3CE349`.
  Audit DLL SHA256: `1A051176EB7F9E310931E6CA245F6D34D66E773E4E83F79394E98310A078CFD3`.
  ILSpy decompilation comparison found only AssemblyInformationalVersion differs
  (old commit 2c3f310... versus current 6f2f818...). All decompiled implementation and
  remaining assembly attributes match. Different hashes alone are not stale-code proof.
  Outputs: `.build/audit/decompiled-shipped/` and `.build/audit/decompiled-built/`.
  ILSpy required DOTNET_ROLL_FORWARD=Major because its .NET 6 runtime is absent.
- `pwsh -NoProfile -File ../scripts/Check-DefRefs.ps1 -ModPath ./Mod -Brief`:
  well-formed XML; 18 distinct def names; no unresolved references, wrong reference
  types or missing parents. Repeated with GameData restricted to the installed Core
  folder: same passing results, establishing no required DLC Def dependency.
- `pwsh -NoProfile -File ../scripts/Check-XmlFields.ps1 -ModPath ./Mod
  -ExtraAssemblies ./Mod/Assemblies/PoultryVarietyPack.dll`: 8 files checked;
  no unknown 1.6 fields. This validator checks field structure, not all values.
- All distinct owned texPath values resolve to local PNG files; the only path outside
  the mod is the declared vanilla Chicken/Dessicated_Chicken path. Texture rendering
  and directional appearance were not tested in game.
- Manual value review found `RouenDuck` PawnKindDef lifeStages[1].dessicatedBodyGraphicData
  drawSize `065`, unlike adjacent `0.65`/`0.63` sizes. This is a malformed intended scale
  (numeric 65 instead of approximately 0.65); the exact visible effect remains untested.
- Repository CI builds and checks DLL existence only. No behavior test suite, XML test
  suite local to this repository, functional scenarios or runtime results were found.
  External XML scripts above are real executed checks, not evidence of behavior tests.

### Settings audit

`settings_audit: not_applicable`. Reviewed both C# files, project configuration and all
shipped XML. The mod adds six fixed bird breeds, eggs and deterministic weighted Marans
skins. Stats and skin weights are content definitions, not documented user preferences
or an existing XML-only settings interface. No concrete settings need was found.
There is no Verse.Mod subclass, ModSettings, SettingsCategory, settings window,
MainButtonDef or settings shortcut. The startup class only installs Harmony patches;
SkinSetModExt contains internal content data. Therefore no empty page or button is exposed.
No settings integration was tested or claimed; RIMMSQOL is neither required nor relevant
to a nonexistent shortcut. Defaults, input limits, settings persistence and settings
application tests are not applicable. Bird/save behavior still requires its own tests.

### Translation audit

Inventory: all seven Def files, About metadata and both C# files. There are **75 owned
text fields** (labels, descriptions, gender labels, life-stage labels and tool labels).
All use native Def translation mechanisms; no code-generated player-facing text exists.
The extension's strings are texture paths, not display text. About, technical comments
and repository documentation are outside the in-game translation gate.

`localization: complete`: identified texts are in native translatable fields.
`translation_en: partial`: all 75 fields have English source fallback, so no redundant
English DefInjected files are needed; Bielefelder.xml has `CBielefelder hen` at line 98
and `betwen`/`noticably` at line 8, preventing full reviewed-English validation.
`translation_fr: partial`: no Languages directory exists; none of the 75 owned fields
has mod-supplied French coverage. This is a confirmed gap, not merely a missing test.
Nested tool and life-stage labels must be included when French resources are added.
No Keyed parameters, translation duplicates, injected paths or placeholder entries exist
to check. Check-DefInjected is not applicable to nonexistent injection files; claiming a
passing path audit would be misleading. Run it after resources exist, then verify
coverage, terminology, handles and formatting. EN/FR rendering remains unverified.

### Strict next transition and optional observations

To reach ModIcon générée: resolve the recorded development defects, rerun affected XML
checks, and resize the installed icon to a conforming 128x128 PNG at Mod/About/ModIcon.png, then inspect it at 32px. The build
and code-correspondence checks already pass; only changes affecting them require a rebuild.
Do not require a preview or an in-game launch to pass this immediate transition.

Later gates require the preview, publication naming/description corrections, French
coverage, completed English review, written functional scenarios and meaningful automated
behavior tests. Final in-game validation must cover both languages, logs, a new game and
an existing save, bird sexes/life stages, eggs/hatching, Marans colour persistence across
aging and save/reload, and fresh/rotting/dessicated corpses.

Optional: localize build intermediate paths into this standalone repository;
Directory.Build.props currently retains a parent-level .build path from the pack.
No purely aesthetic preview recommendation is made: no installed image was inspected.

### Final artifact refresh

Before closing the audit, an untracked Mod/About/ModIcon.png appeared. Direct image
inspection was performed: one orange chicken mascot face, clean silhouette, no text.
The PNG is 1254x1254 and 1120519 bytes, rather than the expected 128x128; it therefore
needs resizing before the icon gate passes. No 32px visual check was performed. This
supersedes earlier observations of absence; output/imagegen remains untouched.
Preview.png is still absent. The global stage remains horsMonoRepo because of the
oversized icon and development defect. No other session's artifacts were modified.

## Workflow re-audit - 2026-10-27

Replayed from the chain start against current protocols. First unmet criterion: 1.d
(licence `silent`: ` (unofficial)` suffix and `UNOFFICIAL` paragraph absent from About name,
description and README title), plus 1.h (STATUS.md not from the template). Retained state:
`setupRepo` (was `horsMonoRepo`, mapped to `makeSources`). Later findings of the
2026-09-13 audit stay valid. ModIcon.png and Preview.png now exist; Preview regeneration
rendered 2026-10-27 with Art/echo-preview.png (owner line-art rooster); ModIcon-source.png trimmed by badge mode (1254x1254 to 1060x1029), padding check passes.
