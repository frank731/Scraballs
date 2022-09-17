using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct GazeDataStruct : PacketDataStruct
    {
        //payload: float timestamp, float x, float y, float z, float vergence
        public GazeDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector3(ref i, out Position);
            Position.z = -Position.z;
            data.ReadNextSingle(ref i, out Vergence);
        }
        public float Timestamp;
        public Vector3 Position;
        public float Vergence;
    }
    public class GazeStream : AHTrackingStream<GazeDataStruct>
    {
        public GazeStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override byte StreamPacketByte { get { return 0x03; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.TIMESTAMPED_GAZE; } }
    }
}
