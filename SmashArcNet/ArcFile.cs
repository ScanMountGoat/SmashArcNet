using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SmashArcNet.RustTypes;
using SmashArcNet.Nodes;
using System.IO;
using System.Runtime.InteropServices;

namespace SmashArcNet
{
    /// <summary>
    /// A safe wrapper for the ARC format. 
    /// </summary>
    public sealed class ArcFile
    {
        /// <summary>
        /// The total number of file entries in the arc.
        /// </summary>
        public ulong FileCount { get; }

        /// <summary>
        /// The file version of the ARC.
        /// </summary>
        public uint Version { get; }

        private readonly IntPtr arcPtr;

        private readonly IntPtr searchCachePtr;

        private ArcFile(IntPtr arcPtr, IntPtr searchCachePtr)
        {
            this.arcPtr = arcPtr;
            this.searchCachePtr = searchCachePtr;
            FileCount = RustBindings.ArcGetFileCount(arcPtr);
            Version = (RustBindings.ArcGetVersion(arcPtr) & 0xF0000) >> 16;
        }

        /// <summary>
        /// Frees the resources associated with this ARC
        /// </summary>
        ~ArcFile()
        {
            // Free memory on the Rust side.
            RustBindings.ArcFree(arcPtr);
        }

        /// <summary>
        /// Tries to create <paramref name="arcFile"/> from <paramref name="path"/>.
        /// Make sure to call <see cref="HashLabels.TryLoadHashes(string)"/> before trying to load an ARC.
        /// </summary>
        /// <param name="path">The data.arc file path</param>
        /// <param name="arcFile">The resulting ARC</param>
        /// <returns><c>true</c> if the ARC file was opened successfully</returns>
        public static bool TryOpenArc(string path, [NotNullWhen(true)] out ArcFile? arcFile)
        {
            // TODO: A lot of this code is duplicated.
            // Pass the open function as a parameter?
            if (!HashLabels.IsInitialized || string.IsNullOrEmpty(path))
            {
                arcFile = null;
                return false;
            }

            var arcPtr = RustBindings.ArcOpen(path);
            if (arcPtr == IntPtr.Zero)
            {
                arcFile = null;
                return false;
            }

            var searchCachePtr = RustBindings.ArcGenerateSearchCache(arcPtr);
            if (searchCachePtr == IntPtr.Zero)
            {
                arcFile = null;
                return false;
            }

            arcFile = new ArcFile(arcPtr, searchCachePtr);
            return true;
        }

        /// <summary>
        /// Tries to create <paramref name="arcFile"/> from <paramref name="ipAddress"/>.
        /// Make sure to call <see cref="HashLabels.TryLoadHashes(string)"/> before trying to load an ARC.
        /// </summary>
        /// <param name="ipAddress">IP address of the Switch console</param>
        /// <param name="arcFile">The resulting ARC</param>
        /// <returns><c>true</c> if the ARC file was opened successfully</returns>
        public static bool TryOpenArcNetworked(string ipAddress, [NotNullWhen(true)] out ArcFile? arcFile)
        {
            if (!HashLabels.IsInitialized || string.IsNullOrEmpty(ipAddress))
            {
                arcFile = null;
                return false;
            }

            var arcPtr = RustBindings.ArcOpenNetworked(ipAddress);
            if (arcPtr == IntPtr.Zero)
            {
                arcFile = null;
                return false;
            }

            var searchCachePtr = RustBindings.ArcGenerateSearchCache(arcPtr);
            if (searchCachePtr == IntPtr.Zero)
            {
                arcFile = null;
                return false;
            }

            arcFile = new ArcFile(arcPtr, searchCachePtr);
            return true;
        }

        /// <summary>
        /// Tries to extract the uncompressed contents of <paramref name="file"/> to <paramref name="outputPath"/>.
        /// The region is set to <see cref="Region.UsEnglish"/>.
        /// </summary>
        /// <param name="file">The file node to extract</param>
        /// <param name="outputPath">The destination file for the extracted contents</param>
        /// <returns><c>true</c> if the file was extracted succesfully</returns>
        public bool TryExtractFile(ArcFileNode file, string outputPath)
        {
            // TODO: Throw exception or return an enum containing the error?
            return RustBindings.ArcExtractFile(arcPtr, file.PathHash, outputPath, Region.UsEnglish) == ExtractResult.Ok;
        }

