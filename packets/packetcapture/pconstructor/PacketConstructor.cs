using System;
using System.Linq;
using System.Text;
using NLog;
using RotMGStats.RealmShark.NET.java;
using RotMGStats.RealmShark.NET.packets.packetcapture;
using RotMGStats.RealmShark.NET.packets.packetcapture.encryption;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.pconstructor
{
    /// <summary>
    /// Packet constructor sending the TCP packets to the stream constructor that in turn sends the
    /// ordered packets to the rotmg constructor. The packets are then sent back to be decrypted with
    /// an RC4 cipher. If a tick packet is sent an aligner is used to check the RC4 alignment using a
    /// simple increment from the previous tick packet. If a new session packet is received then it
    /// resets the cipher.
    /// </summary>
    public class PacketConstructor
    {
        private readonly RC4 rc4Cipher;
        private readonly PacketProcessor packetProcessor;
        private readonly ROTMGPacketConstructor rotmgConst;
        private readonly TickAligner tickAligner;
        private bool firstNonLargePacket;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Packet constructor with specific cipher.
        /// </summary>
        /// <param name="pp">Parent class to send constructed packets back too.</param>
        /// <param name="r">The cipher used to decode packets.</param>
        public PacketConstructor(PacketProcessor pp, RC4 r)
        {
            packetProcessor = pp;
            rc4Cipher = r;
            rotmgConst = new ROTMGPacketConstructor(this);
            tickAligner = new TickAligner(rc4Cipher);
        }

        /// <summary>
        /// Build method to send the packets retrieved by the sniffer for constructing.
        /// </summary>
        /// <param name="data">Raw packet data incoming from the net tap.</param>
        public void Build(byte[] data)
        {
            if (firstNonLargePacket)
            {
                // start listening after a non-max packet
                // prevents errors in pSize.
                if (data.Length < 1460) firstNonLargePacket = false;
                return;
            }
            rotmgConst.Build(data);
        }

        /// <summary>
        /// Rotmg packets constructed by the rotmg constructor are sent back after they
        /// are correctly assembled. If the cipher is correctly aligned then the packets
        /// are decrypted and sent to the packet processor. If the cipher isn't aligned
        /// then the Tick packets are used to re-align the cipher.
        /// </summary>
        /// <param name="encryptedData">Encrypted packets for aligning cipher and decryption.</param>
        public void PacketReceived(ArraySegment<byte> encryptedData)
        {
            try
            {
                var buffer = new byte[encryptedData.Count];
                Array.Copy(encryptedData.Array, encryptedData.Offset, buffer, 0, encryptedData.Count);
                var byteBuffer = new ByteBuffer(buffer);

                int size = byteBuffer.GetInt();
                int type = byteBuffer.Get() & 0xFF;

                bool sync = tickAligner.CheckRC4Alignment(byteBuffer, size, type);

                if (sync)
                {
                    rc4Cipher.Decrypt(5, byteBuffer); // encryptedData is decrypted in this method
                    packetProcessor.ProcessPackets(type, size, byteBuffer);
                }
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e);
            }
        }

        /// <summary>
        /// Reset method to reset both cipher and the aligner tick counter when a reset packet is received.
        /// </summary>
        public void Reset()
        {
            rc4Cipher.Reset();
            tickAligner.Reset();
            rotmgConst.Reset();
        }

        /// <summary>
        /// Reset when starting the sniffer. Given the program can start at any time then any packet which
        /// follows a non-max packet will most likely contain the rotmg-packet header which contains the
        /// packet size. If ignoring this flag, any random MTU(maximum transmission unit packet) packet in
        /// a sequence of concatenated packets could produce a random packet size from its first 4 bytes
        /// resulting in a de-sync.
        /// </summary>
        public void StartResets()
        {
            firstNonLargePacket = true;
        }
    }
}