using System;
using UnityEngine;

namespace AdhawkApi
{
    public partial class UDPBehaviour
    {
        
        // Update backend with the updated request of stream data.
        // deprecated?
        private Coroutine UpdateStreamControl(StreamRate streamRate = StreamRate.Default)
        {
            byte[] result = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes((uint)streamControlMap), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((float)streamRate), 0, result, 4, 4);
            // don't need ack for this, though we may want to consider setting something up in the future in case something goes wrong.
            return SetProperty(udpInfo.PropertyType.STREAM_CONTROL, result);
        }

        public Coroutine StartBlinkStream()
        {
            byte[] result = new byte[5];
            result[4] = 1;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)udpInfo.EventControl.BLINK), 0, result, 0, 4);
            return SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public Coroutine StopBlinkStream()
        {
            byte[] result = new byte[5];
            result[4] = 0;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)udpInfo.EventControl.BLINK), 0, result, 0, 4);
            return SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public Coroutine StartTrackLossEventStream()
        {
            byte[] result = new byte[5];
            result[4] = 1;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)udpInfo.EventControl.TRACKLOSS_START_END), 0, result, 0, 4);
            return SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public Coroutine StopTrackLossEventStream()
        {
            byte[] result = new byte[5];
            result[4] = 0;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)udpInfo.EventControl.TRACKLOSS_START_END), 0, result, 0, 4);
            return SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public void StartEyeOpenCloseStream()
        {
            byte[] result = new byte[5];
            result[4] = 1;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)udpInfo.EventControl.EYE_CLOSE_OPEN), 0, result, 0, 4);
            SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public void StopEyeOpenCloseStream()
        {
            byte[] result = new byte[5];
            result[4] = 0;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt32)udpInfo.EventControl.EYE_CLOSE_OPEN), 0, result, 0, 4);
            SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public Coroutine SetStreamRate(StreamRate targetRate)
        {
            return UpdateStreamControl(targetRate);
        }
    }
}