        /// <summary>
        /// Tries to extract the uncompressed contents of <paramref name="file"/> to <paramref name="outputPath"/> for the specified <paramref name="region"/>.
        /// </summary>
        /// <param name="file">The file node to extract</param>
        /// <param name="outputPath">The destination file for the extracted contents</param>
        /// <param name="region">The regional variant of each file to use</param>
        /// <returns><c>true</c> if the file was extracted succesfully</returns>
        public bool TryExtractFile(ArcFileNode file, string outputPath, Region region)
        {
            // TODO: Throw exception or return an enum containing the error?
            return RustBindings.ArcExtractFile(arcPtr, file.PathHash, outputPath, region) == ExtractResult.Ok;
        }

        /// <summary>
        /// Gets the child nodes of the ARC sorted in ascending alphabetical order.
        /// These will mostly likely be <see cref="ArcDirectoryNode"/> (ex: "fighter/").
        /// The region is set to <see cref="Region.UsEnglish"/>.
        /// </summary>
        /// <returns>the child nodes of ARC root</returns>
        public List<IArcNode> GetRootNodes()
        {
            var listing = RustBindings.ArcListRootDir(arcPtr);
            return GetListingNodes(listing, Region.UsEnglish)
                .OrderBy(n => n.Path)
                .ToList();
        }

        /// <summary>
        /// Gets the child nodes of the ARC sorted in ascending alphabetical order for the specified <paramref name="region"/>.
        /// These will mostly likely be <see cref="ArcDirectoryNode"/> (ex: "fighter/").
        /// The region is set to <see cref="Region.UsEnglish"/>.
        /// </summary>
        /// <param name="region">The regional variant of each file to use</param>
        /// <returns>the child nodes of ARC root</returns>
        public List<IArcNode> GetRootNodes(Region region)
        {
            var listing = RustBindings.ArcListRootDir(arcPtr);
            return GetListingNodes(listing, region)
                .OrderBy(n => n.Path)
                .ToList();
        }

        /// <summary>
        /// Gets the children of <paramref name="parent"/> sorted in ascending alphabetical order.
        /// The resulting list will be empty if there are no children.
        /// The region is set to <see cref="Region.UsEnglish"/>.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <returns>the child nodes of <paramref name="parent"/></returns>
        public List<IArcNode> GetChildren(ArcDirectoryNode parent)
        {
            var listing = RustBindings.ArcListDir(arcPtr, parent.PathHash);
            return GetListingNodes(listing, Region.UsEnglish)
                .OrderBy(n => n.Path)
                .ToList();
        }

        /// <summary>
        /// Gets the children of <paramref name="parent"/> sorted in ascending alphabetical order
        /// for the specified <paramref name="region"/>.
        /// The resulting list will be empty if there are no children.
        /// The region is set to <see cref="Region.UsEnglish"/>.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="region">The regional variant of each file to use</param>
        /// <returns>the child nodes of <paramref name="parent"/></returns>
        public List<IArcNode> GetChildren(ArcDirectoryNode parent, Region region)
        {
            var listing = RustBindings.ArcListDir(arcPtr, parent.PathHash);
            return GetListingNodes(listing, region)
                .OrderBy(n => n.Path)
                .ToList();
        }

        /// <summary>
        /// Finds the files that share their data with <paramref name="file"/>.
        /// The region is set to <see cref="Region.UsEnglish"/>.
        /// </summary>
        /// <param name="file">The file node to search</param>
        /// <returns>A list of file paths that share this file's data</returns>
        public List<string> GetSharedFilePaths(ArcFileNode file)
        {
            return GetSharedFilePaths(file.PathHash, Region.UsEnglish);
        }

        /// <summary>
        /// Finds the files that share their data with <paramref name="file"/> 
        /// for the speciied <paramref name="region"/>.
        /// </summary>
        /// <param name="file">The file node to search</param>
        /// <param name="region">The regional variant of each file to use</param>
        /// <returns>A list of file paths that share this file's data</returns>
        public List<string> GetSharedFilePaths(ArcFileNode file, Region region)
        {
            return GetSharedFilePaths(file.PathHash, region);
        }

        /// <summary>
        /// Searches the entire ARC using a fuzzy file path search.
        /// Results are ordered based on how closely they match <paramref name="searchTerm"/>,
        /// and the top <paramref name="maxFiles"/> results are returned.
        /// </summary>
        /// <param name="searchTerm">The term to search for</param>
        /// <param name="maxFiles">The maximum number of results to return</param>
        /// <param name="region">The regional variant of each file to use</param>
        /// <returns>The matching file paths</returns>
        public List<string> SearchFiles(string searchTerm, ulong maxFiles, Region region)
        {
            var listing = RustBindings.ArcSearchFiles(searchCachePtr, searchTerm, new UIntPtr(maxFiles));

            // TODO: Sort?
            return GetPathsFromHash40Vec(listing, region).ToList();
        }

