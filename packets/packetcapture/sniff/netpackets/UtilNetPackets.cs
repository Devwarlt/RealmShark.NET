namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets
{
    /// <summary>
    /// Util class for Raw, Ether, Ip4 and TCP packets.
    /// </summary>
    public static class UtilNetPackets
    {
        public const int BYTE_SIZE_IN_BYTES = 1;
        public const int SHORT_SIZE_IN_BYTES = 2;
        public const int INT_SIZE_IN_BYTES = 4;
        public const int LONG_SIZE_IN_BYTES = 8;

        public static void ValidateBounds(byte[] array, int offset, int len)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Array is null");
            }
            if (array.Length == 0)
            {
                throw new ArgumentException("Array is empty", nameof(array));
            }
            if (len == 0)
            {
                throw new ArgumentException("Zero len error", nameof(len));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset negative " + offset);
            }
            if (len < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(len), "Len negative " + len);
            }
            if (offset + len > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(len), $"Len plus offset larger than array offset: {offset} len: {len} array: {array.Length}");
            }
        }

        public static byte[] GetBytes(byte[] data, int offset, int length)
        {
            ValidateBounds(data, offset, length);

            byte[] subArray = new byte[length];
            Array.Copy(data, offset, subArray, 0, length);
            return subArray;
        }

        public static int GetByte(byte[] data, int typeOffset)
        {
            return 0xFF & data[typeOffset];
        }

        public static int GetShort(byte[] data, int typeOffset)
        {
            return 0xFFFF & ((data[typeOffset] << 8) | (data[typeOffset + 1] & 0xFF));
        }

        public static int GetInt(byte[] data, int typeOffset)
        {
            return (data[typeOffset] << 24) | ((data[typeOffset + 1] & 0xFF) << 16) | ((data[typeOffset + 2] & 0xFF) << 8) | (data[typeOffset + 3] & 0xFF);
        }

        public static long GetIntAsLong(byte[] data, int typeOffset)
        {
            return (uint)((data[typeOffset] << 24) | ((data[typeOffset + 1] & 0xFF) << 16) | ((data[typeOffset + 2] & 0xFF) << 8) | (data[typeOffset + 3] & 0xFF));
        }
    }
}