using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FileNode
    {
        public ulong Kind { get; set; }
        public ulong Hash { get; set; }
    }
}
