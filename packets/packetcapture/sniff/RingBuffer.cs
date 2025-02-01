namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff
{
    /// <summary>
    /// This is a simple Ring buffer made to store buffered packets as captured
    /// by the sniffer. It stores elements in an array with two indices traversing
    /// the array. The indexes wrap around to zero when hitting the limit of the
    /// array. The first index is the write index and the second the read index.
    /// Write adds elements and increments the counter while the read does the same
    /// when reading the elements. The buffer also doubles in size if the ring buffer
    /// is full and copies the elements into the newly created buffer of double the
    /// size in the corresponding locations in the new array.
    /// </summary>
    /// <typeparam name="T">Generic type, in Sniffer.java its used to store TcpPackets</typeparam>
    public class RingBuffer<T>
    {
        private const byte EMPTY = 0, NORMAL = 1, FULL = 2;
        private T[] buffer;
        private int readPointer = 0, writePointer = 0;
        private byte state = EMPTY;

        /// <summary>
        /// Constructor with initial capacity
        /// </summary>
        /// <param name="capacity">Initial capacity of buffer size.</param>
        public RingBuffer(int capacity)
        {
            buffer = new T[capacity];
        }

        /// <summary>
        /// Is empty check
        /// </summary>
        /// <returns>True if buffer is empty.</returns>
        public bool IsEmpty()
        {
            lock (this)
            {
                return EMPTY == state;
            }
        }

        /// <summary>
        /// Returns the number of elements currently in the buffer.
        /// </summary>
        /// <returns>The number of buffered elements in the buffer.</returns>
        public int Size()
        {
            lock (this)
            {
                if (readPointer > writePointer)
                {
                    return writePointer + (buffer.Length - readPointer);
                }
                else
                {
                    return writePointer - readPointer;
                }
            }
        }

        /// <summary>
        /// Puts objects into the ring buffer.
        /// If the ring buffer fills up to the max,
        /// doubles the size of the buffer.
        /// </summary>
        /// <param name="item">Items to be inserted into the buffer.</param>
        public void Push(T item)
        {
            lock (this)
            {
                if ((writePointer + 1) % buffer.Length == readPointer)
                {
                    state = FULL;
                }
                else
                {
                    if (state == FULL)
                    {
                        T[] next = new T[buffer.Length << 1];
                        // Copy from zero to writePointer
                        Array.Copy(buffer, 0, next, 0, writePointer);
                        // Copy from writePointer to the end of the old array into new
                        Array.Copy(buffer, writePointer, next, buffer.Length + writePointer, buffer.Length - writePointer);
                        readPointer += buffer.Length;
                        buffer = next;
                    }
                    state = NORMAL;
                }
                writePointer = writePointer % buffer.Length;
                buffer[writePointer++] = item;
            }
        }

        /// <summary>
        /// Removes and returns the oldest entry into the buffer.
        /// The elements are extracted as per FIFO
        /// </summary>
        /// <returns>Returns the oldest element in the buffer and removes it.</returns>
        public T Pop()
        {
            lock (this)
            {
                if (readPointer + 1 == writePointer)
                {
                    state = EMPTY;
                }
                else if (state == EMPTY)
                {
                    return default(T);
                }
                else
                {
                    state = NORMAL;
                }
                T buf = buffer[readPointer];
                readPointer = (readPointer + 1) % buffer.Length;
                return buf;
            }
        }
    }
}