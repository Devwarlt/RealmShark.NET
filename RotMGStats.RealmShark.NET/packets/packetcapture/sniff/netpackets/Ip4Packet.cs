using System;
using System.Linq;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets
{
    /// <summary>
    /// Packet building inspired by work done by Pcap4j (https://github.com/kaitoy/pcap4j)
    /// and Network programming in Linux (http://tcpip.marcolavoie.ca/ip.html)
    /// <para>
    /// Ip4 packet constructor.
    /// </para>
    /// </summary>
    public class Ip4Packet
    {
        private const int VERSION_AND_IHL_OFFSET = 0;
        private const int VERSION_AND_IHL_SIZE = 1;
        private const int TOS_OFFSET = 1;
        private const int TOS_SIZE = 1;
        private const int TOTAL_LENGTH_OFFSET = 2;
        private const int TOTAL_LENGTH_SIZE = 2;
        private const int IDENTIFICATION_OFFSET = 4;
        private const int IDENTIFICATION_SIZE = 2;
        private const int FLAGS_AND_FRAGMENT_OFFSET_OFFSET = 6;
        private const int FLAGS_AND_FRAGMENT_OFFSET_SIZE = 2;
        private const int TTL_OFFSET = 8;
        private const int TTL_SIZE = 1;
        private const int PROTOCOL_OFFSET = 9;
        private const int PROTOCOL_SIZE = 1;
        private const int HEADER_CHECKSUM_OFFSET = 10;
        private const int HEADER_CHECKSUM_SIZE = 2;
        private const int IP_ADDRESS_SIZE = 4;
        private const int SRC_ADDR_OFFSET_IP = 12;
        private const int DST_ADDR_OFFSET_IP = 16;
        private const int OPTIONS_OFFSET_IP = 20;
        private const int MIN_IPV4_HEADER_SIZE = 20;

        private readonly EthernetPacket ethernetPacket;
        private readonly byte[] rawData;
        private readonly int version;
        private readonly int ihl;
        private readonly int precedence;
        private readonly int tos;
        private readonly bool mbz;
        private readonly int totalLength;
        private readonly int identification;
        private readonly bool reservedFlag;
        private readonly bool dontFragmentFlag;
        private readonly bool moreFragmentFlag;
        private readonly int fragmentOffset;
        private readonly int ttl;
        private readonly int protocol;
        private readonly int headerChecksum;
        private readonly byte[] srcAddr;
        private readonly byte[] dstAddr;
        private readonly byte[] optionsIP;
        private readonly int payloadLength;
        private readonly byte[] payload;

        public Ip4Packet(byte[] data, EthernetPacket packet)
        {
            ethernetPacket = packet;
            rawData = data;
            int versionAndIhl = UtilNetPackets.GetByte(data, VERSION_AND_IHL_OFFSET);
            version = (versionAndIhl & 0xF0) >> 4;
            ihl = versionAndIhl & 0x0F;

            byte tosByte = (byte)UtilNetPackets.GetByte(data, TOS_OFFSET);
            precedence = (tosByte & 0xE0) >> 5;
            tos = 0x0F & (tosByte >> 1);
            mbz = (tosByte & 0x01) != 0;
            totalLength = UtilNetPackets.GetShort(data, TOTAL_LENGTH_OFFSET);
            identification = UtilNetPackets.GetShort(data, IDENTIFICATION_OFFSET);

            short flagsAndFragmentOffset = (short)UtilNetPackets.GetShort(data, FLAGS_AND_FRAGMENT_OFFSET_OFFSET);
            reservedFlag = (flagsAndFragmentOffset & 0x8000) != 0;
            dontFragmentFlag = (flagsAndFragmentOffset & 0x4000) != 0;
            moreFragmentFlag = (flagsAndFragmentOffset & 0x2000) != 0;
            fragmentOffset = flagsAndFragmentOffset & 0x1FFF;

            ttl = UtilNetPackets.GetByte(data, TTL_OFFSET);
            protocol = UtilNetPackets.GetByte(data, PROTOCOL_OFFSET);
            headerChecksum = UtilNetPackets.GetShort(data, HEADER_CHECKSUM_OFFSET);
            srcAddr = UtilNetPackets.GetBytes(data, SRC_ADDR_OFFSET_IP, IP_ADDRESS_SIZE);
            dstAddr = UtilNetPackets.GetBytes(data, DST_ADDR_OFFSET_IP, IP_ADDRESS_SIZE);

            int headerLengthIP = ihl * 4;
            if (headerLengthIP < 20) headerLengthIP = 20;

            if (headerLengthIP != OPTIONS_OFFSET_IP)
            {
                optionsIP = UtilNetPackets.GetBytes(data, OPTIONS_OFFSET_IP, headerLengthIP - MIN_IPV4_HEADER_SIZE);
            }
            else
            {
                optionsIP = Array.Empty<byte>();
            }

            int dataLength = data.Length - headerLengthIP;
            int length = totalLength - headerLengthIP;
            if (length > dataLength || length < 0) length = dataLength;
            payloadLength = length;

            if (payloadLength != 0)
            {
                payload = UtilNetPackets.GetBytes(data, headerLengthIP, payloadLength);
            }
            else
            {
                payload = Array.Empty<byte>();
            }
        }

        public int Version => version;

        public int Ihl => ihl;

        public int Precedence => precedence;

        public int Tos => tos;

        public bool Mbz => mbz;

        public int TotalLength => totalLength;

        public int Identification => identification;

        public bool ReservedFlag => reservedFlag;

        public bool DontFragmentFlag => dontFragmentFlag;

        public bool MoreFragmentFlag => moreFragmentFlag;

        public int FragmentOffset => fragmentOffset;

        public int Ttl => ttl;

        public int Protocol => protocol;

        public int HeaderChecksum => headerChecksum;

        public byte[] SrcAddr => srcAddr;

        public byte[] DstAddr => dstAddr;

        public byte[] OptionsIP => optionsIP;

        public int PayloadLength => payloadLength;

        public byte[] Payload => payload;

        public TcpPacket GetNewTcpPacket()
        {
            return payload != null && protocol == 6 ? new TcpPacket(payload, payloadLength, this) : null;
        }

        public EthernetPacket EthernetPacket => ethernetPacket;

        public override string ToString()
        {
            return $"Ip4Packet{{\n version={version}\n ihl={ihl}\n precedence={precedence}\n tos={tos}\n mbz={mbz}\n totalLength={totalLength}\n identification={identification}\n reservedFlag={reservedFlag}\n dontFragmentFlag={dontFragmentFlag}\n moreFragmentFlag={moreFragmentFlag}\n fragmentOffset={fragmentOffset}\n ttl={ttl}\n protocol={protocol}\n headerChecksum={headerChecksum}\n srcAddr={IpToString(srcAddr)}\n dstAddr={IpToString(dstAddr)}\n optionsIP={string.Join(", ", optionsIP.Select(b => b.ToString("X2")))}\n dataLength={payloadLength}\n IPdata={string.Join(", ", UtilNetPackets.GetBytes(rawData, 0, ihl * 4).Select(b => b.ToString("X2")))}\n payloadIP={string.Join(", ", payload.Select(b => b.ToString("X2")))}}}";
        }

        public byte[] RawData => rawData;

        private string IpToString(byte[] ip)
        {
            return string.Join(".", ip.Select(b => b.ToString()));
        }
    }
}