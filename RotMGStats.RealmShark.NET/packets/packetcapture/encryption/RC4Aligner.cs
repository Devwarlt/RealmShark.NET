using System;
using System.Text;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.encryption
{
    /// <summary>
    /// Aligner for RC4 by brute forcing the alignment with known conditions. One method uses
    /// a sequence of two consecutive numbers and searches for the sequence by attempting to
    /// decrypt two tick packets in sequence with known bytes between the two. The other brute
    /// forces text packets with a known name as the sender.
    /// </summary>
    public class RC4Aligner
    {
        public const int SEARCH_SIZE = 10000000;

        /// <summary>
        /// String to byte converter.
        /// </summary>
        /// <param name="s">String needing to be converted into bytes</param>
        /// <returns>Returns the byte array of the string.</returns>
        public static byte[] EncodeString(string s)
        {
            byte[] ret = new byte[2 + s.Length];
            ret[1] = (byte)(s.Length & 0xff);
            ret[0] = (byte)((s.Length >> 8) & 0xff);
            Array.Copy(Encoding.UTF8.GetBytes(s), 0, ret, 2, s.Length);
            return ret;
        }

        /// <summary>
        /// Brute force RC4 cracker to find the alignment using a known key using a Text packet
        /// with a known name.
        /// </summary>
        /// <param name="cipher">A RC4 cipher with a given key and given index.</param>
        /// <param name="textData">Text packet data.</param>
        /// <param name="name">Given name of the player sending the message.</param>
        /// <returns>Returning the index of the RC4 ciphers index from initial condition
        /// constructed by key.</returns>
        public static int SyncCipher(RC4 cipher, byte[] textData, string name)
        {
            RC4 finder = cipher.Fork();
            RC4 forked = cipher.Fork();

            byte[] target = EncodeString(name);
            int nameOffset = 5;
            int offset = -1;
        outer:
            while (offset < SEARCH_SIZE)
            {
                offset++;
                byte xor = finder.GetXor();
                if (target[0] != (textData[nameOffset] ^ xor))
                    continue;
                finder.Copy(forked);
                for (int i = 1; i < target.Length; i++)
                {
                    xor = forked.GetXor();
                    if (target[i] != (textData[i + nameOffset] ^ xor))
                    {
                        continue outer;
                    }
                }
                break;
            }
            if (offset == SEARCH_SIZE)
            {
                return -1;
            }

            cipher.Skip(offset);
            return offset;
        }

        /// <summary>
        /// Brute force RC4 cracker to find the alignment using a known key using two
        /// consecutive Tick packets. Any tick packet sent from server have ticks incremented
        /// by one from the previous tick packet. The cracker finds any index of the cipher
        /// by finding A + 1 == B where B is the numeral of a tick packet directly followed by
        /// a tick packet A with known bytes received between the two Tick packets.
        /// </summary>
        /// <param name="cipher">A RC4 cipher with a given key and given index.</param>
        /// <param name="A">Tick packet A</param>
        /// <param name="B">Tick packet B</param>
        /// <param name="delta">Known packets between packet A and B</param>
        /// <returns>Returning the index of the RC4 ciphers index from initial condition
        /// constructed by key.</returns>
        public static int SyncCipher(RC4 cipher, byte[] A, byte[] B, int delta)
        {
            RC4 finderA = cipher.Fork();
            RC4 finderB = cipher.Fork();
            RC4 tmp = cipher.Fork();
            finderB.Skip(delta);
            int offset = -1;
            while (offset < SEARCH_SIZE)
            {
                offset++;
                finderA.Copy(tmp);
                int a = DecodeInt(A, tmp);
                finderB.Copy(tmp);
                int b = DecodeInt(B, tmp);

                if ((a + 1) != b)
                {
                    finderA.GetXor();
                    finderB.GetXor();
                    continue;
                }
                break;
            }
            if (offset == SEARCH_SIZE)
            {
                return -1;
            }

            cipher.Skip(offset);
            return offset;
        }

        /// <summary>
        /// A fast method to return an integer from the head of an array XORed with a given
        /// cipher with a specific index.
        /// </summary>
        /// <param name="bytes">Byte array where integer is</param>
        /// <param name="cipher">A RC4 cipher object with a specific index.</param>
        /// <returns>Returning the resulting integer</returns>
        private static int DecodeInt(byte[] bytes, RC4 cipher)
        {
            return ((bytes[0] ^ cipher.GetXor()) << 24) | ((bytes[1] ^ cipher.GetXor()) << 16) | ((bytes[2] ^ cipher.GetXor()) << 8) | (bytes[3] ^ cipher.GetXor());
        }
    }
}