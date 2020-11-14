﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SmashArcNet.RustTypes;
using SmashArcNet.Nodes;

namespace SmashArcNet
{
    /// <summary>
    /// A safe wrapper for the ARC format. Make sure to call <see cref="HashLabels.Initialize(string)"/> before trying to load an ARC.
    /// </summary>
    public sealed class ArcFile
    {
        /// <summary>
        /// The total number of file entries in the arc.
        /// </summary>
        public ulong FileCount { get; }

        private readonly IntPtr arcPtr;

        private ArcFile(IntPtr arcPtr)
        {
            this.arcPtr = arcPtr;
            FileCount = RustBindings.ArcGetFileCount(arcPtr);
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
        /// </summary>
        /// <param name="path">The data.arc file path</param>
        /// <param name="arcFile">The resulting ARC</param>
        /// <returns><c>true</c> if the ARC file was opened successfully</returns>
        public static bool TryOpenArc(string path, [NotNullWhen(true)] out ArcFile? arcFile)
        {
            var arcPtr = RustBindings.ArcOpen(path);
            if (arcPtr == IntPtr.Zero)
            {
                arcFile = null;
                return false;
            }

            arcFile = new ArcFile(arcPtr);
            return true;
        }
        /// <summary>
        /// Tries to create <paramref name="arcFile"/> from <paramref name="ip"/>.
        /// </summary>
        /// <param name="ip">IP address of console</param>
        /// <param name="arcFile">The resulting ARC</param>
        /// <returns><c>true</c> if the ARC file was opened successfully</returns>
        public static bool TryOpenArcNetworked(string ip, [NotNullWhen(true)] out ArcFile? arcFile)
        {
            var arcPtr = RustBindings.ArcOpenNetworked(ip);
            if (arcPtr == IntPtr.Zero)
            {
                arcFile = null;
                return false;
            }

            arcFile = new ArcFile(arcPtr);
            return true;
        }

        /// <summary>
        /// Tries to extract the uncompressed contents of <paramref name="file"/> to <paramref name="outputPath"/>.
        /// </summary>
        /// <param name="file">The file node to extract</param>
        /// <param name="outputPath">The destination file for the extracted contents</param>
        /// <returns><c>true</c> if the file was extracted succesfully</returns>
        public bool TryExtractFile(ArcFileNode file, string outputPath)
        {
            // TODO: Throw exception or return an enum containing the error?
            return RustBindings.ArcExtractFile(arcPtr, file.PathHash, outputPath) == ExtractResult.Ok;
        }

        /// <summary>
        /// Gets the child nodes of the ARC sorted in ascending alphabetical order.
        /// These will mostly likely be <see cref="ArcDirectoryNode"/> (ex: "fighter/").
        /// </summary>
        /// <returns>the child nodes of ARC root</returns>
        public List<IArcNode> GetRootNodes()
        {
            var listing = RustBindings.ArcListRootDir(arcPtr);
            return GetListingNodes(listing)
                .OrderBy(n => n.Path)
                .ToList();
        }

        /// <summary>
        /// Gets the children of <paramref name="parent"/> sorted in ascending alphabetical order,
        /// The resulting list will be empty if there are no children.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <returns>the child nodes of <paramref name="parent"/></returns>
        public List<IArcNode> GetChildren(ArcDirectoryNode parent)
        {
            var listing = RustBindings.ArcListDir(arcPtr, parent.PathHash);
            return GetListingNodes(listing)
                .OrderBy(n => n.Path)
                .ToList();
        }

        private unsafe List<IArcNode> GetListingNodes(DirListing listing)
        {
            var nodes = new List<IArcNode>();

            if (listing.Ptr != null)
            {
                // Assume that listing size doesn't take more than 32 bits.
                // The size limit for List is only Int32.MaxValue.
                for (var i = 0; i < listing.Size.ToUInt32(); i++)
                {
                    var node = CreateNode(listing.Ptr[i]);
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        private IArcNode CreateNode(FileNode fileNode)
        {
            var isFile = fileNode.Kind == 1;

            if (isFile)
            {
                var data = RustBindings.ArcGetFileMetadata(arcPtr, fileNode.Hash);

                // Recreate the absolute hash from the filename and parent directory.
                // This allows for using the smaller hash file.
                var parent = GetString(data.ParentHash);
                var name = GetString(data.FileNameHash);
                var filePath = System.IO.Path.Combine(parent ?? "", name ?? "");

                return new ArcFileNode(filePath, fileNode.Hash, data);
            }

            var dirPath = GetString(fileNode.Hash) ?? fileNode.Hash.ToString("x");
            return new ArcDirectoryNode(dirPath, fileNode.Hash);
        }

        private static string GetString(Hash40 hash)
        {
            // Make sure Rust frees the string.
            IntPtr ptr = RustBindings.ArcHash40ToString(hash);

            var str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);

            // Rust returns null if the string isn't found.
            if (ptr != IntPtr.Zero)
                RustBindings.ArcFreeStr(ptr);

            return str ?? "";
        }
    }
}
