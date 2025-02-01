using RotMGStats.RealmShark.NET.packets.reader;
using RotMGStats.RealmShark.NET.util;

namespace RotMGStats.RealmShark.NET.packets.data
{
    /// <summary>
    /// Represents the status data of an object.
    /// </summary>
    [Serializable]
    public class ObjectStatusData
    {
        /// <summary>
        /// The object id of the object which this status is for.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// The position of the object which this status is for.
        /// </summary>
        public WorldPosData Pos { get; set; }

        /// <summary>
        /// A list of stats for the object which this status is for.
        /// </summary>
        public StatData[] Stats { get; set; }

        /// <summary>
        /// Deserializer method to extract data from the buffer.
        /// </summary>
        /// <param name="buffer">Data that needs deserializing.</param>
        /// <returns>Returns this object after deserializing.</returns>
        public ObjectStatusData Deserialize(BufferReader buffer)
        {
            ObjectId = buffer.ReadCompressedInt();
            Pos = new WorldPosData().Deserialize(buffer);

            Stats = new StatData[buffer.ReadCompressedInt()];
            for (int i = 0; i < Stats.Length; i++)
            {
                Stats[i] = new StatData().Deserialize(buffer);
            }

            return this;
        }

        public override string ToString()
        {
            return $"    Id={ObjectId} Loc=({Pos.X}, {Pos.Y})" +
                   (Stats.Length == 0 ? "" : Util.ShowAll(Stats));
        }
    }
}