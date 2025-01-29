using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using RotMGStats.RealmShark.NET.packets.outgoing;
using RotMGStats.RealmShark.NET.packets.incoming;
using NLog;

namespace RotMGStats.RealmShark.NET.packets
{
    /// <summary>
    /// Packet are matched with the packet index sent as a header of packets and returned.
    /// </summary>
    public struct PacketType
    {
        public static readonly PacketType NEWTICK = new PacketType(10, Direction.Incoming, () => new NewTickPacket());
        public static readonly PacketType MOVE = new PacketType(62, Direction.Outgoing, () => new MovePacket());

        private static readonly Dictionary<int, PacketType> PACKET_TYPE = new Dictionary<int, PacketType>();
        private static readonly Dictionary<int, Func<Packet>> PACKET_TYPE_FACTORY = new Dictionary<int, Func<Packet>>();
        private static readonly Dictionary<Type, PacketType> PACKET_CLASS = new Dictionary<Type, PacketType>();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        static PacketType()
        {
            try
            {
                foreach (var field in typeof(PacketType).GetFields())
                {
                    if (field.FieldType == typeof(PacketType))
                    {
                        var o = (PacketType)field.GetValue(null);
                        PACKET_TYPE[o.index] = o;
                        PACKET_TYPE_FACTORY[o.index] = o.packet;
                        PACKET_CLASS[o.packet().GetType()] = o;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e);
            }
        }

        private readonly int index;
        private readonly Direction dir;
        private readonly Func<Packet> packet;

        private PacketType(int i, Direction d, Func<Packet> p)
        {
            index = i;
            dir = d;
            packet = p;
        }

        /// <summary>
        /// Get the index of the packet
        /// </summary>
        /// <returns>Index of the enum.</returns>
        public int GetIndex()
        {
            return index;
        }

        /// <summary>
        /// Get the enum by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Enum by index.</returns>
        public static PacketType ByOrdinal(int index)
        {
            return PACKET_TYPE.ContainsKey(index) ? PACKET_TYPE[index] : default;
        }

        /// <summary>
        /// Get the enum type by class.
        /// </summary>
        /// <param name="packet">The packet to be returned the type of.</param>
        /// <returns>Enum type.</returns>
        public static PacketType ByClass(Packet packet)
        {
            return PACKET_CLASS.ContainsKey(packet.GetType()) ? PACKET_CLASS[packet.GetType()] : default;
        }

        /// <summary>
        /// Retrieves the packet type from the PACKET_TYPE list.
        /// </summary>
        /// <param name="type">Index of the packet needing to be retrieved.</param>
        /// <returns>Interface IPacket of the class being retrieved.</returns>
        public static Packet GetPacket(int type)
        {
            return PACKET_TYPE_FACTORY.ContainsKey(type) ? PACKET_TYPE_FACTORY[type]() : null;
        }

        /// <summary>
        /// Checks if packet type exists in the PACKET_TYPE list.
        /// </summary>
        /// <param name="type">Index of the packet.</param>
        /// <returns>True if the packet exists in the list of packets in PACKET_TYPE.</returns>
        public static bool ContainsKey(int type)
        {
            return PACKET_TYPE_FACTORY.ContainsKey(type);
        }

        /// <summary>
        /// Returns the class of the enum.
        /// </summary>
        /// <returns>Class of the enum.</returns>
        public Type GetPacketClass()
        {
            return packet().GetType();
        }

        /// <summary>
        /// Gets the list of packets based on direction.
        /// </summary>
        /// <param name="isIncoming">Filter the packets by direction.</param>
        /// <returns>Packet index list by direction.</returns>
        public static int[] GetPacketTypeByDirection(bool isIncoming)
        {
            List<int> list = new List<int>();
            foreach (var field in typeof(PacketType).GetFields())
            {
                if (field.FieldType == typeof(PacketType))
                {
                    var o = (PacketType)field.GetValue(null);
                    if (isIncoming ? o.dir == Direction.Incoming : o.dir == Direction.Outgoing)
                    {
                        list.Add(o.index);
                    }
                }
            }
            return list.ToArray();
        }

        public enum Direction
        {
            Incoming,
            Outgoing
        }
    }
}