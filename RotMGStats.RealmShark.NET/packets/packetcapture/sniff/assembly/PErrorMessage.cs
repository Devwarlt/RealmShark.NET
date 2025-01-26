namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    /// <summary>
    /// Called on errors when constructing the TCP stream.
    /// </summary>
    public interface PErrorMessage
    {
        /// <summary>
        /// Called when error messages are sent.
        /// </summary>
        /// <param name="error">Error message</param>
        /// <param name="dump">Error dump for debugger</param>
        void ErrorLogs(string error, string dump);
    }
}