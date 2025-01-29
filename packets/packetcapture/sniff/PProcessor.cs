namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff
{
    /// <summary>
    /// Packet processing class calling the sniffer.
    /// </summary>
    public interface PProcessor
    {
        /// <summary>
        /// Reset called when a TCP reset packet is received on incoming packets.
        /// </summary>
        void ResetIncoming();

        /// <summary>
        /// Reset called when a TCP reset packet is received on outgoing packets.
        /// </summary>
        void ResetOutgoing();

        /// <summary>
        /// Incoming stream from the TCP payload.
        /// </summary>
        /// <param name="data">TCP packet payload byte data containing the stream.</param>
        /// <param name="srcAddr">Source of the incoming packets.</param>
        void IncomingStream(byte[] data, byte[] srcAddr);

        /// <summary>
        /// Outgoing stream from the TCP payload.
        /// </summary>
        /// <param name="data">TCP packet payload byte data containing the stream.</param>
        /// <param name="srcAddr">Source of the outgoing packets.</param>
        void OutgoingStream(byte[] data, byte[] srcAddr);
    }
}