namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    /// <summary>
    /// Reset interface used in stream constructor for sending reset updates when
    /// receiving reset packets.
    /// </summary>
    public interface PReset
    {
        /// <summary>
        /// Method called when a reset packet is received.
        /// </summary>
        void Reset();
    }
}