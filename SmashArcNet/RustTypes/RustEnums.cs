namespace SmashArcNet.RustTypes
{
    internal enum ExtractResult : byte
    {
        Ok = 0,
        IoError = 1,
        Missing = 2,
    }

    internal enum FileKind : ulong
    {
        Directory = 0,
        File = 1
    }
}
