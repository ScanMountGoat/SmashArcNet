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
        // C# bools aren't converting properly from Rust, so use byte instead.
        public byte IsStream { get; set; }
        public byte IsShared { get; set; }
        public byte IsRedirect { get; set; }
        public byte IsRegional { get; set; }
        public byte IsLocalized { get; set; }
        public byte IsCompressed { get; set; }
        public byte UsesZstd { get; set; }
    }
}
