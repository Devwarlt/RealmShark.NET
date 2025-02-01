using System.Text;
using RotMGStats.RealmShark.NET.packets.packetcapture.sniff.netpackets;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.assembly
{
    public class TcpStreamErrorHandler
    {
        public static TcpStreamErrorHandler Instance = new TcpStreamErrorHandler();
        private static int index = 0;
        private static int size = 500;
        private static RawPacket[] logList = new RawPacket[size];
        private static PErrorMessage errorMessage;
        private static PErrorStop errorStop;

        /// <summary>
        /// Raw packet logger for dumping error logs.
        /// </summary>
        /// <param name="tcp">Raw TCP packets.</param>
        public void LogTCPPacket(RawPacket tcp)
        {
            logList[index] = tcp;
            index++;
            if (index >= size) index = 0;
        }

        /// <summary>
        /// TCP raw packet byte dump for error logging.
        /// </summary>
        /// <param name="error">Message of the error.</param>
        public void DumpData(string error)
        {
            ErrorMessage(error, error + "\n" + GetRawPacketDump());
        }

        /// <summary>
        /// TCP stream error checker for instances where packets are missing in a TCP stream.
        /// </summary>
        /// <param name="tcpStreamBuilder">TCP packet object to be checked.</param>
        public void ErrorChecker(TcpStreamBuilder tcpStreamBuilder)
        {
            if (tcpStreamBuilder.PacketMap.Count > 95)
            {
                long index = tcpStreamBuilder.SequenceNumber;
                int counter = 0;
                while (counter < 100000)
                {
                    if (tcpStreamBuilder.PacketMap.ContainsKey(index))
                    {
                        tcpStreamBuilder.SequenceNumber = index;
                        TcpPacket tempPack = tcpStreamBuilder.PacketMap[index];
                        string errorMsg = "Packets missing. id:" + (tcpStreamBuilder.IdNumber - tempPack.Ip4Packet.Identification) + " seq:" + (tcpStreamBuilder.SequenceNumber - tempPack.SequenceNumber) + " outgoing:" + (tempPack.DstPort == 2050);
                        ErrorMessage(errorMsg, errorMsg);
                        break;
                    }
                    index++;
                    counter++;
                }
            }
            else if (tcpStreamBuilder.PacketMap.Count >= 100)
            {
                Stop();
                tcpStreamBuilder.Reset();
            }
        }

        /// <summary>
        /// Called when too many packets are missing in a TCP stream.
        /// </summary>
        private void Stop()
        {
            string errorMsg = "Error! Sniffer lost 100 packets from unknown reasons. Shutting down.";
            string dump = errorMsg + "\n" + GetRawPacketDump();
            ErrorMessage(errorMsg, dump);
            ErrorStop();
        }

        /// <summary>
        /// Creates a string form of the raw packets in the buffer for error dumping into logs.
        /// </summary>
        /// <returns>String format of raw packets in the buffer.</returns>
        private string GetRawPacketDump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Packet logger printing logs for possible error handling.\n");
            for (int i = index; i <= (index + size); i++)
            {
                int j = i % size;
                RawPacket packet = logList[j];
                if (packet != null)
                {
                    sb.Append(string.Join(", ", packet.Payload));
                    sb.Append(" ");
                    sb.Append(j);
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Sets the error message handler.
        /// </summary>
        /// <param name="errorMessageHandler">Lambda method to be used as error message handler.</param>
        public void SetErrorMessageHandler(PErrorMessage errorMessageHandler)
        {
            errorMessage = errorMessageHandler;
        }

        /// <summary>
        /// Sets the error stop handler.
        /// </summary>
        /// <param name="errorStopHandler">Lambda method to be used as error stop handler.</param>
        public void SetErrorStopHandler(PErrorStop errorStopHandler)
        {
            errorStop = errorStopHandler;
        }

        /// <summary>
        /// Error message caused by packets missing, if error message handler is set.
        /// </summary>
        /// <param name="errorMsg">Message of missing packets.</param>
        /// <param name="errorDump">Error dump for debugger</param>
        public void ErrorMessage(string errorMsg, string errorDump)
        {
            errorMessage?.ErrorLogs(errorMsg, errorDump);
        }

        /// <summary>
        /// Triggers after too many packets are missing allowing the sniffer to stop,
        /// if error stop handler is set.
        /// </summary>
        public void ErrorStop()
        {
            errorStop?.ErrorStop();
        }
    }
}