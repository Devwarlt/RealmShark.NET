using System;
using RotMGStats.RealmShark.NET.packets.reader;

namespace RotMGStats.RealmShark.NET.packets
{
    /// <summary>
    /// Abstract packet class for all incoming or outgoing packets.
    /// </summary>
    [Serializable]
    public abstract class Packet
    {
        private byte[] data;

        /// <summary>
        /// Gets the payload data.
        /// </summary>
        /// <returns>The payload data as a byte array.</returns>
        public byte[] GetPayload()
        {
            return data;
        }

        /// <summary>
        /// Sets the payload data.
        /// </summary>
        /// <param name="data">The payload data as a byte array.</param>
        public void SetData(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// An interface to be used as a class factory for different packet types.
        /// </summary>
        public interface IPacket
        {
            Packet Factory();
        }

        /// <summary>
        /// Deserialize method to deserialize the data for each packet type.
        /// </summary>
        /// <param name="buffer">The data of the packet in a rotmg buffer format.</param>
        public abstract void Deserialize(BufferReader buffer);
    }
}