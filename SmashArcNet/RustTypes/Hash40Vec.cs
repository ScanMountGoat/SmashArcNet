using System;
using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Hash40Vec
    {
        public Hash40* Ptr { get; set; }
        public UIntPtr Size { get; set; }
    }
}
