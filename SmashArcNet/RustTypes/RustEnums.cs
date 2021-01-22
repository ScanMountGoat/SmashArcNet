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

    /// <summary>
    /// The language region to use for operations that involve regional files.
    /// </summary>
    public enum Region : uint
    {
        None = 0,
        Japanese = 1,
        UsEnglish = 2,
        UsFrench = 3,
        UsSpanish = 4,
        EuEnglish = 5,
        EuFrench = 6,
        EuSpanish = 7,
        EuGerman = 8,
        EuDutch = 9,
        EuItalian = 10,
        EuRussian = 11,
        Korean = 12,
        ChinaChinese = 13,
        TaiwanChinese = 14,
    }
}
