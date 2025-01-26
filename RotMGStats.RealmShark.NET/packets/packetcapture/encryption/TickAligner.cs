using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly;
using RotMGStats.RealmShark.NET.util;
using RotMGStats.RealmShark.NET.java;
using System.Net.Sockets;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.encryption
{
    /// <summary>
    /// The RC4 cipher can be aligned using Tick packets. The packets send ticks regularly
    /// from the server incremented by one between each tick packet. The aligner first aligns
    /// the RC4 cipher using a simple brute force check from two consecutive tick packets.
    /// If the tick counts miss matches relative to next tick packet, it clears all alignments
    /// and re-aligns.
    /// </summary>
    public class TickAligner
    {
        private bool synced = false;
        private int packetBytes = 0;
        private RC4 rc4;
        private RC4 rc4Fork;
        private byte[] TickA;
        private int CURRENT_TICK;

        /// <summary>
        /// Tick aligner constructor to a RC4 cipher.
        /// </summary>
        /// <param name="r">The cipher to align.</param>
        public TickAligner(RC4 r)
        {
            rc4 = r;
            rc4Fork = r.Fork();
        }

        /// <summary>
        /// A comprehensive method that keeps track of tick packets and ensures each tick packet
        /// is aligned correctly against the CURRENT_TICK index counter. When tick packets arrive
        /// they are expected to have the same number as CURRENT_TICK + 1. If miss match is found
        /// an error is thrown. The cipher is reset and a brute force method is used to re-align
        /// the cipher with two consecutive tick packets.
        /// </summary>
        /// <param name="encryptedData">Data of the current receiving packet.</param>
        /// <param name="size">Size of the packet data.</param>
        /// <param name="type">Type of the packet</param>
        /// <returns>Returns the state of the cipher alignment being synced.</returns>
        public bool CheckRC4Alignment(ByteBuffer encryptedData, int size, int type)
        {
            if (synced)
            {
                if (type == PacketType.NEWTICK.GetIndex() || type == PacketType.MOVE.GetIndex())
                {
                    byte[] duplicate = encryptedData.ToArray().Skip(5).Take(encryptedData.Capacity).ToArray();
                    rc4.Copy(rc4Fork);
                    rc4Fork.Decrypt(duplicate);
                    CURRENT_TICK++;
                    int tick = Util.DecodeInt(duplicate);
                    if (CURRENT_TICK != tick)
                    {
                        string error = "Timeline synchronization critical failure, got: " + tick + " expected: " + CURRENT_TICK;
                        TcpStreamErrorHandler.INSTANCE.DumpData(error);
                        rc4.Reset();
                        synced = false;
                        TickA = null;
                    }
                }
                if (synced) packetBytes += size - 5;
            }

            if (!synced)
            {
                if (type == PacketType.NEWTICK.GetIndex() || type == PacketType.MOVE.GetIndex())
                {
                    byte[] tick = encryptedData.ToArray().Skip(5).Take(4).ToArray();
                    if (TickA != null)
                    {
                        rc4.Reset();
                        Console.WriteLine("Packet bytes between sync packets: " + packetBytes);
                        int i = RC4Aligner.SyncCipher(rc4, TickA, tick, packetBytes);
                        if (i != -1)
                        {
                            synced = true;
                            rc4.Skip(packetBytes).Decrypt(tick);
                            rc4.Skip(size - 5 - 4);
                            CURRENT_TICK = Util.DecodeInt(tick);
                            Console.WriteLine("Synced. offset: " + i + " tick: " + CURRENT_TICK);
                        }
                        else
                        {
                            Util.PrintLogs("Time Sync Failed");
                        }
                        TickA = null;
                        packetBytes = 0;
                    }
                    else
                    {
                        TickA = tick;
                        packetBytes = size - 5;
                    }
                }
                else
                {
                    packetBytes += size - 5;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// A reset method for resetting the tick counter. Called when changing game sessions.
        /// </summary>
        public void Reset()
        {
            CURRENT_TICK = -1;
        }
    }
}