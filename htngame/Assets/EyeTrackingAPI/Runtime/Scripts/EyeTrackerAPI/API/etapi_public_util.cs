// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using System.Collections;
using UnityEngine;
using AdhawkApi.Numerics.Filters;

// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{
    public partial class EyeTrackerAPI
    {

        /// <summary>
        /// Get the latest valid calibrated eyetracker data in eye angles (yaw, pitch)
        /// </summary>
        /// <param name="eyeIndex">The index of the eye to get data for</param>
        /// <param name="filterType">The type of filter to run the data through</param>
        /// <returns>The eye angles, or Vector2.positiveInfinity on error</returns>
        public Vector2 GetEyeAngles(int eyeIndex = -1, FilterType filterType = FilterType.MovingAverage)
        {
            return Numerics.Gaze.Vector2Angles(GazeVector);
        }
        /// <summary>
        /// Gets the gaze position on the main camera.
        /// </summary>
        public Vector3 GetGazePositionOnCamera()
        {
            return Camera.main.WorldToScreenPoint(
                Camera.main.transform.position + (Camera.main.transform.rotation * (EyeTrackerAPI.Instance.GazeVector.normalized * 10))
            );
        }
        /// <summary>
        /// Gets the gaze position on a camera.
        /// </summary>
        public Vector3 GetGazePositionOnCamera(Camera cam)
        {
            return cam.WorldToScreenPoint(
                cam.transform.position + (cam.transform.rotation * (EyeTrackerAPI.Instance.GazeVector.normalized * 10))
            );
        }

        public IEnumerator RequestWorldCameraType(UDPRequestCallback callback)
        {
            yield return udpClient.SendUDPRequest(new UDPRequest(
                    udpInfo.GET_SYSTEM_INFO,
                    callback,
                    new byte[] { (byte)udpInfo.SystemInfo.CAMERA_ID }
                ));
        }
        
        private static float Csc(float x)
        {
            return 1 / Mathf.Sin(x);
        }

        /// <summary>
        /// Pass an error message to the error handler.
        /// </summary>
        /// <param name="message"></param>
        public void CallError(string message)
        {
            if (OnErrorMessage != null)
            {
                OnErrorMessage(message);
            }
        }

    }
}