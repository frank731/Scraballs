using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public interface DeviceInterface
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Target GetTarget();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        float GetDeviceFOV();

        /// <summary>
        /// Run device calibration procedure
        /// </summary>
        /// <returns> Coroutine handle that another coroutine can yield to and wait for. </returns>
        Coroutine RunCalibration();

        /// <summary>
        /// Run device validation procedure
        /// </summary>
        /// <returns></returns>
        Coroutine RunValidation();

        /// <summary>
        /// Stop currently running calibration
        /// </summary>
        void StopCurrentSequence();

        /// <summary>
        /// Jump to the next calibration point if there is a calibration running.
        /// </summary>
        void RequestNextCalibrationPoint();

        /// <summary>
        /// Creates autotune request
        /// </summary>
        /// <returns></returns>
        Coroutine RequestAutotune();

        /// <summary>
        /// Run device recenter procedure
        /// </summary>
        /// <returns> Coroutine handle that another coroutine can yield to and wait for. </returns>
        Coroutine RunRecenter();

        /// <summary>
        /// Each device handles gaze data a little differently. This function provides
        /// access to it.
        /// </summary>
        /// <param name="gazeData">Resulting processed gaze data</param>
        /// <returns></returns>
        Vector3 ProcessGazeVector(GazeDataStruct gazeData);
    }
}