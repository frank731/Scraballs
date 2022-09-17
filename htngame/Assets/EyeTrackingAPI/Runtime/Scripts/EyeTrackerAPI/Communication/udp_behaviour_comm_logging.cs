using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AdhawkApi
{
    public partial class UDPBehaviour
    {
        public Coroutine RestartSession(udpInfo.LogMode logMode = udpInfo.LogMode.FULL, string name = "", string tags = "")
        {
            return StartCoroutine(RestartSessionCoroutine(logMode, name, tags));
        }

        private IEnumerator RestartSessionCoroutine(udpInfo.LogMode logMode = udpInfo.LogMode.FULL, string name = "", string tags = "")
        {
            bool toContinue = false;
            yield return StopLogging(callback:
                (byte[] data, UDPRequestStatus status) => { toContinue = true; });
            yield return new WaitUntil(() => toContinue);

            StartLogging(logMode, name, tags);
        }

        public Coroutine StartLogging(udpInfo.LogMode logMode = udpInfo.LogMode.FULL, string name = "", string tags = "", UDPRequestCallback callback = null)
        {
            // so, tags should be formatted type:tag?
            IEnumerable<byte> payload = new byte[] { (byte)logMode };
            if (name != "")
            {
                payload = payload.Concat(Encoding.UTF8.GetBytes("name:" + name));
            }
            if (tags != "")
            {
                string[] tag_array = tags.Split(',');
                for (int i = 0; i < tag_array.Length; i++)
                {
                    tag_array[i] = tag_array[i].Trim();
                }
                string tags_trimmed = string.Join(",", tag_array);
                payload = payload.Concat(Encoding.UTF8.GetBytes("," + tags_trimmed));

                Debug.Log("Sending start log request: Logmode: " + logMode.ToString() + "name:" + name + "," + tags_trimmed);
            } else
            {
                Debug.Log("Sending start log request: Logmode: " + logMode.ToString() + "name:" + name);
            }

            return SendUDPRequest(new UDPRequest(
                udpInfo.START_LOG_SESSION,
                callback,
                payload
            ));
        }

        public Coroutine StopLogging(UDPRequestCallback callback = null)
        {
            Debug.Log("Stop logging request sent!");
            return SendUDPRequest(new UDPRequest(
                udpInfo.STOP_LOG_SESSION,
                callback,
                new byte[] { }
            ));
        }
    }
}
