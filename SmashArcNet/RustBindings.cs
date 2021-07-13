using SmashArcNet.RustTypes;
using System;
using System.Runtime.InteropServices;

namespace SmashArcNet
{
    internal unsafe static class RustBindings
    {
        private const string nativeLib = "smash_arc";

        [DllImport(nativeLib, EntryPoint = "arc_open")]
        internal static extern IntPtr ArcOpen([MarshalAs(UnmanagedType.LPUTF8Str)] string path);
        
        [DllImport(nativeLib, EntryPoint = "arc_open_networked")]
        internal static extern IntPtr ArcOpenNetworked([MarshalAs(UnmanagedType.LPUTF8Str)] string ip);

        [DllImport(nativeLib, EntryPoint = "arc_free")]
        internal static extern void ArcFree(IntPtr arc);

        [DllImport(nativeLib, EntryPoint = "arc_list_dir")]
        internal static extern DirListing ArcListDir(IntPtr arc, Hash40 hash);

        [DllImport(nativeLib, EntryPoint = "arc_list_root_dir")]
        internal static extern DirListing ArcListRootDir(IntPtr arc);

        [DllImport(nativeLib, EntryPoint = "arc_load_labels")]
        internal static extern byte ArcLoadLabels(string path);

        [DllImport(nativeLib, EntryPoint = "arc_hash40_to_str")]
        internal static extern IntPtr ArcHash40ToString(Hash40 hash);

        [DllImport(nativeLib, EntryPoint = "arc_free_str")]
        internal static extern void ArcFreeStr(IntPtr ptr);

        // This function expects a null terminated string.
        // TODO: Does this work properly when the input string has non ANSI characters?
        [DllImport(nativeLib, EntryPoint = "arc_str_to_hash40")]
        internal static extern Hash40 ArcStrToHash40([MarshalAs(UnmanagedType.LPUTF8Str)] string str);

        [DllImport(nativeLib, EntryPoint = "arc_get_file_metadata_regional")]
        internal static unsafe extern FileMetadata ArcGetFileMetadata(IntPtr arc, Hash40 hash, Region region);

        [DllImport(nativeLib, EntryPoint = "arc_get_file_count")]
        internal static unsafe extern ulong ArcGetFileCount(IntPtr arc);

        [DllImport(nativeLib, EntryPoint = "arc_extract_file_regional")]
        internal static unsafe extern ExtractResult ArcExtractFile(IntPtr arc, Hash40 hash, [MarshalAs(UnmanagedType.LPUTF8Str)] string outputPath, Region region);

        [DllImport(nativeLib, EntryPoint = "arc_get_shared_files_regional")]
        internal static extern Hash40Vec ArcGetSharedFileList(IntPtr arc, Hash40 hash, Region region);

        [DllImport(nativeLib, EntryPoint = "arc_free_shared_file_list")]
        internal static extern void ArcFreeSharedFileList(Hash40Vec sharedFiles);

        [DllImport(nativeLib, EntryPoint = "arc_get_version")]
        internal static extern uint ArcGetVersion(IntPtr arc);

        // TODO: Free the search cache?
        [DllImport(nativeLib, EntryPoint = "arc_generate_search_cache")]
        internal static extern IntPtr ArcGenerateSearchCache(IntPtr arc);

        [DllImport(nativeLib, EntryPoint = "arc_search_files")]
        internal static extern Hash40Vec ArcSearchFiles(IntPtr searchCache, [MarshalAs(UnmanagedType.LPUTF8Str)] string searchTerm, UIntPtr maxFiles);
    }
}
