using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct GazeInScreenStruct : PacketDataStruct
    {
        //payload: float timestamp, float x, float y, float z, float vergence
        public GazeInScreenStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector2(ref i, out PositionBoard);
        }
        public float Timestamp;
        public Vector2 PositionBoard;
    }

    public class GazeInScreenStream : AHTrackingStream<GazeInScreenStruct>
    {
        public GazeInScreenStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x08; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.GAZE_IN_SCREEN; } }
    }
}
