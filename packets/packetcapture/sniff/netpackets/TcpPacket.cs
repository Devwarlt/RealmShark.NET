namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets
{
    /// <summary>
    /// Packet building inspired by work done by Pcap4j (https://github.com/kaitoy/pcap4j)
    /// and Network programming in Linux (http://tcpip.marcolavoie.ca/ip.html)
    /// <para>
    /// Tcp packet constructor.
    /// </para>
    /// </summary>
    public class TcpPacket
    {
        private const int SRC_PORT_OFFSET = 0;
        private const int SRC_PORT_SIZE = 2;
        private const int DST_PORT_OFFSET = 2;
        private const int DST_PORT_SIZE = 2;
        private const int SEQUENCE_NUMBER_OFFSET = 4;
        private const int SEQUENCE_NUMBER_SIZE = 4;
        private const int ACKNOWLEDGMENT_NUMBER_OFFSET = 8;
        private const int ACKNOWLEDGMENT_NUMBER_SIZE = 4;
        private const int DATA_OFFSET_BITS_OFFSET = 12;
        private const int CONTROL_BITS_OFFSET = 13;
        private const int WINDOW_OFFSET = 14;
        private const int WINDOW_SIZE = 2;
        private const int CHECKSUM_OFFSET = 16;
        private const int CHECKSUM_SIZE = 2;
        private const int URGENT_POINTER_OFFSET = 18;
        private const int URGENT_POINTER_SIZE = 2;
        private const int OPTIONS_OFFSET_TCP = 20;
        private const int MIN_TCP_HEADER_SIZE = 20;

        private readonly Ip4Packet ip4Packet;
        private readonly byte[] rawData;
        private readonly int srcPort;
        private readonly int dstPort;
        private readonly long sequenceNumber;
        private readonly long acknowledgmentNumber;
        private readonly int dataOffset;
        private readonly int reserved;
        private readonly bool urg;
        private readonly bool ack;
        private readonly bool psh;
        private readonly bool rst;
        private readonly bool syn;
        private readonly bool fin;
        private readonly int window;
        private readonly int checksum;
        private readonly int urgentPointer;
        private readonly byte[] optionsTCP;
        private readonly byte[] payload;
        private readonly int payloadSize;

        public TcpPacket(byte[] data, int length, Ip4Packet packet)
        {
            ip4Packet = packet;
            rawData = data;
            srcPort = UtilNetPackets.GetShort(data, SRC_PORT_OFFSET);
            dstPort = UtilNetPackets.GetShort(data, DST_PORT_OFFSET);
            sequenceNumber = UtilNetPackets.GetIntAsLong(data, SEQUENCE_NUMBER_OFFSET);
            acknowledgmentNumber = UtilNetPackets.GetIntAsLong(data, ACKNOWLEDGMENT_NUMBER_OFFSET);

            int sizeControlBits = UtilNetPackets.GetByte(data, DATA_OFFSET_BITS_OFFSET);
            dataOffset = (sizeControlBits & 0xF0) >> 4;
            reserved = sizeControlBits & 0xF;

            int reservedAndControlBits = UtilNetPackets.GetByte(data, CONTROL_BITS_OFFSET);
            urg = (reservedAndControlBits & 0x20) != 0;
            ack = (reservedAndControlBits & 0x10) != 0;
            psh = (reservedAndControlBits & 0x08) != 0;
            rst = (reservedAndControlBits & 0x04) != 0;
            syn = (reservedAndControlBits & 0x02) != 0;
            fin = (reservedAndControlBits & 0x01) != 0;

            window = UtilNetPackets.GetShort(data, WINDOW_OFFSET);
            checksum = UtilNetPackets.GetShort(data, CHECKSUM_OFFSET);
            urgentPointer = UtilNetPackets.GetShort(data, URGENT_POINTER_OFFSET);

            int headerLengthTCP = (dataOffset & 0xFF) * 4;
            if (headerLengthTCP < 20) headerLengthTCP = 20;

            if (headerLengthTCP != OPTIONS_OFFSET_TCP)
            {
                optionsTCP = UtilNetPackets.GetBytes(data, OPTIONS_OFFSET_TCP, headerLengthTCP - MIN_TCP_HEADER_SIZE);
            }
            else
            {
                optionsTCP = Array.Empty<byte>();
            }

            payloadSize = length - headerLengthTCP;
            if (payloadSize != 0)
            {
                payload = UtilNetPackets.GetBytes(data, headerLengthTCP, payloadSize);
            }
            else
            {
                payload = Array.Empty<byte>();
            }
        }

        public byte[] RawData => rawData;

        public int SrcPort => srcPort;

        public int DstPort => dstPort;

        public long SequenceNumber => sequenceNumber;

        public long AcknowledgmentNumber => acknowledgmentNumber;

        public int DataOffset => dataOffset;

        public int Reserved => reserved;

        public bool Urg => urg;

        public bool Ack => ack;

        public bool Psh => psh;

        public bool Rst => rst;

        public bool Syn => syn;

        public bool Fin => fin;

        public bool ResetBit => rst || syn || fin;

        public int Window => window;

        public int Checksum => checksum;

        public int UrgentPointer => urgentPointer;

        public byte[] OptionsTCP => optionsTCP;

        public byte[] Payload => payload;

        public int PayloadSize => payloadSize;

        public Ip4Packet Ip4Packet => ip4Packet;

        public override string ToString()
        {
            return $"TcpPacket{{\n srcPort={srcPort}\n dstPort={dstPort}\n sequenceNumber={sequenceNumber}\n acknowledgmentNumber={acknowledgmentNumber}\n dataOffset={dataOffset}\n reserved={reserved}\n urg={urg}\n ack={ack}\n psh={psh}\n rst={rst}\n syn={syn}\n fin={fin}\n window={window}\n checksum={checksum}\n urgentPointer={urgentPointer}\n optionsTCP={string.Join(", ", optionsTCP.Select(b => b.ToString("X2")))}\n TCPdata={string.Join(", ", UtilNetPackets.GetBytes(rawData, 0, (dataOffset & 0xFF) * 4).Select(b => b.ToString("X2")))}\n payloadTCP={string.Join(", ", payload.Select(b => b.ToString("X2")))}\n payloadSize={payloadSize}}}";
        }
    }
}