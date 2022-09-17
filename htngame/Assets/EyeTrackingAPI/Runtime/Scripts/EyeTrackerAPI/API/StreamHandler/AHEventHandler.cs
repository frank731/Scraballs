using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AdhawkApi
{
    public class AHEventHandler
    {
        public AHEventHandler(UDPBehaviour udpClient)
        {
            Blink = new BlinkEventStream(udpClient);
            EyeClosed = new EyeClosedEventStream(udpClient);
            EyeOpen = new EyeOpenEventStream(udpClient);
            TracklossStart = new TracklossStartEventStream(udpClient);
            TracklossEnd = new TracklossEndEventStream(udpClient);
            ExternalTrigger = new ExternalTriggerEventStream(udpClient);
        }
        public BlinkEventStream Blink;
        public EyeClosedEventStream EyeClosed;
        public EyeOpenEventStream EyeOpen;
        public TracklossStartEventStream TracklossStart;
        public TracklossEndEventStream TracklossEnd;
        public ExternalTriggerEventStream ExternalTrigger;
    }
}