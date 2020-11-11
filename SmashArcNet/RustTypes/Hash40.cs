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

        public static implicit operator ulong(Hash40 h) => h.Value;
        public static implicit operator Hash40(ulong u) => new Hash40(u);

        public Hash40(ulong value)
        {
            Value = value;
        }
    }
}
