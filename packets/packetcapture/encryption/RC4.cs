using RotMGStats.RealmShark.NET.util;
using RotMGStats.RealmShark.NET.java;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.encryption
{
    /// <summary>
    /// RC4 cipher used to decrypt packets.
    /// </summary>
    public class RC4
    {
        private int[] state = new int[256], initState = new int[256];
        private int i;
        private int j;

        /// <summary>
        /// Constructor of RC4 needing a string key.
        /// </summary>
        /// <param name="key">A key in the form of a string.</param>
        public RC4(string key) : this(Util.HexStringToByteArray(key)) { }

        /// <summary>
        /// A RC4 object with specific internal states.
        /// </summary>
        /// <param name="state">The current state of the key.</param>
        /// <param name="initState">The initial state of the key.</param>
        /// <param name="i">The i index of the RC4 state.</param>
        /// <param name="j">The j index of the RC4 state.</param>
        private RC4(int[] state, int[] initState, int i, int j)
        {
            this.state = state;
            this.initState = initState;
            this.i = i;
            this.j = j;
        }

        /// <summary>
        /// Constructor of RC4 class needing a hex-number-key in a byte array.
        /// </summary>
        /// <param name="key">Key in the form of hex numbers in a byte array.</param>
        public RC4(byte[] key)
        {
            for (i = 0; i < 256; i++)
            {
                state[i] = i;
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + state[i] + key[i % key.Length]) % 256;
                int tmp = state[i];
                state[i] = state[j];
                state[j] = tmp;
            }
            Array.Copy(state, 0, initState, 0, state.Length);
        }

        /// <summary>
        /// Returning the result of Xor:ing the byte with the RC4 cipher. This also results in
        /// incrementing the internal state by 1.
        /// </summary>
        /// <returns>The resulting byte after Xor:ing with the cipher.</returns>
        public byte GetXor()
        {
            lock (this)
            {
                i = (i + 1) % 256;
                j = (j + state[i]) % 256;
                int tmp = state[i];
                state[i] = state[j];
                state[j] = tmp;
                return (byte)state[(state[i] + state[j]) % 256];
            }
        }

        /// <summary>
        /// A crude brute force skip method to increment the ciphers internal state by an amount.
        /// </summary>
        /// <param name="amount">The amount needed to increment the cipher.</param>
        /// <returns>Returning this object for inlining.</returns>
        public RC4 Skip(int amount)
        {
            for (int k = 0; k < amount; k++)
            {
                i = (i + 1) % 256;
                j = (j + state[i]) % 256;
                int tmp = state[i];
                state[i] = state[j];
                state[j] = tmp;
            }
            return this;
        }

        /// <summary>
        /// Decrypting the bytes in an array with the cipher. Then directly inserting the decrypted
        /// bytes back into the same array with same index.
        /// </summary>
        /// <param name="offset">Offset from the start of the array needing to be decrypted.</param>
        /// <param name="array">Array with bytes needing decrypting.</param>
        public void Decrypt(int offset, byte[] array)
        {
            for (int b = offset; b < array.Length; b++)
            {
                array[b] = (byte)(array[b] ^ GetXor());
            }
        }

        /// <summary>
        /// Decrypting the bytes in an array with the cipher. Then directly inserting the decrypted
        /// bytes back into the same array with same index.
        /// </summary>
        /// <param name="array">Array with bytes needing decrypting.</param>
        public void Decrypt(byte[] array)
        {
            Decrypt(0, array);
        }

        /// <summary>
        /// Decrypting the bytes in a ByteBuffer with the cipher. First extracting the byte array
        /// out of the ByteBuffer object and mutating it (for speed purposes) the array decrypted
        /// bytes back into the same array with same index. This will directly modify the ByteBuffer.
        /// </summary>
        /// <param name="offset">Offset from the start of the array needing to be decrypted.</param>
        /// <param name="byteBuffer">ByteBuffer with bytes needing decrypting.</param>
        public void Decrypt(int offset, ByteBuffer byteBuffer)
        {
            byte[] array = byteBuffer.ToArray();
            Decrypt(offset, array);
        }

        public void Decrypt(ByteBuffer byteBuffer)
        {
            Decrypt(0, byteBuffer);
        }

        public RC4 Fork()
        {
            int[] state2 = new int[state.Length];
            Array.Copy(state, 0, state2, 0, state2.Length);
            return new RC4(state2, initState, i, j);
        }

        public void Copy(RC4 c)
        {
            if (state.Length != c.state.Length) c.state = new int[state.Length];
            Array.Copy(state, 0, c.state, 0, state.Length);
            c.initState = initState;
            c.i = i;
            c.j = j;
        }

        public void Reset()
        {
            Array.Copy(initState, 0, state, 0, state.Length);
            i = 0;
            j = 0;
        }

        public static int ConvertByteArrayToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
