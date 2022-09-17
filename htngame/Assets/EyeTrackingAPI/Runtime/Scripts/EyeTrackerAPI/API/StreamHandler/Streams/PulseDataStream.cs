using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct PulseDataStruct : PacketDataStruct
    {
        public PulseDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextInt8(ref i, out TrackerId);
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector2(ref i, out Position);
            data.ReadNextSingle(ref i, out PulseWidth);
            data.ReadNextInt8(ref i, out PhotodiodeId);
        }
        public byte TrackerId;
        public float Timestamp;
        public Vector2 Position;
        public float PulseWidth;
        public byte PhotodiodeId;
    }
    public class PulseDataStream : AHTrackingStream<PulseDataStruct>
    {
        public PulseDataStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override byte StreamPacketByte { get { return 0x11; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.PULSE; } }
    }
}
