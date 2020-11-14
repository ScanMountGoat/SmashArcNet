using System;
using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct DirListing
    {
        public FileNode* Ptr { get; set; }
        public UIntPtr Size { get; set; }
    }

}
