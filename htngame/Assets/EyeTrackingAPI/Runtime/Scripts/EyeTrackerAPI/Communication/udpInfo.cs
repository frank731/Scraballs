// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
namespace AdhawkApi {
    public static class udpInfo
    {
        public const int PORT_CONTROL_DATA = 11032;

        //these are self-explanatory and need no summary.
        public const byte ACK_SUCCESS = 0x00;
        public const byte ACK_ERROR = 0x01;
        public const byte ERROR_INVALID_CONTROL_MESSAGE = 0x02;
        public const byte ERROR_TRACKERS_NOT_READY = 0x03;
        public const byte ERROR_LOST_TRACKING = 0x04;
        public const byte ERROR_RIGHT_EYE_NOT_FOUND = 0x05;
        public const byte ERROR_LEFT_EYE_NOT_FOUND = 0x06;
        public const byte ERROR_SYSTEM_NOT_CALIBRATED = 0x07;
        public const byte NOT_SUPPORTED = 0x08;
        public const byte SESSION_ALREADY_RUNNING = 0x09;
        public const byte NO_CURRENT_SESSION = 0x10;
        public const byte REQUEST_TIMEOUT = 0x11;

        private static readonly Dictionary<byte, string> AckLookup = new Dictionary<byte, string>{
            {0x00, "Success"},
            {0x01, "Internal failure"},
            {0x02, "Invalid argument"},
            {0x03, "Trackers not ready"},
            {0x04, "No eyes detected"},
            {0x05, "Right eye not detected"},
            {0x06, "Left eye not detected"},
            {0x07, "System not calibrated"},
            {0x08, "Not supported" },
            {0x09, "Data logging session already exists" },
            {0x10, "No data logging session exists to stop"},
            {0x11, "Request has timed out"},
            {0x12, "Unexpected response"},
            {0x13, "Hardware faulted"},
            {0x14, "Camera initialization failed"},
            {0x15, "System is busy"},
            {0x16, "Communication error"},
            {0x17, "Device calibration is outdated"},
            {0x18, "Process was aborted or interrupted unexpectedly"},
        };

        public static string GetAckPacketTypeInfo(byte packetType)
        {
            if (AckLookup.TryGetValue(packetType, out string result))
            {
                return result;
            }
            else
            {
                return "";
            }
        }

        ///<summary> Host -> Client, Register Endpoint. </summary>
        public const byte REGISTER_ENDPOINT = 0xc0;

        public const byte UNREGISTER_ENDPOINT = 0xc2;

        public const byte READ_CONFIGS = 0xc3;

        public const byte WRITE_CONFIGS = 0xc4;

        public const byte PING = 0xc5;

        ///<summary> Client -> Host, start listening for calibration points </summary>
        public const byte START_CALIBRATION = 0x81;

        ///<summary> Client -> Host, stop listening for calibration points and calculate ET gaze data </summary>
        public const byte STOP_CALIBRATION = 0x82;

        ///<summary> Client -> Host, stop listening for calibration points and do not calculate anything, remove all currently registered points. </summary>
        public const byte ABORT_CALIBRATION = 0x83;

        ///<summary> <c>Client -> Host</c>, add current calibration point to list. </summary>
        public const byte REGISTER_CALIBRATION_POINT = 0x84;

        ///<summary> Client -> Host, Start running autotune </summary>
        public const byte START_RANGING = 0x85;

        ///<summary> Client -> Host, Start Validation procedure </summary>
        public const byte START_VALIDATION = 0x86;

        ///<summary> Client -> Host, Stop Validation procedure, send recorded data to analytics database</summary>
        public const byte STOP_VALIDATION = 0x87;

        ///<summary> Client -> Host, Register a validation point. </summary>
        public const byte REGISTER_VALIDATION_POINT = 0x88;

        ///<summary> Client -> Host, Essentially tell backend that we want to seperate the next bit of logging data. </summary>
        public const byte LOG_ANNOTATION = 0x89; // deprecated

        /// <summary> Payload: 32-length bitmask </summary>
        public const byte CONTROL_DATA_STREAMS = 0x8a; // deprecated

        ///<summary>
        /// Payload: 4 16-bit integers
        /// {tracker 1 x mean, tracker 1 y mean, tracker 2 x mean, tracker 2 y mean}
        /// </summary>
        public const byte READ_AUTOTUNE_POSITION = 0x8b; // deprecated

