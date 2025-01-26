namespace RotMGStats.RealmShark.NET.extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// Converts the int to an unsigned long.
        /// </summary>
        /// <param name="value">The int value to convert.</param>
        /// <returns>The unsigned long representation of the int.</returns>
        public static ulong ToUnsignedLong(this int value)
        {
            return (ulong)(uint)value;
        }
    }
}