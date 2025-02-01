using RotMGStats.RealmShark.NET.util;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.pconstructor
{
    /// <summary>
    /// Rotmg packet constructor appending bytes into a packet based on the size at the header of each sequence.
    /// </summary>
    public class ROTMGPacketConstructor
    {
        private readonly PacketConstructor packetConstructor;
        private readonly byte[] bytes = new byte[200000];
        private int index;
        private int pSize = 0;

        /// <summary>
        /// ROMGPacketConstructor needing the PacketConstructor class to send correctly stitched packets.
        /// </summary>
        /// <param name="pc">PacketConstructor class needed to send correctly stitched packets.</param>
        public ROTMGPacketConstructor(PacketConstructor pc)
        {
            packetConstructor = pc;
        }

        /// <summary>
        /// Build method used to stitch individual bytes in the data in the TCP packets according to
        /// specified size at the header of the data.
        /// Only start listen after the next packet less than MTU(maximum transmission unit packet) is received.
        /// </summary>
        /// <param name="data">TCP packet with the data inside.</param>
        public void Build(byte[] data)
        {
            foreach (byte b in data)
            {
                bytes[index++] = b;
                if (index >= 4)
                {
                    if (pSize == 0)
                    {
                        pSize = Util.DecodeInt(bytes);
                        if (pSize > 200000)
                        {
                            Util.PrintLogs("Oversize packet construction.");
                            pSize = 0;
                            return;
                        }
                    }

                    if (index == pSize)
                    {
                        index = 0;
                        byte[] realmPacket = bytes.Take(pSize).ToArray();
                        pSize = 0;
                        var packetData = new ArraySegment<byte>(realmPacket);
                        packetConstructor.PacketReceived(packetData);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the byte index and the packet size.
        /// </summary>
        public void Reset()
        {
            index = 0;
            pSize = 0;
        }
    }
}