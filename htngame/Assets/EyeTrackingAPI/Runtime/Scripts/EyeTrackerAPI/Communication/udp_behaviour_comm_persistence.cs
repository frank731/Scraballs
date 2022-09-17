// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using System;
using UnityEngine;

namespace AdhawkApi
{
    public partial class UDPBehaviour
    {
        //private static UInt32 streamControlMap;
        private float backendSearchCheckFrequency = 1.4f;
        private float backendSearchLastCheckTime = 1.4f;
        private int backendChecksDone = 0;
        private int successfulBackendChecks = 0;
        // handles looking for backend so long as we're not in contact with it.
        private void LookForBackend()
        {
            if (BackendState == ConnectionState.unknown || BackendState == ConnectionState.disconnected)
            {
                if (Time.time - backendSearchLastCheckTime > backendSearchCheckFrequency)
                {
                    backendSearchLastCheckTime = Time.time;
                    PingAndSetEndPoint();
                }
            }
            else if (BackendState == ConnectionState.connected)
            {
                if (Time.time - backendSearchLastCheckTime > backendSearchCheckFrequency)
                {
                    if (backendChecksDone > successfulBackendChecks + 3)
                    { // check state of last check...
                        //at least one whole iteration has gone by since backend has responded.
                        if (BackendState == ConnectionState.connected)
                        {
                            onDisconnect.Invoke();
                        }
                        BackendState = ConnectionState.disconnected;
                        Debug.Log("Lost connection to backend.");
                    }
                    backendSearchLastCheckTime = Time.time;
                    PingAndSetEndPoint();
                }
            }
            else
            {
                backendChecksDone = 0;
                successfulBackendChecks = 0;
            }
        }

        /// <summary>
        /// Tell backend where we want it to send stream data.
        /// </summary>
        public void PingAndSetEndPoint()
        {
            if (BackendState == ConnectionState.connected)
            {
                backendChecksDone += 1;
                SendUDPRequest(new UDPRequest(
                        udpInfo.PING,
                        EndPointRequestCallback,
                        new byte[0]
                    ),
                    0.85f // has to be lower than the check frequency.
                );
            }
            else
            {
                SendUDPRequest(new UDPRequest(
                        udpInfo.REGISTER_ENDPOINT,
                        EndPointRequestCallback,
                        BitConverter.GetBytes(streamHandler.LocalPort)
                    ),
                    0.85f // has to be lower than the check frequency.
                );
            }
        }

        private void EndPointRequestCallback(byte[] data, UDPRequestStatus status)
        {
            if (status != UDPRequestStatus.Timeout)
            {
                if (BackendState == ConnectionState.disconnected || BackendState == ConnectionState.unknown)
                { // old backend state was not connected, time to update
                    Debug.Log("Found backend.");
                    // StartEyeTracking(); // automatically start eyetracking in this case
                    // EyeTrackerAPI.Streams.Gaze.Start(); // now handled in the device manager
                    StartBlinkStream();
                    StartTrackLossEventStream();
                    StartEyeOpenCloseStream();
                }
                HandleEndPointSetAck();
            }
        }

        private void HandleEndPointSetAck()
        {
            if (BackendState == ConnectionState.disconnected || BackendState == ConnectionState.unknown)
            {
                if (onConnect != null)
                {
                    onConnect.Invoke();
                }
            }
            BackendState = ConnectionState.connected;
            // this is so we can compare the number of successful backend checks to the number of times we've asked backend for a result:
            successfulBackendChecks += 1;
        }
    }
}