        /// <summary> Payload: float frequency of data (1-1000 Hz) </summary>
        public const byte SET_EYETRACKING_OUTPUT_RATE = 0x8e; // deprecated

        ///<summary>
        /// Client -> Host, tell backend that we're doing a re-center. backend should assume that use is looking at Vector3.forward relative to camera
        /// Payload: Vector x, y, z (12 bytes) position of the recenter dot.
        /// </summary>
        public const byte START_RECENTER_PROCEDURE = 0x8f;

        ///<summary> Find out the state of tracking </summary>
        public const byte GET_TRACKER_STATE = 0x90;

        /* from publicapi.py:
         * 
        PROCEDURE_START = 0xb0
        PROCEDURE_STATUS = 0xb1

        CAMERA_USER_SETTINGS_SET = 0xd0
        START_CAMERA = 0xd2
        STOP_CAMERA = 0xd3
        START_VIDEO_STREAM = 0xd4
        STOP_VIDEO_STREAM = 0xd5
        REGISTER_SCREEN_BOARD = 0xd6
        START_SCREEN_TRACKING = 0xd7
        STOP_SCREEN_TRACKING = 0xd8
        */

        public const byte CAMERA_USER_SETTINGS_SET = 0xd0;

        public const byte CAMERA_START = 0xd2;
        public const byte CAMERA_STOP = 0xd3;

        public const byte START_VIDEO_STREAM = 0xd4;
        public const byte STOP_VIDEO_STREAM = 0xd5;

        public const byte REGISTER_SCREEN_BOARD = 0xd6;
        public const byte START_SCREEN_TRACKING = 0xd7;
        public const byte STOP_SCREEN_TRACKING = 0xd8;


        public const byte CALIBRATION_GUI_START = 0xd6;
        public const byte VALIDATION_GUI_START = 0xd7;
        public const byte AUTOTUNE_GUI_START = 0xd9;
        public const byte QUICK_START_GUI_START = 0xd8;


        ///<summary> Payload: Property type, property value </summary>
        public const byte BLOB_SIZE = 0x92;

        public const byte BLOB_DATA = 0x93;

        public const byte LOAD_BLOB = 0x94;

        public const byte SAVE_BLOB = 0x95;

        public const byte START_LOG_SESSION = 0x96;

        public const byte STOP_LOG_SESSION = 0x97;

        public const byte GET_SYSTEM_INFO = 0x99;

        public const byte PROPERTY_GET = 0x9a;

        public const byte PROPERTY_SET = 0x9b;

        public const byte SYSTEM_CONTROL = 0x9c;

        public const byte PROCEDURE_START = 0xb0;

        public const byte PROCEDURE_STATUS = 0xb1;

        public enum SystemInfo : byte
        {
            CAMERA_ID = 0x01,
            DEVICE_SERIAL = 0x02,
            FIRMWARE_API_VERSION = 0x03,
            FIRMWARE_VERSION = 0x04,
            EYE_MASK = 0x05,
            PRODUCT_ID = 0x06,
            MULTI_INFO = 0x07
        }

        public enum CameraType : byte
        {
            NotAvailable = 0x00,
            SMI = 0x01,
            SMI_INV = 0x02,
            Quanta = 0x03,
            Quanta_inv = 0x04,
            Quanta2 = 0x05,
            Quanta2_INV = 0x06
        }

        public enum BlobVersion : byte
        {
            MULTIGLINT = 0x01,
            CALIBRATION = 0x03
        }

        public enum PropertyType : byte
        {
            AUTOTUNE_POSITION = 0x01,
            STREAM_CONTROL = 0x02,
            IPD = 0x03,
            COMPONENT_OFFSETS = 0x04,
            EVENT_CONTROL = 0x05,
            NORMALIZED_EYE_OFFSETS = 0x09
        }

        public enum GazeEvents : byte
        {
            // indicating a confirmed combined blink event
            BLINK = 0x01,
            // indicating the per-eye eye close event
            EYE_CLOSED = 0x02,
            // indicating the per-eye eye open event
            EYE_OPENED = 0x03,
            // indicating the trackloss start event
            TRACKLOSS_START = 0x04,
            // indicating the trackloss end event
            TRACKLOSS_END = 0x05,
            // indicating the confirmed combined saccade
            SACCADE = 0x06,
            // indicating the per-eye saccade onset/start
            SACCADE_START = 0x07,
            // indicating the per-eye saccade offset/end
            SACCADE_END = 0x08,
            // information about a validation point
            VALIDATION_SAMPLE = 0x09,
            // information about overall validation quality
            VALIDATION_SUMMARY = 0x10,
            // indicating that a procedure has been started
            PROCEDURE_STARTED = 0x11,
            // indicating that a procedure has ended and information about the the final results
            PROCEDURE_ENDED = 0x12,
            // indicating the MCU external gpio trigger event
            EXTERNAL_TRIGGER = 0x13,
        }

