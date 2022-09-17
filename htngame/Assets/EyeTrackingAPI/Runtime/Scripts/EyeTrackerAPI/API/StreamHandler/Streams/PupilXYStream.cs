using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct PupilXYStruct : PacketDataStruct
    {
        public PupilXYStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextInt8(ref i, out TrackerId);
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector2(ref i, out Position);
        }
        public byte TrackerId;
        public float Timestamp;
        public Vector2 Position;
    }
    public class PupilXYStream : AHTrackingStream<PupilXYStruct>
    {
        public PupilXYStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x15; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.PUPIL_CENTER; } }
    }
}
