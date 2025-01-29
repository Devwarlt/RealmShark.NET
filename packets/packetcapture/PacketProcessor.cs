using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using NLog;
using RotMGStats.RealmShark.NET.java;
using RotMGStats.RealmShark.NET.packets;
using RotMGStats.RealmShark.NET.packets.incoming.ip;
using RotMGStats.RealmShark.NET.packets.packetcapture.encryption;
using RotMGStats.RealmShark.NET.packets.packetcapture.logger;
using RotMGStats.RealmShark.NET.packets.packetcapture.pconstructor;
using RotMGStats.RealmShark.NET.packets.packetcapture.register;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff;
using RotMGStats.RealmShark.NET.packets.reader;
using RotMGStats.RealmShark.NET.util;

namespace RotMGStats.RealmShark.NET.packets.packetcapture
{
    /// <summary>
    /// The core class to process packets. First the network tap is sniffed to receive all packets. The packets
    /// are filtered for port 2050, the rotmg port, and TCP packets. Then the packets are stitched together in
    /// streamConstructor and rotmgConstructor class. After the packets are constructed the RC4 cipher is used
    /// decrypt the data. The data is then matched with target classes and emitted through the registry.
    /// </summary>
    public class PacketProcessor : PProcessor
    {
        private readonly PacketConstructor incomingPacketConstructor;
        private readonly PacketConstructor outgoingPacketConstructor;
        private readonly Sniffer sniffer;
        private readonly PacketLogger packetLogger;
        private readonly byte[] srcAddr;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Thread thread;

        /// <summary>
        /// Basic constructor of packetProcessor
        /// TODO: Add linux and mac support later
        /// </summary>
        public PacketProcessor()
        {
            sniffer = new Sniffer(this);
            incomingPacketConstructor = new PacketConstructor(this, new RC4(RotMGRC4Keys.INCOMING_STRING));
            outgoingPacketConstructor = new PacketConstructor(this, new RC4(RotMGRC4Keys.OUTGOING_STRING));
            packetLogger = new PacketLogger();
            srcAddr = new byte[4];
        }

        /// <summary>
        /// Start method for PacketProcessor.
        /// </summary>
        public void Run()
        {
            thread = new Thread(TapPackets);
            thread.Start();
        }

        /// <summary>
        /// Stop method for PacketProcessor.
        /// </summary>
        public void StopSniffer()
        {
            sniffer.CloseSniffers();

            try { thread.Join(1000); }
            catch { }
        }

        /// <summary>
        /// Method to start the packet sniffer that will send packets back to receivedPackets.
        /// </summary>
        public void TapPackets()
        {
            packetLogger.StartLogger();
            incomingPacketConstructor.StartResets();
            outgoingPacketConstructor.StartResets();
            try
            {
                sniffer.StartSniffer();
            }
            catch (TypeLoadException)
            {
                //new MissingNpcapGUI();
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e);
            }
        }

        /// <summary>
        /// Incoming byte data received from incoming TCP packets.
        /// </summary>
        /// <param name="data">Incoming byte stream</param>
        /// <param name="srcAddr">Source IP of incoming packets.</param>
        public void IncomingStream(byte[] data, byte[] srcAddr)
        {
            packetLogger.AddIncoming(data.Length);
            IpEmitter(srcAddr);
            incomingPacketConstructor.Build(data);
            Register.Instance.EmitLogs(packetLogger);
        }

        /// <summary>
        /// Outgoing byte data received from outgoing TCP packets.
        /// </summary>
        /// <param name="data">Outgoing byte stream</param>
        /// <param name="srcAddr">Source IP of outgoing packets.</param>
        public void OutgoingStream(byte[] data, byte[] srcAddr)
        {
            packetLogger.AddOutgoing(data.Length);
            outgoingPacketConstructor.Build(data);
            Register.Instance.EmitLogs(packetLogger);
        }

        /// <summary>
        /// Emits IP changes as incoming packet.
        /// </summary>
        /// <param name="srcIp">Source IP of incoming packets.</param>
        private void IpEmitter(byte[] srcIp)
        {
            for (int i = 0; i < srcAddr.Length; i++)
            {
                if (srcAddr[i] != srcIp[i])
                {
                    Array.Copy(srcIp, 0, srcAddr, 0, srcAddr.Length);
                    Register.Instance.EmitPacketLogs(new IpAddress(srcIp));
                    return;
                }
            }
        }

        /// <summary>
        /// Completed packets constructed by stream and rotmg constructor returned to packet constructor.
        /// Decoded by the cipher and sent back to the processor to be emitted to subscribed users.
        /// </summary>
        /// <param name="type">Constructed packet type.</param>
        /// <param name="size">Size of the packet.</param>
        /// <param name="data">Constructed packet data.</param>
        public void ProcessPackets(int type, int size, ByteBuffer data)
        {
            if (!PacketType.ContainsKey(type))
            {
                logger.Log(LogLevel.Error, $"Unknown packet type: {type} Data: {string.Join(", ", data.ToArray())}");
                return;
            }
            packetLogger.AddPacket(type, size);
            Packet packetType = PacketType.GetPacket(type);
            packetType.SetData(data.ToArray());
            BufferReader pData = new BufferReader(data);

            try
            {
                packetType.Deserialize(pData);
                if (!pData.IsBufferFullyParsed())
                {
                    pData.PrintError(packetType);
                }
            }
            catch (Exception e)
            {
                Util.PrintLogs($"Buffer exploded: {pData.GetIndex()}/{pData.Size}");
                DebugPackets(type, data.ToArray());
                return;
            }
            Register.Instance.EmitPacketLogs(packetType);
        }

        /// <summary>
        /// Helper for debugging packets
        /// </summary>
        private void DebugPackets(int type, byte[] data)
        {
            Packet packetType = PacketType.GetPacket(type);
            Util.PrintLogs($"{PacketType.ByOrdinal(type)} {packetType}");
            Util.PrintLogs(string.Join(", ", data));
        }

        /// <summary>
        /// Closes the sniffer for shutdown.
        /// </summary>
        public void CloseSniffer()
        {
            sniffer.CloseSniffers();
        }

        public void ResetIncoming()
        {
            incomingPacketConstructor.Reset();
        }

        public void ResetOutgoing()
        {
            outgoingPacketConstructor.Reset();
        }
    }
}