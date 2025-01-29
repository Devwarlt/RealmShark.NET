using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using RotMGStats.RealmShark.NET.assets;

namespace RotMGStats.RealmShark.NET.util
{
    /// <summary>
    /// Generic utility class for utility methods.
    /// </summary>
    public class Util
    {
        public static bool SaveLogs { get; set; } = true;

        private static Dictionary<string, StreamWriter> printWriter = new Dictionary<string, StreamWriter>();

        /// <summary>
        /// Fast method to return an integer from the first 4 bytes of a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to extract the integer from.</param>
        /// <returns>The integer converted from the first 4 bytes of an array.</returns>
        public static int DecodeInt(byte[] bytes)
        {
            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        }

        /// <summary>
        /// Enable / disable log print-outs.
        /// </summary>
        /// <param name="logs">Set the saving of logs to true/false.</param>
        public static void SetSaveLogs(bool logs)
        {
            SaveLogs = logs;
        }

        /// <summary>
        /// Error logger.
        /// </summary>
        /// <param name="message">The error message.</param>
        public static void PrintLogs(string message)
        {
            PrintLogs("error/error", message);
        }

        /// <summary>
        /// Print logs to console or to files in a folderAndName.
        /// Log printouts can be disabled.
        /// </summary>
        /// <param name="folderAndName">The folder and the name to write the logs into.</param>
        /// <param name="s">String of the log.</param>
        public static void PrintLogs(string folderAndName, string s)
        {
            if (!SaveLogs)
            {
                Console.WriteLine(s);
            }
            else
            {
                Print(folderAndName, s);
            }
        }

        /// <summary>
        /// Print any files in a folderAndName.
        /// </summary>
        /// <param name="folderAndName">The folder and the name to write the logs into.</param>
        /// <param name="s">String of the log.</param>
        public static void Print(string folderAndName, string s)
        {
            if (!printWriter.TryGetValue(folderAndName, out var printWriterObject))
            {
                try
                {
                    printWriterObject = GetPrintWriter(folderAndName);
                    printWriter[folderAndName] = printWriterObject;
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }
            printWriterObject.WriteLine(s);
            printWriterObject.Flush();
        }

        /// <summary>
        /// Creates file and returns a StreamWriter object for printing text as output.
        /// </summary>
        /// <param name="folderAndName">Name of file to be placed in folder where app is started.
        /// Add folder path separated with "/".
        /// Include "-" end of file to not include time stamp.</param>
        /// <returns>Stream writer object to write text into.</returns>
        public static StreamWriter GetPrintWriter(string folderAndName)
        {
            StreamWriter printWriterObject;
            var dateTimeFormat = "yyyy-MM-dd-HH.mm.ss";
            var dateTime = DateTime.Now;
            string fileName;
            if (folderAndName.EndsWith("-"))
            {
                fileName = folderAndName.Substring(0, folderAndName.Length - 1);
            }
            else
            {
                fileName = folderAndName + "-" + dateTime.ToString(dateTimeFormat) + ".data";
            }
            var file = new FileInfo(fileName);
            if (!file.Directory.Exists)
            {
                try { file.Directory.Create(); }
                catch (IOException)
                {
                    Console.WriteLine("[X] Failed to create path for logfile '" + fileName + "'.");
                }

                if (!file.Exists)
                {
                    file.Create().Dispose();
                }
            }
            printWriterObject = new StreamWriter(file.OpenWrite());
            return printWriterObject;
        }

        /// <summary>
        /// Hex with lines printer.
        /// </summary>
        /// <param name="bytes">Byte array to be printed with pair of hex numbers separated with a line</param>
        /// <returns>Printed hex values with separated lines.</returns>
        public static string ByteArrayPrint(byte[] bytes)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            bool first = true;
            foreach (var b in bytes)
            {
                if (!first) sb.Append("|");
                first = false;
                sb.AppendFormat("{0:x2}", b);
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Receives a hex string and returns it in byte array format.
        /// </summary>
        /// <param name="hex">String of hex data where a pair of numbers represents a byte.</param>
        /// <returns>Returns a byte array converted from the passed hex string.</returns>
        public static byte[] HexStringToByteArray(string hex)
        {
            int l = hex.Length;
            var data = new byte[l / 2];
            for (int i = 0; i < l; i += 2)
            {
                data[i / 2] = (byte)((Convert.ToByte(hex[i].ToString(), 16) << 4) + Convert.ToByte(hex[i + 1].ToString(), 16));
            }
            return data;
        }

        /// <summary>
        /// String output of all objects in the list.
        /// </summary>
        /// <param name="list">List all objects to be printed.</param>
        /// <returns>String output of the list.</returns>
        public static string ShowAll(object[] list)
        {
            var sb = new StringBuilder();
            foreach (var o in list)
            {
                sb.Append("\n").Append(o);
            }
            return sb.ToString();
        }

        /// <summary>
        /// String output of all integers in the list.
        /// </summary>
        /// <param name="list">List of integers to be printed.</param>
        /// <returns>String output of the list.</returns>
        public static string ShowAll(int[] list)
        {
            var sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.Append("\n").Append(i);
            }
            return sb.ToString();
        }

        /// <summary>
        /// String output of all bytes in the list.
        /// </summary>
        /// <param name="list">List of bytes to be printed.</param>
        /// <returns>String output of the list.</returns>
        public static string ShowAll(byte[] list)
        {
            var sb = new StringBuilder();
            foreach (var i in list)
            {
                sb.Append("\n").Append(i);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the current time in string format e.g. "03:34:10".
        /// </summary>
        /// <returns>The current time as a formatted string.</returns>
        public static string GetHourTime()
        {
            var dateTimeFormat = "HH:mm:ss";
            var dateTime = DateTime.Now;
            return dateTime.ToString(dateTimeFormat);
        }

        /// <summary>
        /// Returns the resource file as stream in the resource's folder.
        /// </summary>
        /// <param name="fileName">Name of resource file.</param>
        /// <returns>The resource file as stream.</returns>
        public static Stream ResourceFilePath(string fileName)
        {
            return typeof(IdToAsset).Assembly.GetManifestResourceStream(fileName);
        }

        /// <summary>
        /// Returns the OS version as 'win' or 'mac' as a string. Returns empty if the OS is unsupported.
        /// </summary>
        /// <returns>The name of the current operating system.</returns>
        public static string GetOperatingSystem()
        {
            var os = Environment.OSVersion.Platform.ToString().ToLower();
            if (string.IsNullOrEmpty(os))
            {
                Console.WriteLine("[X] Failed to detect operating system using 'os.name'.");
                return "";
            }
            else if (!os.Contains("win") && !os.Contains("mac"))
            {
                // Unsupported operating system such as most Linux distributions
                return "";
            }
            else
            {
                return os;
            }
        }
    }
}