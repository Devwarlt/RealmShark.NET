namespace RotMGStats.RealmShark.NET.extensions
{
    public static class ShortExtensions
    {
        /// <summary>
        /// Converts the short to an unsigned integer.
        /// </summary>
        /// <param name="value">The short value to convert.</param>
        /// <returns>The unsigned integer representation of the short.</returns>
        public static int ToUnsignedInt(this short value)
        {
            return value & 0xFFFF;
        }
    }
}