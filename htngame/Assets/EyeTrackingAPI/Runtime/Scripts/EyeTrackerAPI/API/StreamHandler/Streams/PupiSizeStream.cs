using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{

    public struct PupilSizeDataStruct : PacketDataStruct
    {
        public PupilSizeDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextSingle(ref i, out PupilDiameterLeft);
            data.ReadNextSingle(ref i, out PupilDiameterRight);
        }
        public float Timestamp;
        public float PupilDiameterLeft;
        public float PupilDiameterRight;
    }

    public class PupilSizeStream : AHTrackingStream<PupilSizeDataStruct>
    {
        public PupilSizeStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x05; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.PUPIL_DIAMETER; } }
    }
}
