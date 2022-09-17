// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using System.Collections;
using UnityEngine;

// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{
    public partial class EyeTrackerAPI
    {
        private UDPBehaviour udpClient;

        public delegate void ErrorMessageHandler(string message);
        private ErrorMessageHandler OnErrorMessage;

        private SystemErrorHandler ErrorCallback;

        private double lastTrackerReadySignalTime = 0;

        private Camera _maincam;
        private Camera MainCam
        {
            get
            {
                if (_maincam == null)
                {
                    _maincam = Camera.main;
                }
                return _maincam;
            }
        }
        
        private CircularArray<FuseDataStruct>[] fuseStreams;
        private BitArray activeGlints = new BitArray(12, false);


        private void HandleGlint(GlintDataStruct glintData)
        {

            Debug.Log("WOAH THERE GOT GLINT DATa");
            int tid = glintData.TrackerId;
            int pid = glintData.PhotodiodeId;

            int index = (tid * 6) + pid;

            GlintPositions[index] = glintData.Position;
        }

        private void HandleFuse(FuseDataStruct fuseData)
        {
            FusePositions[fuseData.TrackerId] = fuseData.Position;
        }

        private void HandlePupilXY(PupilXYStruct pupilxyData)
        {
            PupilPositions[pupilxyData.TrackerId] = pupilxyData.Position;
        }

        private void HandleTrackerReady(TrackerReadyStruct data)
        {
            Debug.Log("Tracker ready recieved");
            lastTrackerReadySignalTime = Time.time;
        }

        private void HandleBlink(BlinkDataStruct blinkData)
        {

            LastBlinkDuration = blinkData.blinkDuration;
            LastBlinkTimestamp = blinkData.Timestamp;

            float timeBeforeBlink = (LastBlinkTimestamp - LastBlinkDuration) - 0.15f;

            Vector4 gazeBeforeBlink = OldGazeData[-1];
            float OldGazeDataTimeMin = OldGazeData[-1].w;
            float OldGazeDataTimeMax = OldGazeData[0].w;

            if (timeBeforeBlink < OldGazeData[0].w)
            {
                GazeBeforeBlink = OldGazeData[0];
            }
            else
            {
                float timePer = (OldGazeDataTimeMin - OldGazeDataTimeMax) / (OldGazeData.Length - 1);
                int checkPos = Mathf.Clamp(Mathf.RoundToInt(OldGazeData.Length - (LastBlinkDuration / timePer)), 0, OldGazeData.Length - 1);
                int count = 0;
                while (checkPos > 1 && checkPos < OldGazeData.Length - 2)
                {
                    count += 1;
                    gazeBeforeBlink = OldGazeData[checkPos];
                    if (timeBeforeBlink < gazeBeforeBlink.w)
                    {
                        if (Mathf.Abs(OldGazeData[checkPos + 1].w - timeBeforeBlink) < Mathf.Abs(gazeBeforeBlink.w - timeBeforeBlink))
                        { // check before
                            checkPos += 1;
                            continue;
                        }
                        else if (Mathf.Abs(OldGazeData[checkPos - 1].w - timeBeforeBlink) < Mathf.Abs(gazeBeforeBlink.w - timeBeforeBlink))
                        { // check after
                            checkPos -= 1;
                            continue;
                        }
                    }
                    break;
                }
                // Debug.Log("Gaze point index before blink is " + (OldGazeData.Length - checkPos).ToString() + " points old." + count);
                GazeBeforeBlink = OldGazeData[checkPos];
            }
            if (LastBlinkDuration > 0.075f)
            {
                DidBlinkLastFrame = true;
            }
        }

    }
}
