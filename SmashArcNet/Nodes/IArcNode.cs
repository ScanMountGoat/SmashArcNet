namespace SmashArcNet.Nodes
{
    /// <summary>
    /// A file or directory listing in the ARC.
    /// </summary>
    public interface IArcNode
    {       
        /// <summary>
        /// The absolute path of the directory or file.
        /// </summary>
        public string Path { get; }
    }
}
