using SmashArcNet.RustTypes;

namespace SmashArcNet.Nodes
{
    /// <summary>
    /// A directory listing in the ARC.
    /// </summary>
    public sealed class ArcDirectoryNode : IArcNode
    {
        /// <summary>
        /// The absolute path of the directory. This may contain a trailing slash.
        /// Examples: "a/b/c/", "a/b/"
        /// </summary>
        public string Path { get; }

        internal Hash40 PathHash { get; }

        internal ArcDirectoryNode(string path, Hash40 pathHash)
        {
            Path = path;
            PathHash = pathHash;
        }

        /// <summary>
        /// example: "a/b/c/"
        /// </summary>
        /// <returns>The string representation of this <see cref="ArcDirectoryNode"/></returns>
        public override string ToString()
        {
            return Path;
        }
    }
}
