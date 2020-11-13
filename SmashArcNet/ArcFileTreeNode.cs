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

        /// <summary>
        /// The file data's offset in bytes
        /// </summary>
        public ulong Offset { get; }

        /// <summary>
        /// The size of the file data in bytes.
        /// </summary>
        public ulong CompSize { get; }

        /// <summary>
        /// The size of the file data in bytes after being decompressed.
        /// </summary>
        public ulong DecompSize { get; }

        /// <summary>
        /// <c>true</c> if the file is a stream file
        /// </summary>
        public bool IsStream { get; }

        /// <summary>
        /// <c>true</c> if the file data is shared with other files.
        /// </summary>
        public bool IsShared { get; }

        /// <summary>
        /// <c>true</c> if the file is redirected.
        /// </summary>
        public bool IsRedirect { get; }

        /// <summary>
        /// <c>true</c> if the file regional.
        /// </summary>
        public bool IsRegional { get; }

        /// <summary>
        /// <c>true</c> if the file is localized.
        /// </summary>
        public bool IsLocalized { get; }

        /// <summary>
        /// <c>true</c> if the file is compressed.
        /// </summary>
        public bool IsCompressed { get; }

        /// <summary>
        /// <c>true</c> if the file uses zstd compression.
        /// </summary>
        public bool UsesZstd { get; }

        internal Hash40 PathHash { get; }

        internal ArcFileTreeNode(FileType type, string path, Hash40 pathHash, FileMetadata fileMetadata)
        {
            PathHash = pathHash;
            Type = type;
            Path = path;

            Offset = fileMetadata.Offset;
            CompSize = fileMetadata.CompSize;
            DecompSize = fileMetadata.DecompSize;
            IsStream = fileMetadata.IsStream != 0;
            IsShared = fileMetadata.IsShared != 0;
            IsRedirect = fileMetadata.IsRedirect != 0;
            IsRegional = fileMetadata.IsRegional != 0;
            IsLocalized = fileMetadata.IsLocalized != 0;
            IsCompressed = fileMetadata.IsCompressed != 0;
            UsesZstd = fileMetadata.UsesZstd != 0;
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
