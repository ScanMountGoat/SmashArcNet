using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FileData
    {
        public uint OffsetInFolder { get; set; }
        public uint CompSize { get; set; }
        public uint DecompSize { get; set; }
    }
}
