namespace SmashArcNet
{
    /// <summary>
    /// Contains methods for dealing with path hashes.
    /// </summary>
    public static class HashLabels
    {
        // TODO: How to ensure this gets called?

        /// <summary>
        /// Initializes the hash dictionary from a path pointing to a line separated list of strings to hash.
        /// </summary>
        /// <param name="path">the text file path containing the strings to hash</param>
        public static void Initialize(string path) => RustBindings.ArcLoadLabels(path);
    }
}
