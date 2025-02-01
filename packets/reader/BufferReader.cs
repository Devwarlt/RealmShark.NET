using System.Text;
using RotMGStats.RealmShark.NET.util;
using RotMGStats.RealmShark.NET.java;
using RotMGStats.RealmShark.NET.extensions;

namespace RotMGStats.RealmShark.NET.packets.reader
{
    /// <summary>
    /// Custom buffer class to deserialize the rotmg packets.
    /// </summary>
    public class BufferReader
    {
        protected ByteBuffer buffer;

        public BufferReader(ByteBuffer data)
        {
            buffer = data;
        }

        /// <summary>
        /// Returns the buffer size.
        /// </summary>
        /// <returns>Size of the buffer.</returns>
        public int Size()
        {
            return buffer.Capacity;
        }

        /// <summary>
        /// Internal index of the buffer.
        /// </summary>
        /// <returns>Returns the internal index the buffer is at.</returns>
        public int GetIndex()
        {
            return buffer.Position;
        }

        /// <summary>
        /// Gets the remaining bytes from current index.
        /// </summary>
        /// <returns>Number of bytes remaining from current index.</returns>
        public int GetRemainingBytes()
        {
            return buffer.Capacity - buffer.Position;
        }

        /// <summary>
        /// Deserialize a boolean.
        /// </summary>
        /// <returns>Returns a boolean that have been deserialized.</returns>
        public bool ReadBoolean()
        {
            return buffer.Get() != 0;
        }

        /// <summary>
        /// Deserialize a byte.
        /// </summary>
        /// <returns>Returns the byte that have been deserialized.</returns>
        public byte ReadByte()
        {
            return buffer.Get();
        }

        /// <summary>
        /// Deserialize an unsigned byte.
        /// </summary>
        /// <returns>Returns an integer containing an unsigned byte that have
        /// been deserialized.</returns>0
        public int ReadUnsignedByte()
        {
            return ByteExtensions.ToUnsignedInt(buffer.Get());
        }

        /// <summary>
        /// Deserialize a short.
        /// </summary>
        /// <returns>Returns the short that have been deserialized.</returns>
        public short ReadShort()
        {
            return buffer.GetShort();
        }

        /// <summary>
        /// Deserialize an unsigned short.
        /// </summary>
        /// <returns>Returns an integer containing an unsigned short that have
        /// been deserialized.</returns>
        public int ReadUnsignedShort()
        {
            return ShortExtensions.ToUnsignedInt(buffer.GetShort());
        }

        /// <summary>
        /// Deserialize an integer.
        /// </summary>
        /// <returns>Returns the integer that have been deserialized.</returns>
        public int ReadInt()
        {
            return buffer.GetInt();
        }

        /// <summary>
        /// Deserialize an unsigned integer.
        /// </summary>
        /// <returns>Returns an integer containing an unsigned integer that have
        /// been deserialized.</returns>
        public long ReadUnsignedInt()
        {
            return (long)IntExtensions.ToUnsignedLong(buffer.GetInt());
        }

        /// <summary>
        /// Deserialize a float.
        /// </summary>
        /// <returns>Returns the float that have been deserialized.</returns>
        public float ReadFloat()
        {
            return buffer.GetFloat();
        }

        /// <summary>
        /// Deserialize a string.
        /// </summary>
        /// <returns>Returns the string that have been deserialized.</returns>
        public string ReadString()
        {
            short len = ReadShort();
            byte[] str = new byte[len];
            buffer.Get(str);
            return Encoding.UTF8.GetString(str);
        }

        /// <summary>
        /// Deserialize a string using UTF32 (more characters that is
        /// never found in-game) not used as far as I'm aware.
        /// </summary>
        /// <returns>Returns the string that have been deserialized.</returns>
        public string ReadStringUTF32()
        {
            int len = ReadInt();
            byte[] str = new byte[len];
            buffer.Get(str);
            return Encoding.UTF32.GetString(str);
        }

        /// <summary>
        /// Deserialize a byte array
        /// </summary>
        public byte[] ReadByteArray()
        {
            byte[] @out = new byte[ReadShort()];
            buffer.Get(@out);
            return @out;
        }

        /// <summary>
        /// Deserialize a byte array.
        /// </summary>
        /// <param name="bytes">Number of bytes that is contained in the array.</param>
        /// <returns>Returns a byte array that have been deserialized.</returns>
        public byte[] ReadBytes(int bytes)
        {
            byte[] @out = new byte[bytes];
            buffer.Get(@out);
            return @out;
        }

        /// <summary>
        /// Rotmg deserializer of a compressed int.
        /// </summary>
        /// <returns>Returns the int that have been deserialized.</returns>
        public int ReadCompressedInt()
        {
            int uByte = ReadUnsignedByte();
            bool isNegative = (uByte & 64) != 0;
            int shift = 6;
            int value = uByte & 63;

            while ((uByte & 128) != 0)
            {
                uByte = ReadUnsignedByte();
                value |= (uByte & 127) << shift;
                shift += 7;
            }

            if (isNegative)
            {
                value = -value;
            }
            return value;
        }

        /// <summary>
        /// Debug print command to print the buffer byte data.
        /// </summary>
        /// <returns>String representation of buffer data in a byte array format.</returns>
        public string PrintBufferArray()
        {
            return string.Join(", ", buffer.ToArray());
        }

        /// <summary>
        /// Returns the remaining bytes in the buffer from the current index.
        /// </summary>
        /// <returns>Returns the remaining bytes.</returns>
        public byte[] GiveRemainingArray()
        {
            return buffer.ToArray().Skip(GetIndex()).Take(Size()).ToArray();
        }

        /// <summary>
        /// Error check if buffer is not finished.
        /// </summary>
        /// <param name="packetType"></param>
        /// <param name="type"></param>

        // -------------- packet error checking -------------

        /// <summary>
        /// Checks if the buffer have finished reading all bytes.
        /// </summary>
        public bool IsBufferFullyParsed()
        {
            return buffer.Capacity == buffer.Position;
        }

        /// <summary>
        /// Prints an error log and the data in the buffer.
        /// </summary>
        /// <param name="packet">Packet type.</param>
        public void PrintError(Packet packet)
        {
            Util.PrintLogs(PacketType.ByClass(packet) + " : " + buffer.Position + "/" + buffer.Capacity);
            Util.PrintLogs(string.Join(", ", buffer.ToArray()));
        }

        public override string ToString()
        {
            return string.Join(", ", buffer.ToArray());
        }
    }
}