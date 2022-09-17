using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct FuseDataStruct : PacketDataStruct
    {
        public FuseDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextInt8(ref i, out TrackerId);
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector2(ref i, out Position);
            data.ReadNextInt8(ref i, out Partial);
        }
        public byte TrackerId;
        public float Timestamp;
        public Vector2 Position;
        public byte Partial;
    }
    public class FuseDataStream : AHTrackingStream<FuseDataStruct>
    {
        public FuseDataStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x13; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.FUSE; } }
    }
}
