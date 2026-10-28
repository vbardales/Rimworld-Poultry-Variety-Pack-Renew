using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RimWorks.Pickle;
using RimWorld;
using Verse;

namespace PoultryVarietyPack.PickleSteps
{
    /// <summary>
    /// What Pickle's own steps cannot say about six birds and one Harmony postfix.
    ///
    /// Every step text starts with "Poultry:". Pickle loads the steps of every active suite into one
    /// namespace, and two suites declaring the same text make healthy scenarios fail with "Ambiguous
    /// step". No text uses parentheses or slashes, which Cucumber expressions read as optional text and
    /// alternatives: cells are spelled x=.. z=...
    ///
    /// WHY THERE IS AN ASSEMBLY AT ALL. The one thing this mod exists for is that a Marans keeps its
    /// colour from chick to adult, and no built-in step spawns a bird at a life stage, ages it, or reads
    /// the texture the render tree really draws for it.
    ///
    /// NOTHING HERE REFERENCES THE MOD UNDER TEST. Birds are found by defName and read through vanilla
    /// classes, so a rename fails a scenario with the name it looked for instead of failing the suite.
    ///
    /// Written without a game: the first run is the first proof these steps work.
    /// </summary>
    [PickleSteps]
    public class PoultrySteps
    {
        private static readonly Regex Colour = new Regex("BlueCopper|BlackCopper|Birchen|SilverCuckoo");

        // What a scenario noted, by the name it gave the bird.
        private static readonly Dictionary<string, string> NotedColour = new Dictionary<string, string>();

        // ------------------------------------------------------------------ finding things

        private static Map CurrentMap(PickleContext ctx)
        {
            ctx.Require(Current.Game != null && Find.CurrentMap != null, "load a save first");
            return Find.CurrentMap;
        }

        private static Pawn Named(PickleContext ctx, string name)
        {
            Pawn pawn = CurrentMap(ctx).mapPawns.AllPawns.FirstOrDefault(p => p.Name != null && p.Name.ToStringShort == name);
            ctx.Assert(pawn != null, $"no pawn named {name} on the map");
            return pawn;
        }

        private static IntVec3 Cell(PickleContext ctx, int x, int z)
        {
            var cell = new IntVec3(x, 0, z);
            ctx.Require(cell.InBounds(CurrentMap(ctx)), $"x={x} z={z} is off the map");
            return cell;
        }

        // ------------------------------------------------------------------ birds