        public enum LogMode : byte
        {
            NONE = 0x01,
            BASIC = 0x02,
            FULL = 0x03
        }

        public static readonly Dictionary<byte, string> CommandTypePacketLookup = new Dictionary<byte, string>{
            {0xc0,"REGISTER_ENDPOINT"},
            {0x81,"START_CALIBRATION"},
            {0x82,"STOP_CALIBRATION"},
            {0x83,"ABORT_CALIBRATION"},
            {0x84,"REGISTER_CALIBRATION_POINT"},
            {0x85,"START_RANGING"},
            {0x86,"START_VALIDATION"},
            {0x87,"STOP_VALIDATION"},
            {0x88,"REGISTER_VALIDATION_POINT"},
            {0x89,"LOG_ANNOTATION"},
            {0x8a,"CONTROL_DATA_STREAMS"},
            {0x8b,"READ_AUTOTUNE_POSITION"},
            {0x8e,"SET_EYETRACKING_OUTPUT_RATE"},
            {0x8f,"START_RECENTER_PROCEDURE"},
            {0x90,"GET_TRACKER_STATE"},
            {0x92,"BLOB_SIZE"},
            {0x93,"BLOB_DATA"},
            {0x94,"LOAD_BLOB"},
            {0x95,"SAVE_BLOB"},
            {0x96,"START_LOG_SESSION"},
            {0x97,"STOP_LOG_SESSION"},
            {0x99,"REQUEST_SYSTEM_INFO"},
            {0x9a,"GET_PROPERTY"},
            {0x9b,"SET_PROPERTY"},
            {0x9c,"SYSTEM_CONTROL"},
            {0xb0,"PROCEDURE_START"},
            {0xb1,"PROCEDURE_STATUS"},
        };

        //==========================================

        // data stream controls:


        public enum EventControl : uint
        {
            BLINK                   = (uint)1 << 0,
            EYE_CLOSE_OPEN          = (uint)1 << 1,
            TRACKLOSS_START_END     = (uint)1 << 2,
            SACCADE                 = (uint)1 << 3,
            SACCADE_START_END       = (uint)1 << 4,
            VALIDATION_RESULTS      = (uint)1 << 5,
            PRODECURE_START_END     = (uint)1 << 6,
            EXTERNAL_TRIGGER        = (uint)1 << 7,
        }

        ///<summary> pay-load data for control stream toggles </summary>
        public enum StreamControl : uint {
            GAZE                = (uint)1 << 0,
            PUPIL               = (uint)1 << 1,
            PUPIL_DIAMETER      = (uint)1 << 2,
            TIMESTAMPED_GAZE    = (uint)1 << 3,
            PER_EYE_GAZE        = (uint)1 << 4,
            GAZE_IMAGE          = (uint)1 << 5,
            GAZE_IN_SCREEN      = (uint)1 << 6,

            IMU_ROTATION        = (uint)1 << 23,
            PUPIL_CENTER        = (uint)1 << 24,
            EMBEDDED_INFO       = (uint)1 << 25,
            CALIBRATION_ERROR   = (uint)1 << 26,
            PULSE               = (uint)1 << 27,
            GLINT               = (uint)1 << 28,
            FUSE                = (uint)1 << 29,
            PUPIL_ELLIPSE       = (uint)1 << 30,
            IMU                 = (uint)1 << 31,
        }
        
        // for logging purposes
        private static readonly Dictionary<byte, string> StreamPacketTypeLookup = new Dictionary<byte, string>{
            {0x01,"TRACKER_READY"},
            {0x02,"TRACKER_READY"},
            {0x03,"GAZE_POINT_TIMESTAMP"},
            {0x04,"PUPIL_DIAMETER"},
            {0x11,"PULSE_DATA"},
            {0x12,"GLINT_DATA"},
            {0x13,"FUSE_DATA"},
        };

        public interface INumber<T>
        {
            T ConvertGeneric<T1>(T1 item);
        }

    }
}