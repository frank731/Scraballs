using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AdhawkApi
{
    /// <summary>
    /// Device handler template - Adds MonoBehaviour to DeviceInterface
    /// </summary>
    public abstract class EyeTrackingDevice : MonoBehaviour, DeviceInterface
    {
        public float vergenceCalPointDistance = 0.5f;

        /// <summary>
        /// Focus target for when we need the player to look at a specific point in space
        /// </summary>
        [SerializeField] protected Target target;

        [SerializeField] protected float DeviceFOV = 80;

        /// <summary>
        /// Get Calibration target used by this device.
        /// This may be something like an aruco marker, or a 3D target, or a 2D
        /// canvas image.
        /// </summary>
        /// <returns>Target object used for this device</returns>
        public virtual Target GetTarget()
        {
            return target;
        }

        /// <summary>
        /// Get device field of view
        /// </summary>
        /// <returns>General FOV of this device through the games viewport</returns>
        public virtual float GetDeviceFOV()
        {
            return DeviceFOV;
        }

        public virtual Coroutine RunVergenceCalibration()
        {
            return null;
        }

        /// <summary>
        /// Run device calibration procedure
        /// </summary>
        /// <returns> Coroutine handle that another coroutine can yield to and wait for. </returns>
        public abstract Coroutine RunCalibration();

        /// <summary>
        /// Run device validation procedure
        /// </summary>
        /// <returns> Coroutine handle that another coroutine can yield to and wait for. </returns>
        public abstract Coroutine RunValidation();

        /// <summary>
        /// Stop currently running calibration
        /// </summary>
        public abstract void StopCurrentSequence();

        /// <summary>
        /// Jump to the next calibration point if there is a calibration running.
        /// </summary>
        public abstract void RequestNextCalibrationPoint();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Coroutine RequestAutotune();

        /// <summary>
        /// Run device recenter procedure
        /// </summary>
        /// <returns> Coroutine handle that another coroutine can yield to and wait for. </returns>
        public abstract Coroutine RunRecenter();

        /// <summary>
        /// Each device handles gaze data a little differently. This function provides
        /// access to it.
        /// </summary>
        /// <param name="gazeData">Resulting processed gaze data</param>
        /// <returns></returns>
        public abstract Vector3 ProcessGazeVector(GazeDataStruct gazeData);

    }
}