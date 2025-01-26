using System;
using System.Linq;
using RotMGStats.RealmShark.NET.packets;
using RotMGStats.RealmShark.NET.packets.reader;
using RotMGStats.RealmShark.NET.packets.data;
using RotMGStats.RealmShark.NET.util;

namespace RotMGStats.RealmShark.NET.packets.incoming
{
    /// <summary>
    /// Received to notify the player of a new game tick.
    /// </summary>
    public class NewTickPacket : Packet
    {
        /// <summary>
        /// The id of the tick.
        /// </summary>
        public int TickId { get; set; }

        /// <summary>
        /// The time between the last tick and this tick, in milliseconds.
        /// </summary>
        public int TickTime { get; set; }

        /// <summary>
        /// Server realtime in ms.
        /// </summary>
        public uint ServerRealTimeMS { get; set; }

        /// <summary>
        /// Last server realtime in ms.
        /// </summary>
        public ushort ServerLastTimeRTTMS { get; set; }

        /// <summary>
        /// An array of statuses for objects which are currently visible to the player.
        /// </summary>
        public ObjectStatusData[] Status { get; set; }

        /// <summary>
        /// Deserialize method to deserialize the data for each packet type.
        /// </summary>
        /// <param name="buffer">The data of the packet in a rotmg buffer format.</param>
        public override void Deserialize(BufferReader buffer)
        {
            TickId = buffer.ReadInt();
            TickTime = buffer.ReadInt();
            ServerRealTimeMS = (uint)buffer.ReadUnsignedInt();
            ServerLastTimeRTTMS = (ushort)buffer.ReadUnsignedShort();
            Status = new ObjectStatusData[buffer.ReadShort()];
            for (int i = 0; i < Status.Length; i++)
            {
                Status[i] = new ObjectStatusData();
                Status[i].Deserialize(buffer);
            }
        }

        public override string ToString()
        {
            return $"NewTickPacket{{\n  tickId={TickId}\n  tickTime={TickTime}\n  serverRealTimeMS={ServerRealTimeMS}\n  serverLastTimeRTTMS={ServerLastTimeRTTMS}\n  status={string.Join(", ", Status.Select(s => s.ToString()))}}}";
        }
    }
}