using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct PupilPositionStruct : PacketDataStruct
    {
        public PupilPositionStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector3(ref i, out PositionLeft);
            data.ReadNextVector3(ref i, out PositionRight);
        }
        public float Timestamp;
        public Vector3 PositionLeft;
        public Vector3 PositionRight;
    }
    public class PupilPositionStream : AHTrackingStream<PupilPositionStruct>
    {
        public PupilPositionStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x04; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.PUPIL; } }
    }
}
