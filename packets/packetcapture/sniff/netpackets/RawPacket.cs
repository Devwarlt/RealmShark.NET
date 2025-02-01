namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets
{
    /// <summary>
    /// Raw packet constructor for retrieving packets off the wire.
    /// </summary>
    public class RawPacket
    {
        private readonly DateTime instant;
        private readonly int payloadSize;
        private readonly byte[] payload;

        /// <summary>
        /// Creates a new RawPacket instance.
        /// </summary>
        /// <param name="rawData">The raw data of the packet.</param>
        /// <param name="ts">The timestamp of the packet.</param>
        /// <returns>A new RawPacket instance.</returns>
        public static RawPacket NewPacket(byte[] rawData)
        {
            return new RawPacket(rawData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawPacket"/> class.
        /// </summary>
        /// <param name="data">The data of the packet.</param>
        /// <param name="ins">The timestamp of the packet.</param>
        public RawPacket(byte[] data)
        {
            instant = DateTime.Now;
            payloadSize = data.Length;
            payload = data;
        }

        /// <summary>
        /// Gets the timestamp of the packet.
        /// </summary>
        public DateTime Instant => instant;

        /// <summary>
        /// Gets the size of the payload.
        /// </summary>
        public int PayloadSize => payloadSize;

        /// <summary>
        /// Gets the payload data.
        /// </summary>
        public byte[] Payload => payload;

        /// <summary>
        /// Creates a new EthernetPacket instance from the payload.
        /// </summary>
        /// <returns>A new EthernetPacket instance.</returns>
        public EthernetPacket GetNewEthernetPacket()
        {
            return new EthernetPacket(payload, this);
        }

        public override string ToString()
        {
            return $"RawPacket{{\n instant={instant}\n payloadSize={payloadSize}\n payload={string.Join(", ", payload.Select(b => b.ToString("X2")))}}}";
        }
    }
}