using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public struct BlinkDataStruct : PacketDataStruct
    {
        public BlinkDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextSingle(ref i, out blinkDuration); // duration
        }
        public float Timestamp;
        public float blinkDuration;
    }
    public class BlinkEventStream : AHEventStream<BlinkDataStruct>
    {
        public BlinkEventStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override udpInfo.EventControl EventControlBit { get { return udpInfo.EventControl.BLINK; } }
        public override udpInfo.GazeEvents EventTypeByte { get { return udpInfo.GazeEvents.BLINK; } }
    }
}
