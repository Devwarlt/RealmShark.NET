using System;
using System.Linq;
using System.Text;
using RotMGStats.RealmShark.NET.packets;
using RotMGStats.RealmShark.NET.packets.reader;
using RotMGStats.RealmShark.NET.util;

namespace RotMGStats.RealmShark.NET.packets.incoming.ip
{
    /// <summary>
    /// Emits this class when IP changes happen on incoming packets.
    /// </summary>
    public class IpAddress : Packet
    {
        /// <summary>
        /// IP of incoming packets
        /// </summary>
        public byte[] SrcAddress { get; set; }

        /// <summary>
        /// IP of incoming packets as a single integer
        /// </summary>
        public int SrcAddressAsInt { get; set; }

        /// <summary>
        /// The IP address server name, i.e. USSouth, EUWest2.
        /// </summary>
        public string IpAddressName { get; set; }

        public IpAddress()
        { }

        public IpAddress(byte[] srcAddr)
        {
            SrcAddress = srcAddr;
            SrcAddressAsInt = Util.DecodeInt(srcAddr);
            SetName(SrcAddressAsInt);
        }

        private void SetName(int ip)
        {
            switch (ip)
            {
                case 921430924:
                    IpAddressName = "USWest4";
                    break;

                case 311434905:
                    IpAddressName = "USWest3";
                    break;

                case 911617968:
                    IpAddressName = "USWest";
                    break;

                case 916000068:
                    IpAddressName = "USSouthWest";
                    break;

                case 886033951:
                    IpAddressName = "USSouth3";
                    break;

                case 55737872:
                    IpAddressName = "USSouth";
                    break;

                case 586068087:
                    IpAddressName = "USNorthWest";
                    break;

                case 59571845:
                    IpAddressName = "USMidWest2";
                    break;

                case 59051031:
                    IpAddressName = "USMidWest";
                    break;

                case 878180357:
                    IpAddressName = "USEast2";
                    break;

                case 921362968:
                    IpAddressName = "USEast";
                    break;

                case 873486039:
                    IpAddressName = "EUWest2";
                    break;

                case 267205855:
                    IpAddressName = "EUWest";
                    break;

                case 599016312:
                    IpAddressName = "EUSouthWest";
                    break;

                case 312444280:
                    IpAddressName = "EUNorth";
                    break;

                case 314104494:
                    IpAddressName = "EUEast";
                    break;

                case 233592826:
                    IpAddressName = "Australia";
                    break;

                case 50369407:
                    IpAddressName = "Asia";
                    break;

                default:
                    IpAddressName = "";
                    break;
            }
        }

        public override void Deserialize(BufferReader buffer)
        {
            // Implement deserialization logic here if needed
        }

        public override string ToString()
        {
            return $"IpAddress{{\n   srcAddress={string.Join(".", SrcAddress.Select(b => (b & 0xFF).ToString()))}\n   srcAddressAsInt={SrcAddressAsInt}\n   ipAddressName={IpAddressName}}}";
        }
    }
}