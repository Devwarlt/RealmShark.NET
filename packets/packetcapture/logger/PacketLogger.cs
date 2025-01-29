using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RotMGStats.RealmShark.NET.packets;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.logger
{
    /// <summary>
    /// Class used to log data traffic over the wire
    /// <para>
    /// TODO: clean up this mess of a class
    /// </para>
    /// </summary>
    public class PacketLogger
    {
        private static readonly string[] Suffix = { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        private long time;
        private readonly int slotIntervalInSeconds = 60;
        private Log[] timeSlotsIn;
        private Log[] timeSlotsOut;
        private Log inTotal;
        private Log outTotal;
        private readonly Dictionary<int, Log> packets = new Dictionary<int, Log>();
        private int inInterval = -1;
        private int outInterval = -1;

        /// <summary>
        /// Used when starting the sniffer and resetting all the logging data.
        /// </summary>
        public void StartLogger()
        {
            time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            timeSlotsIn = new Log[slotIntervalInSeconds];
            timeSlotsOut = new Log[slotIntervalInSeconds];
            for (int i = 0; i < slotIntervalInSeconds; i++) timeSlotsIn[i] = new Log();
            for (int i = 0; i < slotIntervalInSeconds; i++) timeSlotsOut[i] = new Log();
            packets.Clear();
            inTotal = new Log(0);
            outTotal = new Log(0);
        }

        /// <summary>
        /// Add amount of data that is incoming from realm servers.
        /// </summary>
        /// <param name="length">Number of bytes, only the TCP packet.</param>
        public void AddIncoming(int length)
        {
            length += 38; // Add IP and Ethernet header bytes as well.
            inTotal.Add(length);
            int interval = GetInterval();
            if (interval != inInterval)
            {
                inInterval = interval;
                timeSlotsIn[interval].Set(length);
            }
            else
            {
                timeSlotsIn[interval].Add(length);
            }
        }

        /// <summary>
        /// Add amount of data that is outgoing to realm servers.
        /// </summary>
        /// <param name="length">Number of bytes, only the TCP packet.</param>
        public void AddOutgoing(int length)
        {
            length += 58; // Add TCP (20 bytes) + IP (20 bytes) + Ethernet (18 bytes) header and tail bytes as well.
            outTotal.Add(length);
            int interval = GetInterval();
            if (interval != outInterval)
            {
                outInterval = interval;
                timeSlotsOut[interval].Set(length);
            }
            else
            {
                timeSlotsOut[interval].Add(length);
            }
        }

        /// <summary>
        /// Get a time interval in seconds based on the set interval "slotIntervalInSeconds"
        /// </summary>
        /// <returns>The time interval based on "slotIntervalInSeconds"</returns>
        private int GetInterval()
        {
            return ((int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000) % slotIntervalInSeconds);
        }

        /// <summary>
        /// Adds Number of bytes to the specified type of packet received
        /// </summary>
        /// <param name="type">Type of packet being logged</param>
        /// <param name="size">Number of bytes the specified type has</param>
        public void AddPacket(int type, int size)
        {
            if (packets.ContainsKey(type))
            {
                packets[type].Add(size);
            }
            else
            {
                packets[type] = new Log(type, size);
            }
        }

        /// <summary>
        /// Converts current time milliseconds to readable time.
        /// </summary>
        /// <returns>String of readable time from System.currentTimeMillis()</returns>
        private string GetTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(time).ToString("MMM dd,yyyy HH:mm");
        }

        /// <summary>
        /// Gets the data in the allocated log array.
        /// </summary>
        /// <param name="inout">Log array to be computed the data from.</param>
        /// <returns>A merged Log object with both number of bytes (size)
        /// and the amount of packets merged into one Log object.</returns>
        private Log GetMinData(Log[] inout)
        {
            Log min = new Log();
            foreach (Log l in inout)
            {
                min.Merge(l);
            }
            return min;
        }

        /// <summary>
        /// Text output of all logged data.
        /// </summary>
        /// <returns>Text output of all logged data.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Started " + GetTime() + "\n");

            sb.Append("\n");
            sb.Append("Incoming " + inTotal + "\n");
            sb.Append(GetMinData(timeSlotsIn) + " per min\n");

            sb.Append("\n");
            sb.Append("Outgoing " + outTotal + "\n");
            sb.Append(GetMinData(timeSlotsOut) + " per min\n");

            sb.Append("\n");
            sb.Append("Packets\n");

            foreach (var log in packets.Values.OrderByDescending(l => l))
            {
                sb.Append(log);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Log class to keep track of incoming number of bytes and the total number of packets.
        /// </summary>
        private class Log : IComparable<Log>
        {
            private int count;
            private int size;
            private int type;

            public Log()
            {
                count = 0;
                size = 0;
                type = -1;
            }

            public Log(int s)
            {
                count = 0;
                size = s;
                type = -1;
            }

            public Log(int t, int s)
            {
                count = 1;
                size = s;
                type = t;
            }

            public void Add(int s)
            {
                count++;
                size += s;
            }

            public void Set(int s)
            {
                count = 0;
                size = s;
            }

            public void Merge(Log l)
            {
                count += l.count;
                size += l.size;
            }

            public override string ToString()
            {
                int suffixIndex = 0;
                float bytes = size;
                while (bytes > 5000)
                {
                    suffixIndex++;
                    bytes /= 1000;
                }

                string s = suffixIndex == 0 ? $"{size} {PacketLogger.Suffix[suffixIndex]}" : $"{bytes:F2} {PacketLogger.Suffix[suffixIndex]}";

                if (type != -1)
                {
                    return $"Num:{count,5} Siz: {s} {PacketType.ByOrdinal(type)}\n";
                }
                else
                {
                    return $"Num:{count} Siz: {s}";
                }
            }

            public int CompareTo(Log other)
            {
                return other.size - size;
            }
        }
    }
}