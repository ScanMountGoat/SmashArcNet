using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FileMetadata
    {
        public Hash40 PathHash { get; set; }
        public Hash40 ExtHash { get; set; }
        public Hash40 ParentHash { get; set; }
        public Hash40 FileNameHash { get; set; }
        public ulong Offset { get; set; }
        public ulong CompSize { get; set; }
        public ulong DecompSize { get; set; }
        public bool IsStream { get; set; }
        public bool IsShared { get; set; }
        public bool IsRedirect { get; set; }
        public bool IsRegional { get; set; }
        public bool IsLocalized { get; set; }
        public bool IsCompressed { get; set; }
        public bool UsesZstd { get; set; }
    }
}
