using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using PoultryVarietyPack;
using Verse;

// Console tests, no game running. Three claims of the mod:
//   1. the skin-set logic (stage and gender lookup, weighted and stable colour choice) behaves;
//   2. the shipped Marans XML keeps its colour lists parallel from one life stage to the next, which is
//      what keeps a bird the same colour from chick to adult;
//   3. the A Dog Said... Animal Prosthetics 2 patch, run through RimWorld's own PatchOperation, adds the
//      six birds to the right recipe lists and does nothing when that mod is absent.
// What these cannot show (the colour a living bird really draws, save and reload) stays in Pickle.
internal static class Tests
{
    private static int failures, passes;

    private sealed class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }

    private static void Test(string name, Action test)
    {
        try { test(); passes++; }
        catch (AssertionException e)
        {
            failures++;
            Console.WriteLine("FAIL  " + name + "\n      " + e.Message.Replace("\n", "\n      "));
        }
        catch (Exception e)
        {
            failures++;
            Console.WriteLine("ERROR " + name + "\n      " + e.ToString().Replace("\n", "\n      "));
        }
    }

    private static void Require(bool value, string because)
    {
        if (!value) throw new AssertionException(because);
    }

    private static void Equal<T>(T expected, T actual, string because)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new AssertionException(because + ": expected <" + expected + ">, got <" + actual + ">");
    }

    private static string ModRoot => Program.Metadata("ModRoot");

    private static readonly string[] Chickens = { "MaranChicken", "CinnamonQueen", "BielefelderChicken" };
    private static readonly string[] Ducks = { "RunnerDuck", "MuscovyDuck", "RouenDuck" };

    // ---- skin-set logic -------------------------------------------------------------------------

    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static AlternateGraphic Variant(float weight)
    {
        var ag = new AlternateGraphic();
        typeof(AlternateGraphic).GetField("weight", Any).SetValue(ag, weight);
        return ag;
    }

    private static List<AlternateGraphic> Variants(params float[] weights) => weights.Select(Variant).ToList();

    private static readonly MethodInfo WeightedIndexMethod = typeof(Main).Assembly
        .GetType("PoultryVarietyPack.PawnRenderNode_AnimalPart_GraphicFor_Patch")
        .GetMethod("WeightedIndex", Any);

    private static int WeightedIndex(List<AlternateGraphic> list, int seed) => (int)WeightedIndexMethod.Invoke(null, new object[] { list, seed });

    // ---- patch operations -----------------------------------------------------------------------

    private static bool SkipInfrastructure() => false;

    // PatchOperation.Apply opens a DeepProfiler section whose infrastructure only exists in a running
    // game; switching it off is what makes the patch tests headless.
    private static void SilenceProfiler()
    {
        var harmony = new HarmonyLib.Harmony("nelim.poultryvarietypack.tests");
        var skip = new HarmonyLib.HarmonyMethod(typeof(Tests).GetMethod(nameof(SkipInfrastructure), Any));
        foreach (var method in typeof(DeepProfiler).GetMethods(Any).Where(m => m.Name == "Start" || m.Name == "End"))
            harmony.Patch(method, prefix: skip);
    }

    private static PatchOperation ReadOperation(XmlNode node)
    {
        var cls = node.Attributes["Class"].Value;
        var type = typeof(PatchOperation).Assembly.GetType("Verse." + cls) ?? typeof(PatchOperation).Assembly.GetType(cls);
        Require(type != null, "unknown patch operation class " + cls);
        var op = (PatchOperation)Activator.CreateInstance(type);
        void Set(string field, object value)
        {
            var f = type.GetField(field, Any);
            Require(f != null, cls + " has no field " + field);
            f.SetValue(op, value);
        }
        switch (cls)
        {
            case "PatchOperationConditional":
                Set("xpath", node.SelectSingleNode("xpath").InnerText);
                var match = node.SelectSingleNode("match");
                if (match != null) Set("match", ReadOperation(match));
                var nomatch = node.SelectSingleNode("nomatch");
                if (nomatch != null) Set("nomatch", ReadOperation(nomatch));
                break;
            case "PatchOperationSequence":
                Set("operations", node.SelectNodes("operations/li").Cast<XmlNode>().Select(ReadOperation).ToList());
                break;
            case "PatchOperationAdd":
                Set("xpath", node.SelectSingleNode("xpath").InnerText);
                var container = Activator.CreateInstance(typeof(PatchOperation).Assembly.GetType("Verse.XmlContainer"));
                container.GetType().GetField("node", Any).SetValue(container, node.SelectSingleNode("value"));
                Set("value", container);
                break;
            default:
                throw new AssertionException("the test reader does not know " + cls);
        }
        return op;
    }

    private static XmlNode AdsOperation()
    {
        var doc = new XmlDocument();
        doc.Load(Path.Combine(ModRoot, "Patches", "ADogSaidAnimalProsthetics2.xml"));
        return doc.SelectSingleNode("Patch/Operation");
    }

    private static string AdsRecipes(params (string cat, string[] users)[] cats)
    {
        return "<Defs>" + string.Concat(cats.Select(c =>
            "<RecipeDef Name=\"" + c.cat + "\" Abstract=\"True\"><recipeUsers>" +
            string.Concat(c.users.Select(u => "<li>" + u + "</li>")) + "</recipeUsers></RecipeDef>")) + "</Defs>";
    }

    private static XmlDocument AdsFixture()
    {
        var doc = new XmlDocument();
        doc.LoadXml(AdsRecipes(("ADS_Cat3", new[] { "Horse" }), ("ADS_Cat2", new[] { "Cat" }), ("ADS_Cat1", new[] { "Cat", "Chicken", "Duck" })));
        return doc;
    }

    private static List<string> Users(XmlDocument doc, string cat) =>
        doc.SelectNodes("/Defs/RecipeDef[@Name=\"" + cat + "\"]/recipeUsers/li").Cast<XmlNode>().Select(n => n.InnerText).ToList();

    private static void AssertAdsExpectations(XmlDocument doc)
    {
        var cat1 = Users(doc, "ADS_Cat1"); var cat2 = Users(doc, "ADS_Cat2"); var cat3 = Users(doc, "ADS_Cat3");
        foreach (var b in Chickens.Concat(Ducks)) Require(cat1.Contains(b), b + " must join ADS_Cat1");
        foreach (var d in Ducks) Require(cat2.Contains(d), d + " must join ADS_Cat2");
        foreach (var c in Chickens) Require(!cat2.Contains(c), c + " must not join ADS_Cat2: vanilla Chicken is in Cat1 only");
        foreach (var b in Chickens.Concat(Ducks)) Require(!cat3.Contains(b), b + " must not join ADS_Cat3");
    }

    public static int Run()
    {
        SilenceProfiler();

        Test("The mod assembly loads and exposes its skin-set types", () =>
        {
            Require(typeof(SkinSetModExt) != null && WeightedIndexMethod != null, "WeightedIndex not found");
        });

        Test("StageFor clamps the stage index and honours appliesToAll", () =>
        {
            var a = new SkinSetLifeStage(); var b = new SkinSetLifeStage(); var c = new SkinSetLifeStage();
            var ext = new SkinSetModExt { lifeStages = new List<SkinSetLifeStage> { a, b, c } };
            Require(ReferenceEquals(ext.StageFor(0), a) && ReferenceEquals(ext.StageFor(2), c), "in-range lookup");
            Require(ReferenceEquals(ext.StageFor(9), c), "above range clamps to the last block");
            Require(ReferenceEquals(ext.StageFor(-3), a), "below range clamps to the first block");
            ext.appliesToAll = true;
            Require(ReferenceEquals(ext.StageFor(2), a), "appliesToAll uses the first block");
            Require(new SkinSetModExt().StageFor(0) == null, "no blocks gives null");
        });

        Test("ListFor gives females their own list only when one exists", () =>
        {
            var males = Variants(1f); var females = Variants(1f, 1f);
            var both = new SkinSetLifeStage { variants = males, variantsFemale = females };
            Require(ReferenceEquals(both.ListFor(Gender.Female), females), "female list");
            Require(ReferenceEquals(both.ListFor(Gender.Male), males), "male list");
            var only = new SkinSetLifeStage { variants = males };
            Require(ReferenceEquals(only.ListFor(Gender.Female), males), "females fall back to variants");
        });

        Test("WeightedIndex is stable for one seed and in range", () =>
        {
            var list = Variants(0.3f, 0.4f, 0.1f, 0.3f);
            for (var seed = 0; seed < 500; seed++)
            {
                var i = WeightedIndex(list, seed);
                Require(i >= 0 && i < list.Count, "index out of range for seed " + seed);
                Equal(i, WeightedIndex(list, seed), "same seed must give the same colour (seed " + seed + ")");
            }
        });

        Test("WeightedIndex follows the weights over many birds", () =>
        {
            var list = Variants(0.3f, 0.4f, 0.1f, 0.3f);
            var counts = new int[4]; const int n = 20000;
            for (var seed = 1; seed <= n; seed++) counts[WeightedIndex(list, seed)]++;
            // The weights are relative: they sum to 1.1, so the shares are weight / 1.1.
            var expected = new[] { 0.3 / 1.1, 0.4 / 1.1, 0.1 / 1.1, 0.3 / 1.1 };
            for (var i = 0; i < 4; i++)
                Require(Math.Abs(counts[i] / (double)n - expected[i]) < 0.02, "colour " + i + " drawn " + counts[i] / (double)n + ", expected " + expected[i]);
        });

        Test("WeightedIndex never picks a zero weight, and falls back to 0 when all weights are zero", () =>
        {
            var list = Variants(0f, 1f, 0f);
            for (var seed = 1; seed <= 2000; seed++) Equal(1, WeightedIndex(list, seed), "zero-weight variant drawn (seed " + seed + ")");
            Equal(0, WeightedIndex(Variants(0f, 0f), 7), "all zero");
        });

        Test("One seed draws the same index in two lists with the same weights (colour persists chick to adult)", () =>
        {
            var chick = Variants(0.3f, 0.4f, 0.1f, 0.3f); var adult = Variants(0.3f, 0.4f, 0.1f, 0.3f);
            for (var seed = 1; seed <= 2000; seed++) Equal(WeightedIndex(chick, seed), WeightedIndex(adult, seed), "seed " + seed);
        });

        // ---- shipped XML ----
        var maran = new XmlDocument();
        maran.Load(Path.Combine(ModRoot, "Defs", "Animals", "MaranChicken.xml"));

        Test("Marans: one skin-set block per race life stage, lists parallel across blocks", () =>
        {
            var stageCount = maran.SelectNodes("/Defs/ThingDef[defName='MaranChicken']/race/lifeStageAges/li").Count;
            var blocks = maran.SelectNodes("/Defs/PawnKindDef[defName='MaranChicken']/modExtensions/li[@Class='PoultryVarietyPack.SkinSetModExt']/lifeStages/li").Cast<XmlNode>().ToList();
            Equal(stageCount, blocks.Count, "skin-set blocks against race life stages");
            Require(stageCount >= 2, "the Marans must have a chick and an adult stage");
            foreach (var listName in new[] { "variants", "variantsFemale" })
            {
                var lists = blocks.Select(b => b.SelectNodes(listName + "/li").Cast<XmlNode>().ToList()).ToList();
                if (listName == "variants") Require(lists.All(l => l.Count > 0), "every block needs variants");
                var reference = lists.FirstOrDefault(l => l.Count > 0);
                if (reference == null) continue;
                foreach (var l in lists.Where(l => l.Count > 0))
                {
                    Equal(reference.Count, l.Count, listName + ": every block lists the same number of colours");
                    for (var i = 0; i < reference.Count; i++)
                        Equal(reference[i].SelectSingleNode("weight").InnerText, l[i].SelectSingleNode("weight").InnerText, listName + ": weight of colour " + i);
                }
            }
        });

        Test("Marans: weights are positive and every texture exists", () =>
        {
            foreach (XmlNode li in maran.SelectNodes("//modExtensions//variants/li | //modExtensions//variantsFemale/li"))
            {
                Require(float.Parse(li.SelectSingleNode("weight").InnerText, System.Globalization.CultureInfo.InvariantCulture) > 0f, "non-positive weight");
                var tex = li.SelectSingleNode("texPath").InnerText;
                var dir = Path.Combine(ModRoot, "Textures", Path.GetDirectoryName(tex.Replace('/', Path.DirectorySeparatorChar)));
                Require(File.Exists(Path.Combine(ModRoot, "Textures", tex.Replace('/', Path.DirectorySeparatorChar) + ".png")), "no texture for " + tex);
            }
        });

        // ---- ADS 2 patch ----
        Test("The ADS 2 patch parses and names only this mod's races", () =>
        {
            var names = AdsOperation().SelectNodes(".//value/li").Cast<XmlNode>().Select(n => n.InnerText).Distinct().OrderBy(s => s).ToList();
            Equal(string.Join(",", Chickens.Concat(Ducks).OrderBy(s => s)), string.Join(",", names), "defNames the patch adds");
            var defined = new HashSet<string>();
            foreach (var f in Directory.GetFiles(Path.Combine(ModRoot, "Defs"), "*.xml", SearchOption.AllDirectories))
            {
                var d = new XmlDocument(); d.Load(f);
                foreach (XmlNode n in d.SelectNodes("/Defs/ThingDef/defName")) defined.Add(n.InnerText);
            }
            foreach (var n in names) Require(defined.Contains(n), n + " is not a ThingDef of this mod");
        });

        Test("ADS 2 present: the six birds join the right lists (fixture)", () =>
        {
            var doc = AdsFixture();
            Require(ReadOperation(AdsOperation()).Apply(doc), "Apply reported failure");
            AssertAdsExpectations(doc);
            Require(Users(doc, "ADS_Cat1").Contains("Chicken") && Users(doc, "ADS_Cat3").Contains("Horse"), "existing entries must stay");
        });

        Test("ADS 2 present, installed copy: same result on its real file", () =>
        {
            var path = Program.Metadata("AdsCategories");
            if (!File.Exists(path)) { Console.WriteLine("SKIP  installed ADS 2 not found at " + path); return; }
            var doc = new XmlDocument(); doc.Load(path);
            Require(ReadOperation(AdsOperation()).Apply(doc), "Apply reported failure");
            AssertAdsExpectations(doc);
        });

        Test("ADS 2 absent: nothing changes and nothing fails", () =>
        {
            var doc = new XmlDocument(); doc.LoadXml("<Defs><ThingDef><defName>Other</defName></ThingDef></Defs>");
            var before = doc.OuterXml;
            Require(ReadOperation(AdsOperation()).Apply(doc), "Apply reported failure without ADS 2");
            Equal(before, doc.OuterXml, "the document must be untouched");
        });

        Test("The guard is on the recipe name: a differently named recipe list is left alone", () =>
        {
            var doc = new XmlDocument();
            doc.LoadXml(AdsRecipes(("Other_Cat1", new[] { "Cat" })));
            var before = doc.OuterXml;
            Require(ReadOperation(AdsOperation()).Apply(doc), "Apply reported failure");
            Equal(before, doc.OuterXml, "the document must be untouched");
        });

        Test("The expectations can fail: an unpatched fixture is rejected", () =>
        {
            var rejected = false;
            try { AssertAdsExpectations(AdsFixture()); } catch (AssertionException) { rejected = true; }
            Require(rejected, "AssertAdsExpectations accepted a document the patch never touched");
        });

        Console.WriteLine(failures == 0 ? "PASS  " + passes + " tests" : "FAIL  " + failures + " failed, " + passes + " passed");
        return failures == 0 ? 0 : 1;
    }
}
