using System;
using System.Collections.Generic;
using RotMGStats.RealmShark.NET.java.util;
using RotMGStats.RealmShark.NET.packets;
using RotMGStats.RealmShark.NET.packets.packetcapture.logger;
using RotMGStats.RealmShark.NET.util;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.register
{
    /// <summary>
    /// The registry class is used to subscribe to either all or specific packets. If registered packets
    /// are received the emit method will send an update and trigger the lambda used.
    /// </summary>
    public class Register
    {
        public static readonly Register Instance = new Register();
        private readonly Dictionary<Type, List<IPacketListener<Packet>>> packetListeners = new Dictionary<Type, List<IPacketListener<Packet>>>();
        private bool emitting = false;
        private readonly List<Pair<List<IPacketListener<Packet>>, IPacketListener<Packet>>> remove = new List<Pair<List<IPacketListener<Packet>>, IPacketListener<Packet>>>();
        private static readonly List<Subscriber> subscribePacketLogs = new List<Subscriber>();

        /// <summary>
        /// Emitter for sending packets to any subscriber which matches the packets the subscriber have subbed too.
        /// </summary>
        /// <param name="packet">The packet being received and emitted.</param>
        public void EmitPacketLogs(Packet packet)
        {
            emitting = true;
            if (packetListeners.ContainsKey(packet.GetType()))
            {
                foreach (var processor in packetListeners[packet.GetType()])
                {
                    processor.Process(packet);
                }
            }

            if (packetListeners.ContainsKey(typeof(Packet)))
            {
                foreach (var processor in packetListeners[typeof(Packet)])
                {
                    processor.Process(packet);
                }
            }
            emitting = false;

            if (remove.Count > 0)
            {
                foreach (var p in remove)
                {
                    p.Left().Remove(p.Right());
                }
                remove.Clear();
            }
        }

        /// <summary>
        /// Register method to subscribe to packets that are being received from the network tap.
        /// </summary>
        /// <param name="type">The type of class wanting to be subscribed too.</param>
        /// <param name="processor">The lambda needed to trigger what event should happen if packet is received.</param>
        public void RegisterSingle(PacketType type, IPacketListener<Packet> processor)
        {
            if (!packetListeners.ContainsKey(type.GetPacketClass()))
            {
                packetListeners[type.GetPacketClass()] = new List<IPacketListener<Packet>>();
            }
            packetListeners[type.GetPacketClass()].Add(processor);
        }

        /// <summary>
        /// Register method to subscribe to all packets that are being received from the network tap.
        /// </summary>
        /// <param name="processor">The lambda needed to trigger what event should happen if packet is received.</param>
        public void RegisterAll(IPacketListener<Packet> processor)
        {
            if (!packetListeners.ContainsKey(typeof(Packet)))
            {
                packetListeners[typeof(Packet)] = new List<IPacketListener<Packet>>();
            }
            packetListeners[typeof(Packet)].Add(processor);
        }

        /// <summary>
        /// Removes a registered method and stops the method receiving network packets.
        /// </summary>
        /// <param name="type">The type of class wanting to be un-subscribed from.</param>
        /// <param name="processor">The lambda needed to identify what method to unregister.</param>
        /// <returns>True if the removal is successful.</returns>
        public bool Unregister(PacketType type, IPacketListener<Packet> processor)
        {
            if (packetListeners.TryGetValue(type.GetPacketClass(), out var list))
            {
                if (list.Count == 1)
                {
                    return packetListeners.Remove(type.GetPacketClass());
                }
                else if (!emitting)
                {
                    return list.Remove(processor);
                }
                else if (list.Contains(processor))
                {
                    remove.Add(new Pair<List<IPacketListener<Packet>>, IPacketListener<Packet>>(list, processor));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Subscription to logger for incoming and outgoing packets.
        /// Example: PacketLogger.Subscribe(e => LogOut(e));
        /// LogOut(string logs) being a method that prints the logs.
        /// </summary>
        /// <param name="sub">Interface for subscription used in lambda.</param>
        public void SubscribePacketLogger(Subscriber sub)
        {
            subscribePacketLogs.Add(sub);
        }

        /// <summary>
        /// Emits the logs to any subscriber.
        /// </summary>
        public void EmitLogs(PacketLogger logger)
        {
            foreach (var sub in subscribePacketLogs)
            {
                sub.Receive(logger.ToString());
            }
        }

        public interface Subscriber
        {
            void Receive(string msg);
        }
    }
}