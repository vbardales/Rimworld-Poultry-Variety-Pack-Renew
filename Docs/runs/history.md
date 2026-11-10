2026-10-28 console tests (Tests/, Run-UnitTests.ps1 -PreBuild Source/PoultryVarietyPack.csproj): 15 passed, 0 failed at 55aaeea10764b84e8d601683b94b0d5206fc727f
2026-11-02 pickle initial English pass (tree 3b3b3a4): 13 of 19 passed, 6 red = step fault in 01-loads (name shared by ThingDef and PawnKindDef), fixed with a local step
2026-11-03 pickle 01-loads each bird (tree cde6a7b, ticket 20261102-203017-268-c4fb): 6 of 6 passed
2026-11-10 pickle french pass 04-language (tree ca05f46, ticket 20261110-154038-885-274c): 0 of 2 passed, step fault (ActiveLanguage() expects folderName French, game gives French (Français)); step fixed in cde3d7a, replay ticket 20261110-160135-543-fa84 queued