        /// <summary>
        /// Spawns a tame bird of the given kind, sex and life stage index of its race, named so that later
        /// steps find it. The age is the first tick of that stage, so the bird is generated in it.
        /// </summary>
        [Given("Poultry: a {string} named {string} of sex {word} and life stage {int} stands at x={int} z={int}")]
        public void SpawnBird(PickleContext ctx, string kindDefName, string name, string sex, int stage, int x, int z)
        {
            PawnKindDef kind = DefDatabase<PawnKindDef>.GetNamedSilentFail(kindDefName);
            ctx.Assert(kind != null, $"no PawnKindDef \"{kindDefName}\": the def did not load");
            Map map = CurrentMap(ctx);
            var ages = kind.race.race.lifeStageAges;
            ctx.Assert(stage >= 0 && stage < ages.Count, $"{kindDefName} has {ages.Count} life stages, not {stage + 1}");
            Gender gender = sex.Equals("female", StringComparison.OrdinalIgnoreCase) ? Gender.Female : Gender.Male;
            float years = ages[stage].minAge + 0.001f;

            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
                kind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1,
                forceGenerateNewPawn: true, fixedGender: gender,
                fixedBiologicalAge: years, fixedChronologicalAge: years));
            pawn.Name = new NameSingle(name);
            GenSpawn.Spawn(pawn, Cell(ctx, x, z), map);
            ctx.Assert(pawn.ageTracker.CurLifeStageIndex == stage,
                $"{name} was generated in life stage {pawn.ageTracker.CurLifeStageIndex}, not {stage}");
        }

        /// <summary>Moves a bird to the first tick of a later life stage, the way growing up would.</summary>
        [When("Poultry: {string} grows to life stage {int}")]
        public void GrowTo(PickleContext ctx, string name, int stage)
        {
            Pawn pawn = Named(ctx, name);
            var ages = pawn.RaceProps.lifeStageAges;
            ctx.Assert(stage >= 0 && stage < ages.Count, $"{pawn.def.defName} has {ages.Count} life stages, not {stage + 1}");
            pawn.ageTracker.AgeBiologicalTicks = (long)((ages[stage].minAge + 0.001f) * 3600000f);
            ctx.Assert(pawn.ageTracker.CurLifeStageIndex == stage,
                $"{name} is in life stage {pawn.ageTracker.CurLifeStageIndex} after being aged to {stage}");
        }

        /// <summary>
        /// The texture the render tree really resolves for the bird, which is what the Harmony postfix
        /// replaces. Read from the animal-part node, never from the def.
        /// </summary>
        private static string DrawnPath(PickleContext ctx, Pawn pawn)
        {
            PawnRenderTree tree = pawn.Drawer.renderer.renderTree;
            tree.EnsureInitialized(PawnRenderFlags.None);
            PawnRenderNode_AnimalPart part = null;
            void Walk(PawnRenderNode node)
            {
                if (part != null || node == null) return;
                if (node is PawnRenderNode_AnimalPart p) { part = p; return; }
                if (node.children != null) foreach (PawnRenderNode child in node.children) Walk(child);
            }
            Walk(tree.rootNode);
            ctx.Assert(part != null, $"{pawn.Name.ToStringShort} has no animal-part render node");
            Graphic graphic = part.GraphicFor(pawn);
            ctx.Assert(graphic != null, $"{pawn.Name.ToStringShort}: the render node resolved no graphic");
            return graphic.path ?? "";
        }

        private static string ColourOf(PickleContext ctx, Pawn pawn)
        {
            string path = DrawnPath(ctx, pawn);
            Match m = Colour.Match(path);
            ctx.Assert(m.Success, $"{pawn.Name.ToStringShort} is drawn with {path}, which names none of the four Marans colours");
            return m.Value;
        }

        [Then("Poultry: {string} is drawn in one of the four Marans colours")]
        public void DrawnInAColour(PickleContext ctx, string name)
        {
            ColourOf(ctx, Named(ctx, name));
        }

        [Then("Poultry: the colour of {string} is noted")]
        public void NoteColour(PickleContext ctx, string name)
        {
            NotedColour[name] = ColourOf(ctx, Named(ctx, name));
        }

        /// <summary>The whole point of the mod's assembly: the colour drawn for the chick is the colour drawn for the adult.</summary>
        [Then("Poultry: the colour of {string} is still the noted one")]
        public void SameColour(PickleContext ctx, string name)
        {
            ctx.Assert(NotedColour.TryGetValue(name, out string noted), $"no colour was noted for {name}");
            string now = ColourOf(ctx, Named(ctx, name));
            ctx.Assert(now == noted, $"{name} was {noted} and is now {now}: the colour did not follow the bird through growth");
        }

        [Then("Poultry: the texture drawn for {string} names the stage word {string}")]
        public void StageWord(PickleContext ctx, string name, string word)
        {
            string path = DrawnPath(ctx, Named(ctx, name));
            ctx.Assert(path.Contains(word), $"{name} is drawn with {path}, which does not contain {word}");
        }

        // ------------------------------------------------------------------ eggs

        /// <summary>
        /// Hatches a fertilized egg through the game's own CompHatcher, the way the end of an incubation
        /// does, and checks the pawn that comes out is of the expected kind and a newborn. The Bielefelder
        /// egg named a hatcher that did not exist in the original; this is the step that would have said so.
        /// </summary>
        [Then("Poultry: a fertilized {string} egg hatches into a {string} in life stage {int}")]
        public void EggHatches(PickleContext ctx, string eggDefName, string kindDefName, int stage)
        {
            Map map = CurrentMap(ctx);
            ThingDef eggDef = DefDatabase<ThingDef>.GetNamedSilentFail(eggDefName);
            ctx.Assert(eggDef != null, $"no ThingDef \"{eggDefName}\": the def did not load");
            Thing egg = ThingMaker.MakeThing(eggDef);
            IntVec3 cell = CellFinder.RandomClosewalkCellNear(map.Center, map, 12);
            GenSpawn.Spawn(egg, cell, map);
            CompHatcher hatcher = ((ThingWithComps)egg).GetComp<CompHatcher>();
            ctx.Assert(hatcher != null, $"{eggDefName} has no CompHatcher");
            ctx.Assert(hatcher.Props.hatcherPawn != null && hatcher.Props.hatcherPawn.defName == kindDefName,
                $"{eggDefName} hatches into {hatcher.Props.hatcherPawn?.defName ?? "nothing"}, not {kindDefName}");

            int before = map.mapPawns.AllPawns.Count(p => p.kindDef.defName == kindDefName);
            hatcher.Hatch();
            var born = map.mapPawns.AllPawns.Where(p => p.kindDef.defName == kindDefName).ToList();
            ctx.Assert(born.Count == before + 1, $"after Hatch() there are {born.Count} {kindDefName} on the map, there were {before}");
            Pawn chick = born.First(p => !p.Dead);
            ctx.Assert(chick.ageTracker.CurLifeStageIndex == stage,
                $"the hatchling is in life stage {chick.ageTracker.CurLifeStageIndex}, not {stage}");
        }

        // ------------------------------------------------------------------ language

        private static string ActiveLanguage() => LanguageDatabase.activeLanguage.folderName;

        /// <summary>
        /// The label of a ThingDef in the language of the pass, from the value written for it. Fails,
        /// naming the language, in a pass whose language has no value here.
        /// </summary>
        [Then("Poultry: the label of the ThingDef {string} is {string} in English and {string} in French")]
        public void LabelInLanguage(PickleContext ctx, string defName, string english, string french)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            ctx.Assert(def != null, $"no ThingDef \"{defName}\": the def did not load");
            string language = ActiveLanguage();
            string expected;
            if (language == "English") expected = english;
            else if (language == "French") expected = french;
            else { ctx.Assert(false, $"this pass runs in {language}, a language this scenario has no value for"); return; }
            ctx.Assert(def.label == expected, $"in {language} the label of {defName} is \"{def.label}\", not \"{expected}\"");
        }

        [Then("Poultry: the description of the ThingDef {string} starts with {string} in English and {string} in French")]
        public void DescriptionInLanguage(PickleContext ctx, string defName, string english, string french)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            ctx.Assert(def != null, $"no ThingDef \"{defName}\": the def did not load");
            string language = ActiveLanguage();
            string expected;
            if (language == "English") expected = english;
            else if (language == "French") expected = french;
            else { ctx.Assert(false, $"this pass runs in {language}, a language this scenario has no value for"); return; }
            ctx.Assert(def.description != null && def.description.StartsWith(expected),
                $"in {language} the description of {defName} is \"{def.description}\", it does not start with \"{expected}\"");
        }

        /// <summary>The gendered and life-stage labels of a pawn kind, which sit in nested DefInjected paths.</summary>
        [Then("Poultry: the PawnKindDef {string} calls its females {string} in English and {string} in French")]
        public void FemaleLabel(PickleContext ctx, string kindDefName, string english, string french)
        {
            PawnKindDef kind = DefDatabase<PawnKindDef>.GetNamedSilentFail(kindDefName);
            ctx.Assert(kind != null, $"no PawnKindDef \"{kindDefName}\": the def did not load");
            string language = ActiveLanguage();
            string expected;
            if (language == "English") expected = english;
            else if (language == "French") expected = french;
            else { ctx.Assert(false, $"this pass runs in {language}, a language this scenario has no value for"); return; }
            ctx.Assert(kind.labelFemale == expected, $"in {language} the female label of {kindDefName} is \"{kind.labelFemale}\", not \"{expected}\"");
        }

        // ------------------------------------------------------------------ save

        /// <summary>A bird must be where the save left it, in the same life stage and with the same colour.</summary>
        [Then("Poultry: {string} is in life stage {int}")]
        public void InLifeStage(PickleContext ctx, string name, int stage)
        {
            Pawn pawn = Named(ctx, name);
            ctx.Assert(pawn.ageTracker.CurLifeStageIndex == stage,
                $"{name} is in life stage {pawn.ageTracker.CurLifeStageIndex}, not {stage}");
        }
    }
}
