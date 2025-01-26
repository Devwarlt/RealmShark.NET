namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    /// <summary>
    /// Class handling large errors stopping the sniffer.
    /// </summary>
    public interface PErrorStop
    {
        /// <summary>
        /// Called when a large error is detected attempting to stop sniffing packets.
        /// </summary>
        void ErrorStop();
    }
}