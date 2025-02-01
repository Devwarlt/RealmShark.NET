using System.Reflection;
using System.Runtime.InteropServices;
using NLog;
using SharpPcap;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets;
using SharpPcap.LibPcap;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.ardikars
{
    /// <summary>
    /// Bridge class to hook directly into native methods instead of using
    /// preset methods used by the ardikars library.
    /// </summary>
    public class NativeBridge
    {
        private static readonly Logger logger = LogManager.GetLogger(nameof(NativeBridge));

        /// <summary>
        /// for testing
        /// </summary>
        public static void Main(string[] args)
        {
            logger.Info("clearconsole");
            ICaptureDevice pcap;
            try
            {
                var devices = CaptureDeviceList.Instance;
                if (devices.Count == 0)
                {
                    logger.Error("No devices were found on this machine");
                    return;
                }

                var device = devices[0];
                pcap = device;
                pcap.Open(DeviceModes.Promiscuous, 1000);
                pcap.Filter = "tcp port 2050";
            }
            catch (Exception e)
            {
                logger.Error(e);
                return;
            }

            PacketListener listener = packet =>
            {
                try
                {
                    var ethernetPacket = packet.GetNewEthernetPacket();
                    if (ethernetPacket != null)
                    {
                        var ip4Packet = ethernetPacket.GetNewIp4Packet();
                        if (ip4Packet != null)
                        {
                            var tcpPacket = ip4Packet.GetNewTcpPacket();
                            logger.Info(tcpPacket);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            };
            try
            {
                Loop(pcap, -1, listener);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        /// <summary>
        /// The main looping function on the network tap.
        /// This is method calls the wrapper method that
        /// in turn starts the packet sniffing.
        /// </summary>
        /// <param name="pcap">Packet capture class wrapping the interface for sniffing the wire.</param>
        /// <param name="packetCount">Number of packets to listen to. -1 loops infinitely.</param>
        /// <param name="listener">Lambda abstract interface used when packets are captured.</param>
        //public static void Loop(ICaptureDevice pcap, int packetCount, PacketListener listener)
        //{
        //    try
        //    {
        //        var field = pcap.GetType().GetField("pcapHandle", BindingFlags.NonPublic | BindingFlags.Instance);
        //        var p = (IntPtr)field.GetValue(pcap);

        //        NativeMappings.pcap_loop(p, packetCount, new GotPacketFuncExecutor(listener, SimpleExecutor.Instance).GotPacket, IntPtr.Zero);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //    }
        //}
        public static void Loop(ICaptureDevice pcap, int packetCount, PacketListener listener)
        {
            try
            {
                if (pcap is LibPcapLiveDevice liveDevice)
                {
                    var pcapHandle = liveDevice.Handle.DangerousGetHandle();
                    _ = NativeMappings.pcap_loop(pcapHandle,
                                                 packetCount,
                                                 new GotPacketFuncExecutor(listener, SimpleExecutor.Instance).GotPacket,
                                                 IntPtr.Zero);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported ICaptureDevice type.");
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        /// <summary>
        /// Returns a list of all interfaces on the device.
        /// </summary>
        /// <returns>List of all interfaces on the device.</returns>
        public static ICaptureDevice[] GetInterfaces()
        {
            var allDevices = CaptureDeviceList.Instance;
            if (allDevices.Count == 0)
                return Array.Empty<ICaptureDevice>();

            return allDevices.ToArray();
        }

        /// <summary>
        /// Interface class for responding to captured packets.
        /// </summary>
        public delegate void PacketListener(RawPacket packet);

        /// <summary>
        /// Thread executor when packets are captured.
        /// </summary>
        public sealed class SimpleExecutor : TaskScheduler
        {
            private SimpleExecutor()
            { }

            private static readonly SimpleExecutor instance = new SimpleExecutor();

            public static SimpleExecutor Instance => instance;

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return null;
            }

            protected override void QueueTask(Task task)
            {
                TryExecuteTask(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return TryExecuteTask(task);
            }
        }

        /// <summary>
        /// Interface class for unwrapping captured packets from native
        /// pointers to useful byte arrays and timestamps.
        /// </summary>
        private sealed class GotPacketFuncExecutor
        {
            private readonly PacketListener listener;
            private readonly TaskScheduler executor;

            public GotPacketFuncExecutor(PacketListener listener, TaskScheduler executor)
            {
                this.listener = listener;
                this.executor = executor;
            }

            public void GotPacket(IntPtr args, IntPtr header, IntPtr packet)
            {
                var len = NativeMappings.pcap_pkthdr.getLen(header);
                var data = new byte[NativeMappings.pcap_pkthdr.getCaplen(header)];
                Marshal.Copy(packet, data, 0, data.Length);

                try
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (data.Length == len)
                        {
                            listener(new RawPacket(data));
                        }
                    }, CancellationToken.None, TaskCreationOptions.None, executor);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }
    }

    internal static class NativeMappings
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void pcap_handler(IntPtr user, IntPtr header, IntPtr packet);

        [DllImport("wpcap.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int pcap_loop(IntPtr p, int cnt, pcap_handler callback, IntPtr user);

        public static class pcap_pkthdr
        {
            public static int getLen(IntPtr header)
            {
                return Marshal.ReadInt32(header, 8);
            }

            public static int getCaplen(IntPtr header)
            {
                return Marshal.ReadInt32(header, 12);
            }

            public static IntPtr getTvSec(IntPtr header)
            {
                return Marshal.ReadIntPtr(header);
            }

            public static IntPtr getTvUsec(IntPtr header)
            {
                return Marshal.ReadIntPtr(header, 4);
            }
        }
    }
}