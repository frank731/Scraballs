using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public class EyeClosedEventStream : AHEventStream<TrackerStateChangeDataStruct>
    {
        public EyeClosedEventStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override udpInfo.EventControl EventControlBit { get { return udpInfo.EventControl.EYE_CLOSE_OPEN; } }
        public override udpInfo.GazeEvents EventTypeByte { get { return udpInfo.GazeEvents.EYE_CLOSED; } }
    }

}