        /// <summary>
        /// Creates an ARC node from <paramref name="path"/> using the specified <paramref name="region"/>.
        /// The returned <see cref="IArcNode"/> is not guaranteed to refer to a valid entry in the ARC.
        /// </summary>
        /// <param name="path">The absolute path of the file or directory</param>
        /// <param name="region"></param>
        /// <returns>An <see cref="IArcNode"/> representing <paramref name="path"/></returns>
        public IArcNode CreateNode(string path, Region region)
        {
            // TODO: This might not be the best way to check for directories vs files.
            var hash = RustBindings.ArcStrToHash40(path);
            if (Path.HasExtension(path))
            {
                var data = RustBindings.ArcGetFileMetadata(arcPtr, hash, region);

                var paths = GetPaths(data);
                return new ArcFileNode(paths.Item1, paths.Item2, paths.Item3, hash, data);
            }
            else
            {
                return new ArcDirectoryNode(path, hash);
            }
        }

        private unsafe List<IArcNode> GetListingNodes(DirListing listing, Region region)
        {
            var nodes = new List<IArcNode>();

            if (listing.Ptr != null)
            {
                // Assume that listing size doesn't take more than 32 bits.
                // The size limit for List is only Int32.MaxValue.
                for (var i = 0; i < listing.Size.ToUInt32(); i++)
                {
                    var node = CreateNode(listing.Ptr[i], region);
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        private IArcNode CreateNode(FileNode fileNode, Region region)
        {
            if (fileNode.Kind == FileKind.File)
            {
                var data = RustBindings.ArcGetFileMetadata(arcPtr, fileNode.Hash, region);

                var paths = GetPaths(data);
                return new ArcFileNode(paths.Item1, paths.Item2, paths.Item3, fileNode.Hash, data);
            }

            // The expected behavior is to see the full path hash as a hex string if a label isn't found.
            var dirPath = GetPathString(fileNode.Hash);
            return new ArcDirectoryNode(dirPath, fileNode.Hash);
        }

        private static (string, string, string) GetPaths(FileMetadata data)
        {
            // Stream files don't specify an extension and filename hash.
            // The smaller hashes file contains the full absolute path for stream files to account for this.
            if (data.IsStream != 0)
            {
                var streamFilePath = GetPathString(data.PathHash);
                var streamFileName = Path.GetFileName(streamFilePath) ?? "";
                // ARC extensions don't contain the '.' at the beginning. 
                var streamFileExtension = Path.GetExtension(streamFileName)?.Replace(".", "") ?? "";
                return (streamFilePath, streamFileName, streamFileExtension);
            }

            // Recreate the absolute hash from the filename and parent directory.
            // This allows for using the smaller hash file.
            string parent = GetPathString(data.ParentHash);
            string name = GetPathString(data.FileNameHash);
            string extension = GetPathString(data.ExtHash);

            // Always combine the parent and file name to get the full path. 
            // Both hashes present: "a/b/c/d.ext"
            // Parent missing: "0x..../d.ext"
            // File missing: "a/b/c/0x..."
            var filePath = Path.Combine(parent, name);
            return (filePath, name, extension);
        }

        private unsafe List<string> GetSharedFilePaths(Hash40 hash, Region region)
        {

            // Assume that list size doesn't take more than 32 bits.
            // The size limit for List is only Int32.MaxValue.
            var sharedFileList = RustBindings.ArcGetSharedFileList(arcPtr, hash, region);
            var sharedPaths = GetPathsFromHash40Vec(sharedFileList, region);

            return sharedPaths;
        }

        private unsafe List<string> GetPathsFromHash40Vec(Hash40Vec hashVec, Region region)
        {
            var values = new List<string>();
            if (hashVec.Ptr != null)
            {
                for (int i = 0; i < hashVec.Size.ToUInt32(); i++)
                {
                    // The returned hash is the full path of the node, so recreate the path from the metadata hashes.
                    var data = RustBindings.ArcGetFileMetadata(arcPtr, hashVec.Ptr[i], region);
                    var paths = GetPaths(data);
                    values.Add(paths.Item1);
                }
                RustBindings.ArcFreeSharedFileList(hashVec);
            }

            return values;
        }

        private static string GetPathString(Hash40 hash)
        {
            // Make sure Rust frees the string.
            IntPtr ptr = RustBindings.ArcHash40ToString(hash);

            var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);

            // Rust returns null if the string isn't found.
            if (ptr != IntPtr.Zero)
                RustBindings.ArcFreeStr(ptr);

            return str ?? $"0x{hash.Value:x10}";
        }
    }
}
