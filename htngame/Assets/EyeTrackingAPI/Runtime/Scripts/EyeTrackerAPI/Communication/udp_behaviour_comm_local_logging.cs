using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace AdhawkApi
{
    public partial class UDPBehaviour
    {
        private const string log_file_path = "../Log/";
        private const string filename_format = "dd-MM-yyyy-HH-mm-ss";
        private const string log_file_name = "-log.txt";

        private const string packet_sent_string = "Sent >> ";
        private const string packet_rec_string = "Ackn << ";

        private const string packet_hex_format = "X2";

        private const int MAX_LOG_LENGTH = 9999999; //10 megabytes-ish

        /// <summary>
        /// enable or disable logging packets to file
        /// </summary>
        public bool LoggingEnabled = false;
        
        private StringBuilder logStringBuilder = new StringBuilder();

        private void SaveLog()
        {
            if (logStringBuilder.Length >= MAX_LOG_LENGTH)
            {
                Debug.LogError("WARNING: log file greater than 100mb.");
            }

            Directory.CreateDirectory(Path.Combine(Application.dataPath, log_file_path));
            //write out our stringbuilder
            using (StreamWriter file = new StreamWriter(
                Path.Combine(
                    Application.dataPath,
                    log_file_path,
                    string.Concat(DateTime.Now.ToString(filename_format), log_file_name)
            )))
            {
                file.WriteLine(logStringBuilder.ToString());
            }
        }

        private void CheckLogLength()
        {
            if (logStringBuilder.Length > MAX_LOG_LENGTH)
            {
                Debug.Log("Max Log file size reached: " + logStringBuilder.Length.ToString() + ", saving log and clearing log buffer.");
                SaveLog();
                logStringBuilder.Clear();
            }
        }

        private void AddLog(string data)
        {
            if (!LoggingEnabled) return;
            logStringBuilder.AppendLine();
            logStringBuilder.Append(CurTimeStamp());
            logStringBuilder.Append(data);
            CheckLogLength();
        }

        private string CurTimeStamp()
        {
            return string.Format("[{0:D2}:{1:D2}:{2:D2}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }

        private void LogSentPacket(byte command_type, IEnumerable<byte> data = null)
        {
            if (!LoggingEnabled) return;
            string packetInfo = PacketInfo(command_type);
            logStringBuilder.AppendLine();
            logStringBuilder.Append(string.Concat(CurTimeStamp(), packet_sent_string, packetInfo));
            if (data != null)
            {
                if (data.Count() > 0)
                {
                    logStringBuilder.Append(", data: ");
                    if (packetInfo == "84 (REGISTER_CALIBRATION_POINT)")
                    {
                        logStringBuilder.Append("Vector3: ");
                        logStringBuilder.Append(data.ToArray().ToVector3(0).ToString("F4"));
                    }
                    else
                    {
                        foreach (byte b in data)
                        {
                            logStringBuilder.Append(b.ToString(packet_hex_format));
                            logStringBuilder.Append(" ");
                        }
                    }
                }
            }
            CheckLogLength();
        }

        private void LogRecievePacket(byte command_type, IEnumerable<byte> data = null)
        {
            if (!LoggingEnabled) return;
            logStringBuilder.AppendLine();
            logStringBuilder.Append(string.Concat(CurTimeStamp(), packet_rec_string, PacketInfo(command_type)));
            if (data != null)
            {
                if (data.Count() > 0)
                {
                    logStringBuilder.Append(", data: ");
                    foreach (byte b in data)
                    {
                        logStringBuilder.Append(b.ToString(packet_hex_format));
                        logStringBuilder.Append(" ");
                    }
                }
            }
            CheckLogLength();
        }

        private void LogTimeout(byte packetType)
        {
            if (!LoggingEnabled) return;
            logStringBuilder.AppendLine();
            logStringBuilder.Append(CurTimeStamp());
            logStringBuilder.Append("TIMEOUT ");
            logStringBuilder.Append(PacketInfo(packetType));
            CheckLogLength();
        }
    }
}
