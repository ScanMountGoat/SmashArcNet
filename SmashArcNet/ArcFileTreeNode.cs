using SmashArcNet.RustTypes;

namespace SmashArcNet
{
    /// <summary>
    /// A directory or file listing in the ARC.
    /// </summary>
    public class ArcFileTreeNode
    {
        /// <summary>
        /// Indicates whether the node is a directory or file.
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// A directory that will likely contain child nodes.
            /// </summary>
            Directory,

            /// <summary>
            /// A file node that will likely have exportable contents and have no children.
            /// </summary>
            File
        }

        /// <summary>
        /// The type of file.
        /// </summary>
        public FileType Type { get; }

        /// <summary>
        /// The absolute path of the directory or file.
        /// </summary>
        public string Path { get; }

        public ulong Offset { get; }
        public ulong CompSize { get; }
        public ulong DecompSize { get; }
        public bool IsStream { get; }
        public bool IsShared { get; }
        public bool IsRedirect { get; }
        public bool IsRegional { get; }
        public bool IsLocalized { get; }
        public bool IsCompressed { get; }
        public bool UsesZstd { get; }

        internal Hash40 PathHash { get; }

        internal ArcFileTreeNode(FileType type, string path, Hash40 pathHash, FileMetadata fileMetadata)
        {
            PathHash = pathHash;
            Type = type;
            Path = path;

            // TODO: Get file path and extension.
            // TODO: Use separate types?
            Offset = fileMetadata.Offset;
            CompSize = fileMetadata.CompSize;
            DecompSize = fileMetadata.DecompSize;
            IsStream = fileMetadata.IsStream;
            IsShared = fileMetadata.IsShared;
            IsRedirect = fileMetadata.IsRedirect;
            IsRegional = fileMetadata.IsRegional;
            IsLocalized = fileMetadata.IsLocalized;
            IsCompressed = fileMetadata.IsCompressed;
            UsesZstd = fileMetadata.UsesZstd;
        }

        /// <summary>
        /// The string representation of this <see cref="ArcFileTreeNode"/>
        /// <para></para>
        /// examples: "F file.txt", "D dir/dir2"
        /// </summary>
        /// <returns>the type and path as a string</returns>
        public override string ToString()
        {
            return $"{(Type == FileType.Directory ? "D" : "F")} {Path}";
        }
    }
}
