using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct GazeImageStruct : PacketDataStruct
    {
        //payload: float timestamp, float x, float y, float z, float vergence
        public GazeImageStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector2(ref i, out PosInImage);
            data.ReadNextVector2(ref i, out DegreeToPixels);
        }
        public float Timestamp;
        public Vector2 PosInImage;
        public Vector2 DegreeToPixels;
    }
    public class GazeImageStream : AHTrackingStream<GazeImageStruct>
    {
        public GazeImageStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override byte StreamPacketByte { get { return 0x07; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.GAZE_IMAGE; } }
    }
}
