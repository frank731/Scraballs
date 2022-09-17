using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TextLog
{

    private string log_file_path = "../Log/";
    private string filename_format = "dd-MM-yyyy-HH-mm-ss";
    private string log_file_name = "-log.txt";
    private const int MAX_LOG_LENGTH = 9999999; //10 megabytes-ish

    private bool timeStamped = false;

    private StringBuilder logStringBuilder = new StringBuilder();

    public bool LoggingEnabled = false;

    public TextLog(string log_file_name = "-log.txt", string dateFormat = "dd-MM-yyyy-HH-mm-ss", string logDir = "../Log/", bool timeStamped = false)
    {
        this.log_file_name = log_file_name + ".txt";
        this.filename_format = dateFormat;
        this.log_file_path = logDir;
        this.timeStamped = timeStamped;
    }

    public void SaveLog()
    {
        if (logStringBuilder.Length >= MAX_LOG_LENGTH)
        {
            Debug.LogWarning("WARNING: log file greater than 10mb.");
        }

        Directory.CreateDirectory(Path.Combine(Application.dataPath, log_file_path));
        //write out our stringbuilder
        using (StreamWriter file = new StreamWriter(
            Path.Combine(
                Application.dataPath,
                log_file_path,
                string.Concat(DateTime.Now.ToString(filename_format), log_file_name)
        ))){
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

    private string CurTimeStamp()
    {
        return string.Format("[{0:D2}:{1:D2}:{2:D2}] ", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }

    public void AddLine(string text)
    {
        logStringBuilder.AppendLine();
        if (timeStamped)
        {
            logStringBuilder.Append(CurTimeStamp());
            logStringBuilder.Append(": ");
        }
        logStringBuilder.Append(text);
        CheckLogLength();
    }

    public void AddCSVLine(params string[] text)
    {
        logStringBuilder.AppendLine();
        if (timeStamped)
        {
            logStringBuilder.Append(CurTimeStamp());
            logStringBuilder.Append(",");
        }
        for (int i = 0; i < text.Length; i++)
        {
            logStringBuilder.Append(text[i]);
            if (i < text.Length-1) logStringBuilder.Append(",");
        }
        CheckLogLength();
    }

    public void Clear()
    {
        logStringBuilder.Clear();
    }
}
