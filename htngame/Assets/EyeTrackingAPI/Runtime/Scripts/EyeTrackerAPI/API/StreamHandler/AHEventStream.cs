using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdhawkApi
{
    /*
    public struct EventStruct : PacketDataStruct
    {
        //event_type, timestamp, event_packet
        public EventStruct(byte[] data)
        {
            int i = 0;
            byte eventType;
            data.ReadNextInt8(ref i, out eventType);
            EventType = (udpInfo.GazeEvents)eventType;
            if (EventType == udpInfo.GazeEvents.EYE_CLOSED || EventType == udpInfo.GazeEvents.EYE_OPENED)
            {
                eventData = new float[1];
                data.ReadNextSingle(ref i, out Timestamp);
                byte tracker_id;
                data.ReadNextInt8(ref i, out tracker_id);
                eventData[0] = tracker_id; // tracker id
            }
            else if (EventType == udpInfo.GazeEvents.BLINK)
            {
                eventData = new float[1];
                data.ReadNextSingle(ref i, out Timestamp);
                data.ReadNextSingle(ref i, out eventData[0]); // duration
            }
            else if (EventType == udpInfo.GazeEvents.TRACKLOSS_START || EventType == udpInfo.GazeEvents.TRACKLOSS_END)
            {
                data.ReadNextSingle(ref i, out Timestamp);
                eventData = new float[1];
                byte tracker_id;
                data.ReadNextInt8(ref i, out tracker_id);
                eventData[0] = tracker_id;
            }
            else
            {
                data.ReadNextSingle(ref i, out Timestamp);
                eventData = new float[0];
            }
        }
        public udpInfo.GazeEvents EventType;
        public float Timestamp;
        public float[] eventData;
    }
    */

    public class AHEventStream<T> where T : PacketDataStruct
    {
        public AHEventStream(UDPBehaviour udpClient)
        {
            this.udpClient = udpClient;
            udpClient?.RegisterStreamHandler(StreamPacketByte, ProcessPacketData);
        }

        public byte StreamPacketByte { get { return 0x18; } }

        protected List<Action<PacketDataStruct>> dataListeners = new List<Action<PacketDataStruct>>();

        public bool Active = false;

        protected UDPBehaviour udpClient;

        public virtual udpInfo.EventControl EventControlBit { get { throw new NotImplementedException(); } }
        public virtual udpInfo.GazeEvents EventTypeByte { get { throw new NotImplementedException(); } }

        public T data;

        public virtual void ProcessPacketData(byte[] data)
        {
            if (data[0] != (byte)EventTypeByte)
            {
                return;
            }

            byte[] eventData = new byte[data.Length - 1];

            Buffer.BlockCopy(data, 1, eventData, 0, eventData.Length);

            try
            {
                this.data = (T)Activator.CreateInstance(typeof(T), eventData);
            }
            catch (Exception e)
            {
                Debug.LogError("Error: Stream data packet length invalid! Unregistering stream: " + EventControlBit.ToString() + "\n" + e);
                udpClient.UnregisterStreamHandler(StreamPacketByte);
                return;
            }
            Active = true;
            InvokeListeners(this.data);
        }

        // override so that we can set this up custom for the event stream
        public virtual void Start()
        {
            // if udpClient is destroyed then null propogation will return not null still
            // BUT thankfully udpClient is never destroyed. If it is, expect everything to go wrong and errors to be raised.
            udpClient?.ToggleEvent(EventControlBit, true);
        }

        public virtual void Stop()
        {
            udpClient?.ToggleEvent(EventControlBit, false);
            // all event streams are always registered in the packet handler so do not need to be added
        }

        public virtual void AddListener(Action<PacketDataStruct> callback)
        {
            dataListeners.Add(callback);
            // all event streams are always registered in the packet handler so do not need to be removed
        }

        public virtual void RemoveListener(Action<PacketDataStruct> callback)
        {
            dataListeners.Remove(callback);
        }

        protected void InvokeListeners(PacketDataStruct data)
        {
            for (int i = 0; i < dataListeners.Count; i++)
            {
                dataListeners[i](data);
            }
        }

    }
}