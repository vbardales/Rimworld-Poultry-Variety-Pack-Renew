using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

internal static class Program
{
    internal static string Metadata(string key) => Assembly.GetExecutingAssembly()
        .GetCustomAttributes<AssemblyMetadataAttribute>().Single(a => a.Key == key).Value;

    private static int Main()
    {
        // The game's assemblies and the mod's own are resolved out of their folders, which keeps them
        // out of the build output.
        AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
        {
            var name = new AssemblyName(e.Name).Name;
            var path = Path.Combine(Metadata("RimWorldManaged"), name + ".dll");
            if (File.Exists(path)) return Assembly.LoadFrom(path);
            var mod = Metadata("ModAssembly");
            return name == "PoultryVarietyPack" && File.Exists(mod) ? Assembly.LoadFrom(mod) : null;
        };
        return Run();
    }

    // Kept out of Main so the resolver is installed before Tests is jitted.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Run() => Tests.Run();
}
