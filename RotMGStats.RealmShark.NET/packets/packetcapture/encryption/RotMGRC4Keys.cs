namespace RotMGStats.RealmShark.NET.packets.packetcapture.encryption
{
    public static class RotMGRC4Keys
    {
        /// <summary>
        /// Incoming packet key from the rotmg servers to the client.
        /// </summary>
        public static string INCOMING_STRING = "c91d9eec420160730d825604e0";

        /// <summary>
        /// Outgoing packet key from the client to the rotmg servers.
        /// </summary>
        public static string OUTGOING_STRING = "5a4d2016bc16dc64883194ffd9";
    }
}