// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.


// monobehaviour aspect of the EyeTrackerAPI singleton
// this file is extended among several files prepended with etapi_

using UnityEngine;
using UnityEngine.Events;

// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{

    /// <summary>
    /// Convenience tool for interfacing unity with backend and managing the udp communication tool
    /// </summary>
    public partial class EyeTrackerAPI : MonoBehaviour
    {
        private const string P_PREFS_SESSION_TAGS = "AHSessionTags";
        private const string P_PREFS_LOGMODE = "Logmode";
        private const string P_PREFS_SESSION_NAME = "AHSessionTagsProfileName";

        [Tooltip("Represents the gaze data source. Default is Backend")]
        [SerializeField] private EtSourceType etSource = EtSourceType.Backend;

        [SerializeField] private udpInfo.LogMode AdHawkAnalyticsLogMode = udpInfo.LogMode.BASIC;

        // [Tooltip("Will save a log of all udp commands sent to backend (as well as acks recieved)")]
        [HideInInspector] private bool backendLogging = false;

        [SerializeField] public UnityEvent OnConnectEvent = new UnityEvent();

        [HideInInspector] public string SessionProfileName = "Unity";

        [HideInInspector] public string SessionTags = "";

        // [SerializeField] public udpInfo.LogMode SessionLogMode = udpInfo.LogMode.BASIC;

        [HideInInspector] public udpInfo.LogMode SessionLogMode = udpInfo.LogMode.BASIC;

        private void Update()
        {
            if (SimulatedWithMouse)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DidBlinkLastFrame = true;
                    LastBlinkDuration = 0.1f;
                    GazeBeforeBlink = GazeVector;
                }
            }
            UpdateGazeOrigin();
        }

        private void LateUpdate()
        {
            if (oldBlink != DidBlinkLastFrame)
            {
                oldBlink = DidBlinkLastFrame;
            } else
            {
                if (oldBlink)
                {
                    DidBlinkLastFrame = false;
                }
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }

            udpClient = GetComponent<UDPBehaviour>(); // find or create UDP behaviour.
            if (udpClient == null)
            {
                udpClient = gameObject.AddComponent<UDPBehaviour>(); // just create one on start rather than assuming one already exists.
            }
            if (backendLogging)
            {
                udpClient.LoggingEnabled = true;
            }

            // read session tags from playerprefs:
            SessionProfileName = PlayerPrefs.GetString(P_PREFS_SESSION_NAME, SessionProfileName);
            SessionTags = PlayerPrefs.GetString(P_PREFS_SESSION_TAGS, SessionTags);
            SessionLogMode = (udpInfo.LogMode)PlayerPrefs.GetInt(P_PREFS_LOGMODE, (byte)SessionLogMode);

            udpClient.RegisterOnConnect(() => { udpClient.RestartSession(SessionLogMode, SessionProfileName, SessionTags); });
            udpClient.RegisterOnConnect(OnConnectEvent.Invoke);

            Streams = new AHStreamHandler(udpClient);
            Events = new AHEventHandler(udpClient);

            udpClient.RegisterOnConnect(() => {
                Streams.Gaze.Start();
                Events.Blink.Start();
                Streams.TrackerReady.Start();
                SetLogMode(AdHawkAnalyticsLogMode);
            });


        }

        private void Start()
        {
            GlintPositions = new Vector2[MAX_PD_PER_EYE * 2];
            PupilPositions = new Vector2[2]; // assume two for now

            ScreenDPI = Screen.dpi;
            ScreenDPI = PlayerPrefs.GetFloat("user_dpi", ScreenDPI);

            Events.Blink.AddListener((blinkData) => HandleBlink((BlinkDataStruct)blinkData));
            Streams.GlintXY.AddListener((data) => HandleGlint((GlintDataStruct)data));
            Streams.PupilXY.AddListener((data) => HandlePupilXY((PupilXYStruct)data));
            Streams.PupilXY.AddListener((data) => HandleFuse((FuseDataStruct)data));
            Streams.TrackerReady.AddListener((data) => HandleTrackerReady((TrackerReadyStruct)data));
        }

        private void OnDestroy()
        {
            OnErrorMessage = null;
        }

        /// <summary>
        /// Mainly resets gaze origin to camera position. In the future this will be the the Player object position
        /// </summary>
        private void UpdateGazeOrigin()
        {
            if (Player.Instance == null)
            {
                GazeOrigin = MainCam.transform.position;
            } else
            {
                GazeOrigin = Player.Instance.EyeCenter.position;
            }
            
        }

    }
}