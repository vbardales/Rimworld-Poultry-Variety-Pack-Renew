using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PoultryVarietyPack
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            // Its own Harmony id. The pack this port came out of patches the same method under
            // com.Bastyon; two mods sharing an id cannot be told apart in a Harmony report, and
            // unpatching one would take the other with it.
            var harmony = new Harmony("com.nelim.poultryvarietypack");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    /// <summary>
    /// Swaps the texture after the fact, at the one point where vanilla resolves an animal's graphic:
    /// PawnRenderNode_AnimalPart.GraphicFor.
    ///
    /// 1.6: PawnGraphicSet and its nakedGraphic field, which the AnimalVariations library this
    /// replaces went through, disappeared in 1.5 along with the arrival of the render tree.
    ///
    /// The postfix touches only the living, fresh render: corpse, rotting and dessicated have their own
    /// branches in GraphicFor and must not be overwritten.
    /// </summary>
    [HarmonyPatch(typeof(PawnRenderNode_AnimalPart), nameof(PawnRenderNode_AnimalPart.GraphicFor))]
    public static class PawnRenderNode_AnimalPart_GraphicFor_Patch
    {
        public static void Postfix(Pawn pawn, ref Graphic __result)
        {
            if (__result == null) return;
            if (pawn?.kindDef == null || pawn.Dead) return;
            if (pawn.RaceProps == null || !pawn.RaceProps.Animal) return;
            if (pawn.Drawer?.renderer == null || pawn.Drawer.renderer.CurRotDrawMode != RotDrawMode.Fresh) return;

            AlternateGraphic ag = PickSkinSet(pawn);
            if (ag != null) __result = ag.GetGraphic(__result);
        }

        /// <summary>Variants by life stage and by gender.</summary>
        private static AlternateGraphic PickSkinSet(Pawn pawn)
        {
            var ext = pawn.kindDef.GetModExtension<SkinSetModExt>();
            if (ext == null) return null;

            int stageIndex = pawn.ageTracker?.CurLifeStageIndex ?? 0;
            var stage = ext.StageFor(stageIndex);
            if (stage == null) return null;

            var list = stage.ListFor(pawn.gender);
            if (list.NullOrEmpty()) return null;

            // The index is drawn once and for all on the FIRST stage, then reused as is for the later
            // ones. Without that a blue chick would turn into a black hen as it grew: the lists run in
            // parallel from one stage to the next, and the index carries the colour.
            var reference = ext.StageFor(0)?.ListFor(pawn.gender);
            if (reference.NullOrEmpty()) reference = list;

            int index = WeightedIndex(reference, pawn.thingIDNumber);
            if (index >= list.Count) index = list.Count - 1;
            return list[index];
        }

        private static int WeightedIndex(List<AlternateGraphic> list, int seed)
        {
            float total = 0f;
            for (int i = 0; i < list.Count; i++) total += Mathf.Max(0f, list[i].Weight);
            if (total <= 0f) return 0;

            // A given bird looks the same from one session to the next, and from one resolution of
            // the render tree to the next, because the seed is its own thing id.
            Rand.PushState(seed ^ 46101);
            float roll;
            try { roll = Rand.Value * total; }
            finally { Rand.PopState(); }

            float running = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                running += Mathf.Max(0f, list[i].Weight);
                if (roll < running) return i;
            }
            return list.Count - 1;
        }
    }
}
