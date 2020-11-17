using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FileNode
    {
        public FileKind Kind { get; set; }
        public Hash40 Hash { get; set; }
    }
}
