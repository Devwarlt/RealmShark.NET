using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    /// <summary>
    /// Stream constructor ordering TCP packets in sequence. Payload is extracted and sent back in its raw form.
    /// </summary>
    public class TcpStreamBuilder
    {
        public readonly Dictionary<long, TcpPacket> PacketMap = new Dictionary<long, TcpPacket>();
        public long SequenceNumber;
        public int IdNumber;

        private readonly PStream packetStream;
        private readonly PReset packetReset;

        /// <summary>
        /// Constructor of StreamConstructor which needs a reset class to reset if reset
        /// packet is retrieved and a constructor class to send ordered packets to.
        /// </summary>
        /// <param name="preset">Reset class if a reset packet is retrieved.</param>
        /// <param name="pstream">Constructor class to send ordered packets to.</param>
        public TcpStreamBuilder(PReset preset, PStream pstream)
        {
            packetReset = preset;
            packetStream = pstream;
        }

        /// <summary>
        /// Build method for ordering packets according to index used by TCP.
        /// </summary>
        /// <param name="packet">TCP packets needing to be ordered.</param>
        public void StreamBuilder(TcpPacket packet)
        {
            if (packet.ResetBit)
            {
                if (packet.Syn)
                {
                    Reset();
                }
                return;
            }
            if (SequenceNumber == 0)
            {
                SequenceNumber = packet.SequenceNumber;
                IdNumber = packet.Ip4Packet.Identification;
            }

            PacketMap[packet.SequenceNumber] = packet;

            TcpStreamErrorHandler.INSTANCE.ErrorChecker(this);

            try
            {
                while (PacketMap.ContainsKey(SequenceNumber))
                {
                    TcpPacket packetSeqed = PacketMap[SequenceNumber];
                    PacketMap.Remove(SequenceNumber);
                    IdNumber = packetSeqed.Ip4Packet.Identification;
                    if (packet.Payload != null)
                    {
                        SequenceNumber += packetSeqed.PayloadSize;
                        packetStream.Stream(packetSeqed.Payload, packetSeqed.Ip4Packet.SrcAddr);
                    }
                }
            }
            catch (Exception e)
            {
                TcpStreamErrorHandler.INSTANCE.DumpData($"Logging errors caused (by TCP stream builder). \n {string.Join(", ", e.StackTrace)}");
            }
        }

        /// <summary>
        /// Reset method if a reset packet is retrieved.
        /// </summary>
        public void Reset()
        {
            packetReset.Reset();
            PacketMap.Clear();
            SequenceNumber = 0;
            IdNumber = 0;
        }
    }
}