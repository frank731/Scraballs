using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct PerEyeGazeDataStruct : PacketDataStruct
    {
        //payload: float timestamp, float x, float y, float z, float vergence
        public PerEyeGazeDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector3(ref i, out PositionRight);
            data.ReadNextVector3(ref i, out PositionLeft);
            PositionRight.z = -PositionRight.z;
            PositionLeft.z = -PositionLeft.z;
        }
        public float Timestamp;
        public Vector3 PositionLeft;
        public Vector3 PositionRight;
    }
    public class PerEyeGazeStream : AHTrackingStream<PerEyeGazeDataStruct>
    {
        public PerEyeGazeStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override byte StreamPacketByte { get { return 0x06; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.PER_EYE_GAZE; } }
    }
}
