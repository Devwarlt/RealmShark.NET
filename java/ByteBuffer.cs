namespace RotMGStats.RealmShark.NET.java
{
    public class ByteBuffer
    {
        private byte[] buffer;
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteBuffer"/> class with the specified byte array.
        /// </summary>
        /// <param name="buffer">The byte array to wrap.</param>
        public ByteBuffer(byte[] buffer)
        {
            this.buffer = buffer;
            this.position = 0;
        }

        /// <summary>
        /// Gets or sets the current position in the buffer.
        /// </summary>
        public int Position
        {
            get => position;
            set
            {
                if (value < 0 || value > buffer.Length)
                    throw new ArgumentOutOfRangeException(nameof(value), "Position out of range");
                position = value;
            }
        }

        /// <summary>
        /// Gets the current capacity of the buffer.
        /// </summary>
        public int Capacity => buffer.Length;

        /// <summary>
        /// Converts the buffer to a byte array.
        /// </summary>
        /// <returns>The byte array representation of the buffer.</returns>
        public byte[] ToArray()
        {
            byte[] array = new byte[buffer.Length];
            Array.Copy(buffer, array, buffer.Length);
            return array;
        }

        /// <summary>
        /// Ensures the buffer has the specified capacity.
        /// </summary>
        /// <param name="minCapacity">The minimum capacity required.</param>
        public void EnsureCapacity(int minCapacity)
        {
            if (minCapacity > buffer.Length)
            {
                int newCapacity = Math.Max(buffer.Length * 2, minCapacity);
                byte[] newBuffer = new byte[newCapacity];
                Array.Copy(buffer, newBuffer, buffer.Length);
                buffer = newBuffer;
            }
        }

        /// <summary>
        /// Gets the byte value at the current position and advances the position by 1 byte.
        /// </summary>
        /// <returns>The byte value at the current position.</returns>
        public byte Get()
        {
            if (position >= buffer.Length)
                throw new IndexOutOfRangeException("Buffer overflow");

            return buffer[position++];
        }

        /// <summary>
        /// Gets the specified number of bytes from the current position and advances the position.
        /// </summary>
        /// <param name="length">The number of bytes to get.</param>
        /// <returns>The byte array from the current position.</returns>
        public byte[] Get(int length)
        {
            if (position + length > buffer.Length)
                throw new IndexOutOfRangeException("Buffer overflow");

            byte[] result = new byte[length];
            Array.Copy(buffer, position, result, 0, length);
            position += length;
            return result;
        }

        /// <summary>
        /// Gets the integer value at the current position and advances the position by 4 bytes.
        /// </summary>
        /// <returns>The integer value at the current position.</returns>
        public int GetInt()
        {
            if (position + 4 > buffer.Length)
                throw new IndexOutOfRangeException("Buffer overflow");

            int value = BitConverter.ToInt32(buffer, position);
            position += 4;
            return value;
        }

        /// <summary>
        /// Gets the short value at the current position and advances the position by 2 bytes.
        /// </summary>
        /// <returns>The short value at the current position.</returns>
        public short GetShort()
        {
            if (position + 2 > buffer.Length)
                throw new IndexOutOfRangeException("Buffer overflow");

            short value = BitConverter.ToInt16(buffer, position);
            position += 2;
            return value;
        }

        /// <summary>
        /// Gets the float value at the current position and advances the position by 4 bytes.
        /// </summary>
        /// <returns>The float value at the current position.</returns>
        public float GetFloat()
        {
            if (position + 4 > buffer.Length)
                throw new IndexOutOfRangeException("Buffer overflow");

            float value = BitConverter.ToSingle(buffer, position);
            position += 4;
            return value;
        }

        /// <summary>
        /// Gets the specified number of bytes from the current position and advances the position.
        /// </summary>
        /// <param name="dst">The destination array to copy bytes into.</param>
        public void Get(byte[] dst)
        {
            if (position + dst.Length > buffer.Length)
                throw new IndexOutOfRangeException("Buffer overflow");

            Array.Copy(buffer, position, dst, 0, dst.Length);
            position += dst.Length;
        }

        /// <summary>
        /// Resets the position to the beginning of the buffer.
        /// </summary>
        public void Reset()
        {
            position = 0;
        }
    }
}