namespace RotMGStats.RealmShark.NET.extensions
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Converts the byte to an unsigned integer.
        /// </summary>
        /// <param name="value">The byte value to convert.</param>
        /// <returns>The unsigned integer representation of the byte.</returns>
        public static int ToUnsignedInt(this byte value)
        {
            return value & 0xFF;
        }
    }
}