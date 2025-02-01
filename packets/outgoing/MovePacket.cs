using RotMGStats.RealmShark.NET.packets.reader;
using RotMGStats.RealmShark.NET.packets.data;

namespace RotMGStats.RealmShark.NET.packets.outgoing
{
    /// <summary>
    /// Sent to acknowledge a `NewTickPacket`, and to notify the
    /// server of the client's current position and time.
    /// </summary>
    public class MovePacket : Packet
    {
        /// <summary>
        /// The tick id of the `NewTickPacket` which this is acknowledging.
        /// </summary>
        public int TickId { get; set; }

        /// <summary>
        /// The serverRealTimeMS.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// The move records of the client.
        /// This property can be an empty array.
        /// </summary>
        public MoveRecord[] Records { get; set; }

        /// <summary>
        /// Deserialize method to deserialize the data for each packet type.
        /// </summary>
        /// <param name="buffer">The data of the packet in a rotmg buffer format.</param>
        public override void Deserialize(BufferReader buffer)
        {
            TickId = buffer.ReadInt();
            Time = buffer.ReadInt();
            Records = new MoveRecord[buffer.ReadShort()];
            for (int i = 0; i < Records.Length; i++)
            {
                Records[i] = new MoveRecord();
                Records[i].Deserialize(buffer);
            }
        }

        public override string ToString()
        {
            return $"MovePacket{{\n   tickId={TickId}\n   time={Time}\n   records={string.Join(", ", Records.Select(r => r.ToString()))}}}";
        }
    }
}