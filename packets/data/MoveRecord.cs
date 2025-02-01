using RotMGStats.RealmShark.NET.packets.reader;

namespace RotMGStats.RealmShark.NET.packets.data
{
    /// <summary>
    /// Movement data of entity moving to point x and y with delta time.
    /// </summary>
    [Serializable]
    public class MoveRecord
    {
        /// <summary>
        /// The client time of this move record.
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// The position where the entity is moving to.
        /// </summary>
        public WorldPosData Pos { get; set; }

        /// <summary>
        /// Deserializer method to extract data from the buffer.
        /// </summary>
        /// <param name="buffer">Data that needs deserializing.</param>
        /// <returns>Returns this object after deserializing.</returns>
        public MoveRecord Deserialize(BufferReader buffer)
        {
            Time = buffer.ReadInt();
            Pos = new WorldPosData().Deserialize(buffer);
            return this;
        }

        public override string ToString()
        {
            return $"MoveRecord{{\n   time={Time}\n   pos={Pos}}}";
        }
    }
}