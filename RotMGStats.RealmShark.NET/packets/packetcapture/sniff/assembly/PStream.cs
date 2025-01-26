namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    /// <summary>
    /// TCP packet stream interface used to send ordered TCP packets with bytes contained in their payload.
    /// </summary>
    public interface PStream
    {
        /// <summary>
        /// Ordered TCP packet method to send the byte stream contained in the payload.
        /// </summary>
        /// <param name="data">The stream contained in TCP packet bytes.</param>
        /// <param name="srcAddr">Source IP of the TCP packet.</param>
        void Stream(byte[] data, byte[] srcAddr);
    }
}