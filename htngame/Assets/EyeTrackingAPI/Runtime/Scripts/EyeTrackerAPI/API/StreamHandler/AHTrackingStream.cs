using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdhawkApi
{
    public interface PacketDataStruct {}

    public class AHTrackingStream<T> where T : PacketDataStruct
    {
        public AHTrackingStream(UDPBehaviour udpClient)
        {
            this.udpClient = udpClient;
        }

        public virtual byte StreamPacketByte { get { throw new NotImplementedException(); } }

        protected List<Action<PacketDataStruct>> dataListeners = new List<Action<PacketDataStruct>>();

        public bool Active = false;
        
        protected UDPBehaviour udpClient;

        public virtual udpInfo.StreamControl StreamControlBit { get { throw new NotImplementedException(); } }
        
        public T data;

        public void ProcessPacketData(byte[] data) 
        {
            try
            {
                this.data = (T)Activator.CreateInstance(typeof(T), data);
            }
            catch (Exception e)
            {
                Debug.LogError("Error: Stream data packet length invalid! Unregistering stream: " + StreamControlBit.ToString() + "\n" + e);
                udpClient.UnregisterStreamHandler(StreamPacketByte);
                return;
            }
            Active = true;
            InvokeListeners(this.data);
        }

        // override so that we can set this up custom for the event stream
        public virtual void Start(StreamRate rate = StreamRate.Default)
        {
            // if udpClient is destroyed then null propogation will return not null still
            // BUT thankfully udpClient is never destroyed. If it is, expect everything to go wrong and errors to be raised.
            Debug.Log("Starting stream: " + StreamPacketByte);
            udpClient?.ToggleStream(StreamControlBit, true, rate);
            udpClient?.RegisterStreamHandler(StreamPacketByte, ProcessPacketData);
        }

        public virtual void Stop()
        {
            udpClient?.ToggleStream(StreamControlBit, false);
            udpClient?.UnregisterStreamHandler(StreamPacketByte);
            Active = false;
        }

        public virtual void AddListener(Action<PacketDataStruct> callback)
        {
            dataListeners.Add(callback);
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
