using RotMGStats.RealmShark.NET.packets.reader;

namespace RotMGStats.RealmShark.NET.packets.data
{
    /// <summary>
    /// Coordinate data of world objects.
    /// </summary>
    [Serializable]
    public class WorldPosData
    {
        /// <summary>
        /// Position x
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Position y
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Deserializer method to extract data from the buffer.
        /// </summary>
        /// <param name="buffer">Data that needs deserializing.</param>
        /// <returns>Returns this object after deserializing.</returns>
        public WorldPosData Deserialize(BufferReader buffer)
        {
            X = buffer.ReadFloat();
            Y = buffer.ReadFloat();
            return this;
        }

        public override string ToString()
        {
            return $"WorldPosData (x:{X}, y:{Y})";
        }
    }
}