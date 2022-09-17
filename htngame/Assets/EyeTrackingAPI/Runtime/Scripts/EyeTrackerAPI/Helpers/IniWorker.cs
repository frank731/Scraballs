// Code adapted from Unity forums
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

namespace AdhawkApi.FileIO{

    public static class INIWorker
    {
        private static string path = Path.Combine(Application.dataPath, "../config.ini");

        private static Dictionary<string, Dictionary<string, string>> IniDictionary = new Dictionary<string, Dictionary<string, string>>();
        private static bool Initialized = false;
        /// <summary>
        /// Sections list
        /// </summary>
        public enum Sections
        {
            info,
            settings
        }
        /// <summary>
        /// Keys list
        /// </summary>
        public enum Keys
        {
            screenDistance,
            screenWidth,
            screenHeight,
            targetHorizontalPitch,
            targetVerticalPitch,
            screenDPI,
            deviceName,

            angleSpreadHorizontal,
            andleSpreadVertical,
            angle_offset_horizontal,
            angle_offset_vertical,
            distance_m,
            frequency,
            step_time,
            recording_duration,
            dot_scale,
            saccadeTestDuration,
            saccade_test_delay_s,
            log_mode
        }

        private static bool FirstRead()
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    string theSection = "";
                    string theKey = "";
                    string theValue = "";

                    while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                    {
                        line.Trim();
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }
                        if (line.StartsWith("[") && line.EndsWith("]"))
                        {
                            theSection = line.Substring(1, line.Length - 2);
                        }
                        else if (!line.StartsWith(";"))
                        {
                            string[] ln = line.Split(new char[] { '=' });
                            theKey = ln[0].Trim();
                            theValue = ln[1].Trim();
                        }
                        if (theSection == "" || theKey == "" || theValue == "")
                            continue;
                        PopulateIni(theSection, theKey, theValue);
                        Debug.Log("Read key: " + theSection + ", " + theKey + ", " + theValue);
                        theKey = "";
                        theValue = "";
                    }
                }
            } else
            {
                Debug.LogError("Unable to find config.ini.");
            }
            return true;
        }

        private static void PopulateIni(string _Section, string _Key, string _Value)
        {
            if (IniDictionary.ContainsKey(_Section))
            {
                if (IniDictionary[_Section].ContainsKey(_Key))
                    IniDictionary[_Section][_Key] = _Value;
                else
                    IniDictionary[_Section].Add(_Key, _Value);
            }
            else
            {
                Dictionary<string, string> neuVal = new Dictionary<string, string>();
                neuVal.Add(_Key.ToString(), _Value);
                IniDictionary.Add(_Section.ToString(), neuVal);
            }
        }
        /// <summary>
        /// Write data to INI file. Section and Key no in enum.
        /// </summary>
        /// <param name="_Section"></param>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void IniWriteValue(string _Section, string _Key, string _Value)
        {
            if (!Initialized)
                FirstRead();
            PopulateIni(_Section, _Key, _Value);
            //write ini
            WriteIni();
        }
        /// <summary>
        /// Write data to INI file. Section and Key bound by enum
        /// </summary>
        /// <param name="_Section"></param>
        /// <param name="_Key"></param>
        /// <param name="_Value"></param>
        public static void IniWriteValue(Sections _Section, Keys _Key, string _Value)
        {
            IniWriteValue(_Section.ToString(), _Key.ToString(), _Value);
        }

        private static void WriteIni()
        {
            Dictionary<string, string> commentsMap = new Dictionary<string, string>();
            if (File.Exists(path))
            {
                string contents = "";
                using (StreamReader sr = new StreamReader(path))
                {
                    contents = sr.ReadToEnd();
                }
                string[] fileContents = contents.Split('\n');
                string curComments = "";
                for (int i = 0; i < fileContents.Length; i++)
                {
                    if (fileContents[i].StartsWith("["))
                    {
                        continue;
                    }
                    else if (fileContents[i].StartsWith("#"))
                    {
                        if (curComments == "")
                        {
                            curComments = curComments + fileContents[i].Trim();
                        } else
                        {
                            curComments = curComments + "\n" + fileContents[i].Trim();
                        }
                    } else if (fileContents[i].Split('=').Length == 2)
                    {
                        commentsMap.Add(fileContents[i].Split('=')[0].Trim(), curComments);
                        curComments = "";
                    }
                }
            }
            else
            {
                Debug.LogError("Unable to find config.ini.");
            }


            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> section in IniDictionary)
                {
                    sw.WriteLine("[" + section.Key.ToString() + "]");
                    foreach (KeyValuePair<string, string> entry in section.Value)
                    {
                        // check for newline in the value if it's a string
                        string value = entry.Value.ToString();
                        if (value.Contains(Environment.NewLine) || value.Contains("\r\n"))
                        {
                            Debug.LogError("WARNING: ATTEMPTING TO WRITE NEWLINE VALUE TO .INI: UNSUPPORTED");
                        }
                        value = value.Replace(Environment.NewLine, " ");
                        value = value.Replace("\r\n", " ");

                        string key = entry.Key.ToString();

                        if (commentsMap.ContainsKey(key))
                        {
                            sw.Write(commentsMap[key] + "\n");
                            sw.WriteLine(entry.Key.ToString() + "=" + value);
                        }
                        else
                        {
                            sw.WriteLine(entry.Key.ToString() + "=" + value);
                        }
                    }
                }
            }
        }
        public static float IniReadFloat(Sections _Section, Keys _Key, float def)
        {
            try
            {
                return float.Parse(IniReadValue(_Section, _Key, def.ToString()));
            }
            catch (ArgumentNullException)
            {
                return def;
            }
            catch (FormatException)
            {
                Debug.LogError("Error: Unable to read float " + _Section.ToString() + "." + _Key.ToString() + " from config.ini");
                return def;
            }
        }
        /// <summary>
        /// Read data from INI file. Section and Key bound by enum
        /// </summary>
        /// <param name="_Section"></param>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static string IniReadValue(Sections _Section, Keys _Key, string def)
        {
            if (!Initialized)
                FirstRead();
            return IniReadValue(_Section.ToString(), _Key.ToString(), def);
        }
        /// <summary>
        /// Read data from INI file. Section and Key no in enum.
        /// </summary>
        /// <param name="_Section"></param>
        /// <param name="_Key"></param>
        /// <returns></returns>
        public static string IniReadValue(string _Section, string _Key, string def)
        {
            if (!Initialized)
                FirstRead();
            if (IniDictionary.ContainsKey(_Section))
                if (IniDictionary[_Section].ContainsKey(_Key))
                    return IniDictionary[_Section][_Key];
            Debug.LogError("Error: Unable to read value " + _Section.ToString() + "." + _Key.ToString() + " from config.ini");
            return def;
        }
    }
}