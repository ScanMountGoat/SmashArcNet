using System;
using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct DirListing
    {
        // TODO: Will be null if directory listing failed
        public FileNode* Ptr { get; set; }
        // TODO: Is it safe to use ulong (usize in Rust)?
        public UIntPtr Size { get; set; }
    }

}
