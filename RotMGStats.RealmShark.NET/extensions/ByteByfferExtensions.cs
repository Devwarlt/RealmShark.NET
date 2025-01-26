using RotMGStats.RealmShark.NET.java;

namespace RotMGStats.RealmShark.NET.extensions
{
    public static class ByteBufferExtensions
    {
        /// <summary>
        /// Gets a short (16-bit integer) from the buffer at the current position.
        /// </summary>
        /// <param name="buffer">The ByteBuffer instance.</param>
        /// <returns>The short value at the current position.</returns>
        public static short GetShort(this ByteBuffer buffer)
        {
            byte high = buffer.Get();
            byte low = buffer.Get();
            return (short)((high << 8) | (low & 0xFF));
        }

        /// <summary>
        /// Gets a float (32-bit floating point number) from the buffer at the current position.
        /// </summary>
        /// <param name="buffer">The ByteBuffer instance.</param>
        /// <returns>The float value at the current position.</returns>
        public static float GetFloat(this ByteBuffer buffer)
        {
            byte[] bytes = buffer.Get(4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Gets an array of bytes from the buffer starting at the current position.
        /// </summary>
        /// <param name="buffer">The ByteBuffer instance.</param>
        /// <param name="dst">The destination byte array to store the bytes.</param>
        public static void Get(this ByteBuffer buffer, byte[] dst)
        {
            byte[] bytes = buffer.Get(dst.Length);
            Array.Copy(bytes, dst, dst.Length);
        }
    }
}