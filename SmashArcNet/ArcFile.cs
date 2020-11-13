using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SmashArcNet.RustTypes;

namespace SmashArcNet
{
    /// <summary>
    /// A safe wrapper for the ARC format.
    /// </summary>
    public class ArcFile
    {
        private readonly IntPtr arcPtr;

        private ArcFile(IntPtr arcPtr)
        {
            this.arcPtr = arcPtr;
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
        /// Gets the child nodes of the ARC sorted in ascending alphabetical order.
        /// These will mostly likely be <see cref="ArcFileTreeNode.FileType.Directory"/> (ex: "fighter/").
        /// </summary>
        /// <returns>the child nodes of ARC root</returns>
        public List<ArcFileTreeNode> GetRootNodes()
        {
            var listing = RustBindings.ArcListRootDir(arcPtr);
            return CreateArcNodes(listing)
                .OrderBy(n => n.Path)
                .ToList();
        }

        /// <summary>
        /// Gets the children of <paramref name="parent"/> sorted in ascending alphabetical order,
        /// which may be <see cref="ArcFileTreeNode.FileType.Directory"/> or <see cref="ArcFileTreeNode.FileType.File"/>.
        /// The resulting list will be empty if there are no children.
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <returns>the child nodes of <paramref name="parent"/></returns>
        public List<ArcFileTreeNode> GetChildren(ArcFileTreeNode parent)
        {
            var listing = RustBindings.ArcListDir(arcPtr, parent.PathHash);
            return CreateArcNodes(listing)
                .OrderBy(n => n.Path)
                .ToList();
        }

        private unsafe List<ArcFileTreeNode> CreateArcNodes(DirListing listing)
        {
            var nodes = new List<ArcFileTreeNode>();

            if (listing.Ptr != null)
            {
                // Assume that listing size doesn't take more than 32 bits.
                // The size limit for List is only Int32.MaxValue.
                for (var i = 0; i < listing.Size.ToUInt32(); i++)
                {
                    var node = CreateFileNode(listing.Ptr[i]);
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        private ArcFileTreeNode CreateFileNode(FileNode fileNode)
        {
            var isFile = fileNode.Kind == 1;

            if (isFile)
            {
                var data = RustBindings.ArcGetFileMetadata(arcPtr, fileNode.Hash);

                // Recreate the absolute hash from the filename and parent directory.
                // This allows for using the smaller hash file.
                var parent = RustBindings.ArcHash40ToString(data.ParentHash);
                var name = RustBindings.ArcHash40ToString(data.FileNameHash);
                var filePath = System.IO.Path.Combine(parent ?? "", name ?? "");

                return new ArcFileTreeNode(isFile ? ArcFileTreeNode.FileType.File : ArcFileTreeNode.FileType.Directory, filePath, fileNode.Hash, data);
            }

            var dirPath = RustBindings.ArcHash40ToString(fileNode.Hash) ?? fileNode.Hash.ToString("x");
            return new ArcFileTreeNode(isFile ? ArcFileTreeNode.FileType.File : ArcFileTreeNode.FileType.Directory, dirPath, fileNode.Hash, new FileMetadata());
        }
    }
}
