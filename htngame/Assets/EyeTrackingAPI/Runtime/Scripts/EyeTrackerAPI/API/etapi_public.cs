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
        /// Access the AdHawk eyetracking and other tracking data streams
        /// </summary>
        public static AHStreamHandler Streams { get; private set; }

        /// <summary>
        /// Access event streams such as Blink
        /// </summary>
        public static AHEventHandler Events { get; private set; }

        /// <summary>
        /// Whether or not we've run a calibration on backend
        /// </summary>
        public bool Calibrated { get; private set; }
        /// <summary>
        ///  Local gaze rotation relative to head rotation 
        /// </summary>
        public Quaternion GazeRotation
        {
            get
            {
                return Quaternion.LookRotation(GazeVector);
            }
        }
        /// <summary> 
        /// Gaze direction as a vector 
        /// </summary>
        public Vector3 GazeVector
        {
            get
            {
                if (Streams.Gaze != null && DeviceHandler != null && !SimulatedWithMouse)
                {
                    return DeviceHandler.ProcessGazeVector(Streams.Gaze.data);
                }
                else if (Instance.SimulatedWithMouse)
                {
                    return Instance.MouseAsGaze();
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        
        private Camera cam2d;
        private Vector3 MouseAsGaze()
        {
            if (cam2d == null)
            {
                var cam2dgo = GameObject.Find("2DCamera");
                if (cam2dgo == null){
                    cam2d = Camera.main;
                } else
                {
                    cam2d = cam2dgo.GetComponent<Camera>();
                }
            }
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.0f;
            return Quaternion.Inverse(cam2d.transform.rotation) * (cam2d.ScreenToWorldPoint(mousePos) - cam2d.transform.position);
        }

        [HideInInspector] public Vector3 GazeBeforeBlink = Vector3.zero;

        /// <summary> 
        /// Origin of gaze vector - in most cases this is simply the main camera's position.
        /// </summary>
        public Vector3 GazeOrigin { get; private set; }
        /// <summary>
        /// Eyetracking vergence
        /// </summary>
        public float Vergence { get; private set; }
        /// <summary>
        /// Calculated gaze depth from vergence.
        /// </summary>
        public float GazeDepth { get; private set; } = 1.0f;
        public float RawGazeDepth { get; private set; } = 1.0f;
        /// <summary>
        /// Indicates whether this app has requested an autotune and recieved a successful result.
        /// This is not actually an indication of whether backend is tuned, but is instead a useful
        /// tool to find out if RequestAutotune() was ran and recieved information that the autotune worked.
        /// </summary>
        public bool ConfirmedAutotune { get; private set; } = false;
        /// <summary>
        /// Whether or not an autotune is happening
        /// If true, frontend is waiting for autotune result from backend.
        /// </summary>
        public bool RunningAutotune { get; private set; } = false;
        /// <summary>
        /// Is EyeTrackerAPI currently calibrating?
        /// </summary>
        public bool Calibrating { get; private set; } = false;
        /// <summary>
        /// Current tracking status.
        /// </summary>
        public TrackingState CurrentTrackingState { get; private set; }
        private bool[] trackingState = new bool[] { true, true };
        /// <summary>
        /// Returns true if the user completed a blink since the last frame
        /// </summary>
        public bool DidBlinkLastFrame { get; private set; } = false;
        private bool oldBlink;
        /// <summary>
        /// Duration of the last blink event
        /// </summary>
        public float LastBlinkDuration { get; private set; } = 0;
        /// <summary>
        /// Timestamp of the last blink event
        /// </summary>
        public float LastBlinkTimestamp { get; private set; } = 0;

        public bool LeftEyeClosed { get; private set; } = false;
        public bool RightEyeClosed { get; private set; } = false;

        /// <summary>
        /// Pre-calibration information about whether eyes are visible to the scanner modules.
        /// </summary>
        public TrackingState UncalibratedTrackingState { get; private set; }
        /// <summary>
        /// Current average velocity of the total detected glints
        /// </summary>
        public float GlintMoveDeltasAverage { get; private set; } = 0;
        /// <summary>
        /// Essentially the number of active photodiodes currently detecting glints within a certain threshold.
        /// </summary>
        public int NumGlintsDetected { get; private set; } = 0;

        /// <summary>
        /// Access latest glint positions
        /// </summary>
        public Vector2[] GlintPositions { get; private set; } = new Vector2[12];
        /// <summary>
        /// fuse data stream (basically the center of the glint offsets for each eye)
        /// 0x13 Fuse Data
        /// payload: uint8_t trackerid, float timestamp, float x, float y, uint8_t partial
        /// </summary>
        public Vector2[] FusePositions { get; private set; } = new Vector2[2];
        /// <summary>
        /// Pupil XY position as a glint object.
        /// </summary>
        public Vector2[] PupilPositions { get; private set; } = new Vector2[2];
        /// <summary>
        /// Device gyro rotational acceleration information if it is available
        /// </summary>
        public Quaternion DeviceIMURotation { get; private set; } = Quaternion.identity;
        /// <summary>
        /// Device gyro motion acceleration information if it is available
        /// </summary>
        public Vector3 DeviceIMUMotion { get; private set; } = Vector3.zero;
        /// <summary>
        /// Actual rate of eyetracking stream
        /// </summary>
        public float PupilDiameterLeft { get; private set; }
        public float PupilDiameterRight { get; private set; }
        public float GazeStreamRate { get; private set; } = 0;
        /// <summary>
        /// Returns true if EyeTracerAPI is using mouse as input
        /// </summary>
#if UNITY_EDITOR
        public bool SimulatedWithMouse { get { return etSource == EtSourceType.SimulatedWithMouse; } }
#else
        public bool SimulatedWithMouse { get { return false; } }
#endif
        /// <summary>
        /// Grants access to the device interface being used by EyeTrackerAPI.
        /// </summary>
        public DeviceInterface DeviceHandler { get; private set; } = null;

        [HideInInspector] public float ScreenDPI;

        /// <summary>
        /// Circular buffer of gaze data reaching into the past - mostly for the purpose of tracking gaze before a blink occurs
        /// </summary>
        public CircularArray<Vector4> OldGazeData = new CircularArray<Vector4>(512);

        [HideInInspector] public bool autoSkipInvalidCalibrationPoints = false;

        /// <summary>
        /// Whether or not the device server is running and connected
        /// </summary>
        public bool BackendConnected
        {
            get
            {
                if (udpClient != null)
                {
                    return udpClient.ServerConnected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sets the gaze rate to the provided enum setting
        /// </summary>
        /// <param name="targetRate"></param>
        /// <returns></returns>
        public Coroutine SetStreamRate(StreamRate targetRate)
        {
            return udpClient.SetStreamRate(targetRate);
        }

        public delegate void GazeVectorStreamHandler(Vector3 gazeVec, float Timestamp, float OffsetTime);

        public static GazeVectorStreamHandler GazeVectorStreamDelegate;
    }
}