using SmashArcNet;
using System;
using SmashArcNet.Nodes;

namespace SmashArcNetCLI
{
    static class Program
    {
        private static void RecurseOverTree(ArcFile arc, IArcNode node)
        {
            if (!(node is ArcDirectoryNode directory))
                return;

            foreach (var child in arc.GetChildren(directory))
            {
                Console.WriteLine($"{child}");
                RecurseOverTree(arc, child);
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: SmashArcNetCLI <Hashes> <data.arc>");
                return;
            }

            HashLabels.TryLoadHashes(args[0]);

            if (!ArcFile.TryOpenArc(args[1], out ArcFile? arcFile))
            {
                Console.WriteLine("Failed to open ARC.");
                return;
            }

            Console.WriteLine($"ARC Version: {arcFile.Version}, File Count: {arcFile.FileCount}");

            foreach (var node in arcFile.GetRootNodes())
            {
                Console.WriteLine(node);
                RecurseOverTree(arcFile, node);
            }
        }
    }
}
