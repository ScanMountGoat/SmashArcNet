using SmashArcNet;
using System;

namespace SmashArcNetCLI
{
    static class Program
    {
        private static void RecurseOverTree(ArcFile arc, ArcFileTreeNode node)
        {
            foreach (var child in arc.GetChildren(node))
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

            HashLabels.Initialize(args[0]);

            //if (!ArcFile.TryOpenArcNetworked(args[1], out ArcFile? arcFile))
            if (!ArcFile.TryOpenArc(args[1], out ArcFile? arcFile))
            {
                Console.WriteLine("Failed to open arc");
                return;
            }

            foreach (var node in arcFile.GetRootNodes())
            {
                Console.WriteLine(node);
                RecurseOverTree(arcFile, node);
            }
        }
    }
}
