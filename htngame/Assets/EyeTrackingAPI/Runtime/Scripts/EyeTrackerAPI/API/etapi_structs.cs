// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using UnityEngine;

// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{
    public partial class EyeTrackerAPI
    {
        /// <summary>
        /// Describes Various sources for eyetracking or eyetracking simulation
        /// </summary>
        public enum EtSourceType
        {
            //EmbeddedEt,     // ET device handles tracking
            //ABVTUnityApi,   // Eyetracking is handled on the device (maybe consider another external package for this?)
            Backend,        // Eyetracking is handled on backend
            SimulatedWithMouse,
        }

        /// <summary>
        /// Tracking state of the eyetracker
        /// </summary>
        public enum TrackingState
        {
            /// <summary>
            /// Tracking is lost
            /// </summary>
            TrackingLost = 0,
            /// <summary>
            /// Left eye is tracking
            /// </summary>
            TrackingLeft = 1,
            /// <summary>
            /// Right eye is tracking
            /// </summary>
            TrackingRight = 2,
            /// <summary>
            /// Both eyes are tracking
            /// </summary>
            TrackingBoth = 3,
            /// <summary>
            /// Reserved for future use
            /// </summary>
            TrackingUnknown = 4
        }

        /// <summary>
        /// Describes various errors to do with udp communication
        /// </summary>
        public enum SystemError
        {
            /// <summary>
            /// Device port closed, likely due to device disconnection.
            /// </summary>
            PortClosed,
            /// <summary>
            /// Data corruption was detected when communicating with the device.
            /// </summary>
            DataCorruption,
            /// <summary>
            /// Error occurred while configuring the device.
            /// </summary>
            ConfigError,
            /// <summary>
            /// validation error
            /// </summary>
            ValidationError,
        };

        /// <summary>
        /// Simple structure that encapsulates a float with Mathf.Smoothdamp function built in.
        /// </summary>
        private struct SmoothFloat
        {
            public float value;
            public float smoothTime;
            private float speedref;
            public SmoothFloat(float smoothTime)
            {
                value = 0;
                this.smoothTime = smoothTime;
                speedref = 0;
            }
            public void Calculate(float target)
            {
                value = Mathf.SmoothDamp(value, target, ref speedref, smoothTime);
            }
        }
        
        /// <summary>
        /// Error handler for eyetracker system errors
        /// </summary>
        /// <param name="error">Error type being thrown</param>
        public delegate void SystemErrorHandler(SystemError error);
    }
}