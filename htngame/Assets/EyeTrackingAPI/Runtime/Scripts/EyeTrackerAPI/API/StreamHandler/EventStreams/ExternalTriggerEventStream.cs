using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public struct ExternalTriggerEventDataStruct : PacketDataStruct
    {
        // timestamp, trigger_id
        public ExternalTriggerEventDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextInt8(ref i, out trigger_id); // duration
        }
        public float Timestamp;
        public byte trigger_id;
    }
    public class ExternalTriggerEventStream : AHEventStream<ExternalTriggerEventDataStruct>
    {
        public ExternalTriggerEventStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override udpInfo.EventControl EventControlBit { get { return udpInfo.EventControl.EXTERNAL_TRIGGER; } }
        public override udpInfo.GazeEvents EventTypeByte { get { return udpInfo.GazeEvents.EXTERNAL_TRIGGER; } }
    }
}
