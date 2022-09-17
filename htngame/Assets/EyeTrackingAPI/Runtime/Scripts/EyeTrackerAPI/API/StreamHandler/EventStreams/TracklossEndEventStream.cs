using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public class TracklossEndEventStream : AHEventStream<TrackerStateChangeDataStruct>
    {
        public TracklossEndEventStream(UDPBehaviour udpClient) : base(udpClient) { }

        public override udpInfo.EventControl EventControlBit { get { return udpInfo.EventControl.TRACKLOSS_START_END; } }
        public override udpInfo.GazeEvents EventTypeByte { get { return udpInfo.GazeEvents.TRACKLOSS_END; } }
    }
}
