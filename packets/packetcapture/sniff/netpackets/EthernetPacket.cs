namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets
{
    /// <summary>
    /// Packet building inspired by work done by Pcap4j (https://github.com/kaitoy/pcap4j)
    /// and Network programming in Linux (http://tcpip.marcolavoie.ca/ip.html)
    /// <para>
    /// Ethernet packet constructor.
    /// </para>
    /// </summary>
    public class EthernetPacket
    {
        public const int IEEE802_3_MAX_LENGTH = 1500;
        private const int MIN_ETHERNET_PAYLOAD_LENGTH = 46; // [bytes]
        private const int MAX_ETHERNET_PAYLOAD_LENGTH = 1500; // [bytes]

        private const int ETHERNET_HEADER_SIZE = 14;
        private const int ETHERNET_HEADER_SIZE_WITH_Q802_1HEADER = 18;
        public const int MAC_SIZE_IN_BYTES = 6;
        private const int DST_ADDR_OFFSET_ETHER = 0;
        private const int SRC_ADDR_OFFSET_ETHER = 6;
        private const int TYPE_OFFSET = 12;
        private const int Q802_1HEADER_SIZE = 4;
        private const int Q802_1HEADER_TYPE_OFFSET = 16;

        private readonly RawPacket rawPacket;
        private readonly byte[] macDest;
        private readonly byte[] macSrc;
        private readonly int etherType;
        private readonly int etherRawPayloadOffset;
        private readonly int payloadSize;
        private readonly byte[] q802_1Header;
        private readonly byte[] payload;

        public EthernetPacket(byte[] data, RawPacket packet)
        {
            rawPacket = packet;
            macDest = UtilNetPackets.GetBytes(data, DST_ADDR_OFFSET_ETHER, MAC_SIZE_IN_BYTES);
            macSrc = UtilNetPackets.GetBytes(data, SRC_ADDR_OFFSET_ETHER, MAC_SIZE_IN_BYTES);
            int type = UtilNetPackets.GetShort(data, TYPE_OFFSET);

            if (type == 0x8100)
            {
                etherType = UtilNetPackets.GetShort(data, Q802_1HEADER_TYPE_OFFSET);
                etherRawPayloadOffset = ETHERNET_HEADER_SIZE_WITH_Q802_1HEADER;
                q802_1Header = UtilNetPackets.GetBytes(data, TYPE_OFFSET, Q802_1HEADER_SIZE);
            }
            else
            {
                etherType = type;
                q802_1Header = Array.Empty<byte>();
                etherRawPayloadOffset = ETHERNET_HEADER_SIZE;
            }

            int currentPayloadSize = data.Length - etherRawPayloadOffset;
            if (etherType <= IEEE802_3_MAX_LENGTH)
            {
                payloadSize = etherType > currentPayloadSize ? currentPayloadSize : etherType;
            }
            else
            {
                payloadSize = currentPayloadSize;
            }
            payload = UtilNetPackets.GetBytes(data, etherRawPayloadOffset, payloadSize);
        }

        public int RawEtherOffset => etherRawPayloadOffset;

        public byte[] MacDest => macDest;

        public byte[] MacSrc => macSrc;

        public int EtherType => etherType;

        public int PayloadSize => payloadSize;

        public byte[] Payload => payload;

        public int EtherRawPayloadOffset => etherRawPayloadOffset;

        public byte[] Q802_1Header => q802_1Header;

        public Ip4Packet GetNewIp4Packet()
        {
            return payload != null && etherType == 0x800 ? new Ip4Packet(payload, this) : null;
        }

        public RawPacket RawPacket => rawPacket;

        public override string ToString()
        {
            return $"EthernetPacket{{\n macDest={MacString(macDest)}\n macSrc={MacString(macSrc)}\n etherType={etherType:X4}\n etherPayloadOffset={etherRawPayloadOffset}\n payloadEther={string.Join(", ", payload.Select(b => b.ToString("X2")))}}}";
        }

        private string MacString(byte[] mac)
        {
            return string.Join(":", mac.Select(b => b.ToString("X2")));
        }
    }
}