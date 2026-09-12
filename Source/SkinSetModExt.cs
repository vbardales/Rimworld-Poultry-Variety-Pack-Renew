using System.Collections.Generic;
using Verse;

namespace PoultryVarietyPack
{
    /// <summary>
    /// Texture variants that depend on the life stage AND on the gender.
    ///
    /// Vanilla cannot do it: PawnKindDef.alternateGraphics is a flat list whose every entry fixes a
    /// single texPath, applied as is from the chick to the adult. Yet the Marans of this mod come in
    /// four colours, each with a chick texture, a rooster one and a hen one.
    ///
    /// That is what AnimalVariations.dll did through its *_SkinSet.xml files, third-party and with no
    /// sources, which does not survive the render tree RimWorld moved to in 1.5. This extension takes
    /// over its semantics in def XML, with no foreign DLL.
    ///
    /// The namespace is this mod's own rather than the pack's, on purpose: the private pack this port
    /// came out of still ships the same class under Bastyon.SkinSetModExt, and two assemblies exposing
    /// one fully qualified name leave GenTypes to pick between them.
    /// </summary>
    public class SkinSetModExt : DefModExtension
    {
        /// <summary>One block per life stage, in the order of the PawnKindDef lifeStages.</summary>
        public List<SkinSetLifeStage> lifeStages;

        /// <summary>If true, the first block applies to every stage.</summary>
        public bool appliesToAll = false;

        public SkinSetLifeStage StageFor(int lifeStageIndex)
        {
            if (lifeStages.NullOrEmpty()) return null;
            if (appliesToAll) return lifeStages[0];
            if (lifeStageIndex < 0) lifeStageIndex = 0;
            if (lifeStageIndex >= lifeStages.Count) lifeStageIndex = lifeStages.Count - 1;
            return lifeStages[lifeStageIndex];
        }
    }

    public class SkinSetLifeStage
    {
        public List<AlternateGraphic> variants;

        /// <summary>Optional: when empty, females use <see cref="variants"/>.</summary>
        public List<AlternateGraphic> variantsFemale;

        public List<AlternateGraphic> ListFor(Gender gender)
        {
            if (gender == Gender.Female && !variantsFemale.NullOrEmpty()) return variantsFemale;
            return variants;
        }
    }
}
