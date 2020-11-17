using System.Runtime.InteropServices;

namespace SmashArcNet.RustTypes
{
    /// <summary>
    /// A CRC32 hash with a specified length.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Hash40
    {
        public ulong Value { get; }

        public Hash40(ulong value)
        {
            Value = value;
        }
    }
}
