using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct TrackerReadyStruct : PacketDataStruct
    {
        //status,
        public TrackerReadyStruct(byte[] data)
        {
        }
    }
    public class TrackerReadyStream : AHTrackingStream<TrackerReadyStruct>
    {
        public TrackerReadyStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x02; } }

        public override udpInfo.StreamControl StreamControlBit { get { return 0; } }

        public override void Start(StreamRate rate = StreamRate.Default)
        {
            Debug.Log("Starting stream: " + StreamPacketByte);
            udpClient?.RegisterStreamHandler(StreamPacketByte, ProcessPacketData);
        }
    }
}
