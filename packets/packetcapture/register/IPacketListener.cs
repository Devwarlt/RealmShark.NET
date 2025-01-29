using RotMGStats.RealmShark.NET.packets;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.register
{
    /// <summary>
    /// Listener interface used in the registry class matching subscribed classes to packet classes.
    /// </summary>
    /// <typeparam name="T">The type of packet.</typeparam>
    public interface IPacketListener<T> where T : Packet
    {
        void Process(T packet);
    }
}