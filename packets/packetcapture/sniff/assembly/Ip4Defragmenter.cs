using System;
using System.Collections.Generic;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    /// <summary>
    /// Ip4 de-fragmenter class to re-assemble ip4 packets that have
    /// been fragmented through certain routes over the net.
    ///
    /// Implementation based on article:
    /// https://packetpushers.net/ip-fragmentation-in-detail/
    ///
    /// TODO: test this class properly. WARNING UNTESTED!
    /// </summary>
    public static class Ip4Defragmenter
    {
        private static readonly Dictionary<int, List<Ip4Packet>> fragments = new Dictionary<int, List<Ip4Packet>>();

        /// <summary>
        /// Main method used to de-fragment packets. If the packet doesn't need re-assembly
        /// then it returns the packet without any changes. If the packet however is part
        /// of a fragmented ip4 packet then it stores it and checks if all other fragments
        /// of the same id is present. If all parts the fragmented packet is present then
        /// it re-assembles them into the original ip4 packet and returns it. Otherwise,
        /// returns null.
        /// </summary>
        /// <param name="ip4packet">Ip4 packet that needs to be checked if it needs to be re-assembled.</param>
        /// <returns>De-fragmented packet if it needs to be re-assembled. Otherwise, returns the same ip4 packet.</returns>
        public static Ip4Packet Defragment(Ip4Packet ip4packet)
        {
            if (ip4packet == null || ip4packet.DontFragmentFlag || (!ip4packet.MoreFragmentFlag && ip4packet.FragmentOffset == 0))
            {
                return ip4packet;
            }
            int id = ip4packet.Identification;
            if (!fragments.ContainsKey(id))
            {
                fragments[id] = new List<Ip4Packet>(5);
            }
            fragments[id].Add(ip4packet);
            return Assemble(id, ip4packet);
        }

        /// <summary>
        /// Assembler that puts the packets back into the original Ip4 packet before it was fragmented.
        /// </summary>
        /// <param name="id">The id of the packet</param>
        /// <param name="ip4packet">The Ip4 packet</param>
        /// <returns>Assembled packet if it needs to be re-assemble or returns null if all fragments haven't arrived.</returns>
        private static Ip4Packet Assemble(int id, Ip4Packet ip4packet)
        {
            Ip4Packet head = null, tail = null;
            List<Ip4Packet> frags = fragments[id];
            int math = 0;
            foreach (Ip4Packet ip in frags)
            {
                if (!ip.MoreFragmentFlag)
                {
                    tail = ip;
                    math += ip.FragmentOffset * 8;
                    continue;
                }
                else if (ip.FragmentOffset == 0)
                {
                    head = ip;
                }
                math -= ip.PayloadLength;
            }
            if (math != 0 || head == null || tail == null) return null;

            byte[] data = new byte[head.Ihl * 4 + tail.FragmentOffset * 8 + tail.PayloadLength];

            Array.Copy(head.RawData, 0, data, 0, head.TotalLength);
            foreach (Ip4Packet ip in frags)
            {
                if (ip != head)
                {
                    Array.Copy(ip.Payload, 0, data, ip.FragmentOffset * 8, ip.PayloadLength);
                }
            }
            fragments.Remove(id);
            return new Ip4Packet(data, ip4packet.EthernetPacket);
        }
    }
}