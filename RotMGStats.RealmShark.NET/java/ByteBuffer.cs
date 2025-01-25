using System;

namespace RotMGStats.RealmShark.NET.java
{
    /// <summary>
    /// A class that provides functionality similar to Java's ByteBuffer.
    /// </summary>
    public class ByteBuffer
    {
        private byte[] buffer;
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteBuffer"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public ByteBuffer(int capacity)
        {
            buffer = new byte[capacity];
            position = 0;
        }

        /// <summary>
        /// Puts a byte into the buffer at the current position.
        /// </summary>
        /// <param name="value">The byte value to put into the buffer.</param>
        public void Put(byte value)
        {
            EnsureCapacity(1);
            buffer[position++] = value;
        }

        /// <summary>
        /// Puts an array of bytes into the buffer starting at the current position.
        /// </summary>
        /// <param name="src">The byte array to put into the buffer.</param>
        public void Put(byte[] src)
        {
            EnsureCapacity(src.Length);
            Array.Copy(src, 0, buffer, position, src.Length);
            position += src.Length;
        }

        /// <summary>
        /// Puts an integer into the buffer at the current position.
        /// </summary>
        /// <param name="value">The integer value to put into the buffer.</param>
        public void PutInt(int value)
        {
            EnsureCapacity(4);
            buffer[position++] = (byte)(value >> 24);
            buffer[position++] = (byte)(value >> 16);
            buffer[position++] = (byte)(value >> 8);
            buffer[position++] = (byte)value;
        }

        /// <summary>
        /// Gets a byte from the buffer at the current position.
        /// </summary>
        /// <returns>The byte value at the current position.</returns>
        public byte Get()
        {
            return buffer[position++];
        }

        /// <summary>
        /// Gets an array of bytes from the buffer starting at the current position.
        /// </summary>
        /// <param name="length">The number of bytes to get from the buffer.</param>
        /// <returns>A byte array containing the bytes from the buffer.</returns>
        public byte[] Get(int length)
        {
            byte[] dst = new byte[length];
            Array.Copy(buffer, position, dst, 0, length);
            position += length;
            return dst;
        }

        /// <summary>
        /// Gets an integer from the buffer at the current position.
        /// </summary>
        /// <returns>The integer value at the current position.</returns>
        public int GetInt()
        {
            int value = (buffer[position++] << 24) |
                        (buffer[position++] << 16) |
                        (buffer[position++] << 8) |
                        buffer[position++];
            return value;
        }

        /// <summary>
        /// Resets the position to zero.
        /// </summary>
        public void Flip()
        {
            position = 0;
        }

        /// <summary>
        /// Returns a byte array containing the data from the buffer up to the current position.
        /// </summary>
        /// <returns>A byte array containing the data from the buffer.</returns>
        public byte[] ToArray()
        {
            byte[] result = new byte[position];
            Array.Copy(buffer, 0, result, 0, position);
            return result;
        }

        /// <summary>
        /// Ensures that the buffer has enough capacity to accommodate additional data.
        /// </summary>
        /// <param name="additionalCapacity">The additional capacity needed.</param>
        private void EnsureCapacity(int additionalCapacity)
        {
            if (position + additionalCapacity > buffer.Length)
            {
                Array.Resize(ref buffer, buffer.Length * 2);
            }
        }
    }
}