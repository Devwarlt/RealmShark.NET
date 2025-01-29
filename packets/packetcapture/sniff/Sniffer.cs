using System;
using System.Linq;
using System.Threading;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.ardikars;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets;
using RotMGStats.RealmShark.NET.util;
using static RotMGStats.RealmShark.NET.packets.packetcapture.sniff.ardikars.NativeBridge;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff
{
    /// <summary>
    /// A sniffer used to tap packets out of the Windows OS network layer. Before sniffing
    /// packets it needs to find what network interface the packets are sent or received from,
    /// aka if proxies are used.
    /// </summary>
    public class Sniffer
    {
        private static readonly bool DisableChecksum = true; // disabled given most routers checksum packets automatically.
        private readonly int port = 2050; // 2050 is default rotmg server port.
        private readonly Sniffer thisObject;
        private readonly RingBuffer<RawPacket> ringBuffer;
        private readonly TcpStreamBuilder incoming;
        private readonly TcpStreamBuilder outgoing;
        private PacketCommunicator[] pcaps;
        private PacketCommunicator realmPcap;
        private bool stop;

        /// <summary>
        /// Constructor of a Windows sniffer.
        /// </summary>
        /// <param name="processor">PProcessor instance used as the base.</param>
        public Sniffer(PProcessor processor)
        {
            thisObject = this;
            ringBuffer = new RingBuffer<RawPacket>(32);
            incoming = new TcpStreamBuilder(processor.ResetIncoming, processor.IncomingStream);
            outgoing = new TcpStreamBuilder(processor.ResetOutgoing, processor.OutgoingStream);
        }

        /// <summary>
        /// Main sniffer method to listen on the network tap for any packets filtered by port
        /// 2050 (default port rotmg uses) and TCP packets only (the packet type rotmg uses).
        /// All network interfaces are listen to given some users might have multiple. A thread
        /// is created to listen to all interfaces until any packet of the correct type (port
        /// 2050 of type TCP) is found. The all other channels are halted and only the correct
        /// interface is listened on.
        /// </summary>
        public void StartSniffer()
        {
            var devices = LivePacketDevice.AllLocalMachine;
            pcaps = new PacketCommunicator[devices.Count];
            realmPcap = null;
            stop = false;

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var communicator = device.Open(65536, PacketDeviceOpenAttributes.Promiscuous, 1000);
                pcaps[i] = communicator;

                communicator.SetFilter("tcp port " + port);
                StartPacketSniffer(communicator);
            }

            CloseUnusedSniffers();
            ProcessBufferedPackets();
        }

        /// <summary>
        /// Small pauses for async to finish tasks.
        /// </summary>
        /// <param name="ms">Millisecond of pause</param>
        private static void Pause(int ms)
        {
            try
            {
                Thread.Sleep(ms);
            }
            catch (ThreadInterruptedException) { }
        }

        /// <summary>
        /// Start a packet sniffers on different threads
        /// and close any sniffer not being used.
        /// </summary>
        /// <param name="pcap">Current handle to the PacketCommunicator instance.</param>
        public void StartPacketSniffer(PacketCommunicator pcap)
        {
            new Thread(() =>
            {
                PacketListener listener = packet =>
                {
                    TcpStreamErrorHandler.Instance.LogTCPPacket(packet);

                    if (packet != null && ComputeChecksum(packet.Payload))
                    {
                        lock (ringBuffer)
                        {
                            ringBuffer.Push(packet);
                        }
                        realmPcap = pcap;
                        lock (thisObject)
                        {
                            Monitor.PulseAll(thisObject);
                        }
                    }
                };
                NativeBridge.Loop(pcap, -1, listener);
            }).Start();
            Pause(1);
        }

        /// <summary>
        /// Close threads of sniffer network interfaces not being used after
        /// capturing at least one realm packet in the correct net-interface.
        /// </summary>
        private void CloseUnusedSniffers()
        {
            try
            {
                lock (thisObject)
                {
                    Monitor.Wait(thisObject);
                }
                while (!stop)
                {
                    if (realmPcap != null)
                    {
                        foreach (var pcap in pcaps)
                        {
                            if (pcap != null && realmPcap != pcap)
                            {
                                pcap.Dispose();
                            }
                        }
                        return;
                    }
                    Pause(100);
                }
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Processing waits until new packets are captured by the sniffer, wakes
        /// up and processes the buffered packets in the ring buffer and goes
        /// back to sleep.
        /// </summary>
        private void ProcessBufferedPackets()
        {
            try
            {
                while (!stop)
                {
                    lock (thisObject)
                    {
                        Monitor.Wait(thisObject);
                    }
                    while (!ringBuffer.IsEmpty())
                    {
                        RawPacket packet;
                        lock (ringBuffer)
                        {
                            packet = ringBuffer.Pop();
                        }
                        if (packet == null) continue;

                        try
                        {
                            var ethernetPacket = packet.GetNewEthernetPacket();
                            if (ethernetPacket != null)
                            {
                                var ip4packet = ethernetPacket.GetNewIp4Packet();
                                var assembledIp4packet = Ip4Defragmenter.Defragment(ip4packet);
                                if (assembledIp4packet != null)
                                {
                                    var tcpPacket = assembledIp4packet.GetNewTcpPacket();
                                    if (tcpPacket != null)
                                    {
                                        ReceivedPackets(tcpPacket);
                                    }
                                }
                            }
                        }
                        catch (Exception e) when (e is IndexOutOfRangeException || e is ArgumentException || e is NullReferenceException)
                        {
                            Util.PrintLogs(e.Message);
                            Util.PrintLogs(string.Join(", ", packet.Payload));
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Sorting method to arrange incoming and outgoing packets based on ports.
        /// </summary>
        /// <param name="packet">The TCP packets retrieved from the network tap.</param>
        private void ReceivedPackets(TcpPacket packet)
        {
            if (packet.SrcPort == port) // Incoming packets have 2050 source port.
            {
                incoming.StreamBuilder(packet);
            }
            else if (packet.DstPort == port) // Outgoing packets have 2050 destination port.
            {
                outgoing.StreamBuilder(packet);
            }
        }

        /// <summary>
        /// Verify checksum of TCP packets. This does however not checksum the Ip4Header
        /// given only the data of the TCP packet is vital. Not the header data.
        /// <para>
        /// WARNING! Don't use this checksum given the router handles checksums. Filtering packets
        /// with checksum results in packets being lost. Even if the checksum fails the packets
        /// pass the RC4 cipher meaning the packets are fine, even if the checksum miss matches.
        /// </para>
        /// </summary>
        /// <param name="bytes">Raw bytes of the packet being received.</param>
        /// <returns>true if the checksum is similar to the TCP checksum sent in the packet.</returns>
        private static bool ComputeChecksum(byte[] bytes)
        {
            if (DisableChecksum) return true;
            int tcpLen = (bytes[17] & 0xFF) + ((bytes[16] & 0xFF) << 8) - ((bytes[14] & 15) * 4);
            int sum = 6 + tcpLen; // add tcp num + length of tcp

            for (int i = 26; i < tcpLen + 33; i += 2) // compute all byte pairs starting from ip dest/src to end of tcp payload
            {
                if (i == 50) continue; // skip the TCP checksum values at address 50 & 51
                sum += (bytes[i + 1] & 0xFF) + ((bytes[i] & 0xFF) << 8);
            }

            if ((tcpLen & 1) == 1) // add the last odd pair as if the whole packet had a zero byte added to the end
                sum += (bytes[bytes.Length - 1] & 0xFF) << 8;

            while ((sum >> 16) != 0) // one compliment
                sum = (sum & 0xFFFF) + (sum >> 16);

            sum = ~sum; // invert bits
            sum = sum & 0xFFFF; // remove upper bits

            int checksumTCP = (bytes[51] & 0xFF) + ((bytes[50] & 0xFF) << 8);
            if (checksumTCP == 0xFFFF) checksumTCP = 0; // get checksum from tcp packet and set to 0 if value is FFFF,
            //                                                                              FFFF is impossible to have.

            return checksumTCP == sum;
        }

        /// <summary>
        /// Close all network interfaces sniffing the wire.
        /// </summary>
        public void CloseSniffers()
        {
            stop = true;
            if (realmPcap != null)
            {
                realmPcap.Dispose();
            }
            else
            {
                try
                {
                    foreach (var c in pcaps)
                    {
                        c?.Dispose();
                    }
                }
                catch (NullReferenceException)
                {
                    // Network tap is already closed
                    Console.WriteLine("[X] Error stopping sniffer: sniffer not running.");
                }
            }
        }
    }
}