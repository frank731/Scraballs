using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct PupilEllipseStruct : PacketDataStruct
    {
        public PupilEllipseStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextInt8(ref i, out TrackerId);
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector2(ref i, out Center);
            data.ReadNextVector2(ref i, out PrincipleAxis);
            data.ReadNextSingle(ref i, out PrincipleAngle);
        }
        public byte TrackerId; 
        public float Timestamp;
        public Vector2 Center;
        public Vector2 PrincipleAxis;
        public float PrincipleAngle;
    }
    public class PupilEllipseStream : AHTrackingStream<PupilEllipseStruct>
    {
        public PupilEllipseStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x14; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.PUPIL_ELLIPSE; } }
    }
}
