using SmashArcNet.RustTypes;

namespace SmashArcNet.Nodes
{
    /// <summary>
    /// A file listing in the ARC.
    /// </summary>
    public sealed class ArcFileNode : IArcNode
    {
        /// <summary>
        /// The absolute path of the file or the full path hash in hex if the file name or parent hash label is not found.
        /// Ex: "fighter/mario/model/body/c00/model.numatb" or "0x29954022ed"
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The name of the file without its parent directory or the hash in hex if no label is found. 
        /// Ex: "model.numatb" or "0xcb3406d79" 
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// The file extension or the extension hash in hex if no label is found.
        /// Ex: "numatb" or "0x6dab89279"
        /// </summary>
        public string Extension { get; }

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

        internal ArcFileNode(string path, string fileName, string extension, Hash40 pathHash, FileMetadata fileMetadata)
        {
            PathHash = pathHash;
            Path = path;
            Extension = extension;
            FileName = fileName;

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
        /// example: "a/b/c/file.txt"
        /// </summary>
        /// <returns>The string representation of this <see cref="ArcFileNode"/></returns>
        public override string ToString()
        {
            return Path;
        }
    }
}
