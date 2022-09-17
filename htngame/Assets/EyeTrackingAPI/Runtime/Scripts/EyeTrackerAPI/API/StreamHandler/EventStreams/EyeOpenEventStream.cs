using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public struct TrackerStateChangeDataStruct : PacketDataStruct
    {
        public TrackerStateChangeDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextInt8(ref i, out TrackerID);
        }
        public float Timestamp;
        public byte TrackerID;
    }
    public class EyeOpenEventStream : AHEventStream<TrackerStateChangeDataStruct>
    {
        public EyeOpenEventStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override udpInfo.EventControl EventControlBit { get { return udpInfo.EventControl.EYE_CLOSE_OPEN; } }
        public override udpInfo.GazeEvents EventTypeByte { get { return udpInfo.GazeEvents.EYE_OPENED; } }
    }
}
