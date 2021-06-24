using SmashArcNet.RustTypes;

namespace SmashArcNet
{
    /// <summary>
    /// Contains methods for dealing with path hashes.
    /// </summary>
    public static class HashLabels
    {
        /// <summary>
        /// <c>true</c> if hashes were loaded successfully by the last call to <see cref="TryLoadHashes(string)"/>.
        /// </summary>
        public static bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes the hash dictionary from a path pointing to a line separated list of strings to hash.
        /// </summary>
        /// <param name="path">the text file path containing the strings to hash</param>
        /// <returns><c>true</c> if the hash labels were loaded successfully</returns>
        public static bool TryLoadHashes(string path)
        {
            // This may fail.
            IsInitialized = RustBindings.ArcLoadLabels(path) != 0;
            return IsInitialized;
        }
    }
}
