// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.


// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{
    public partial class EyeTrackerAPI
    {
        private byte[] EMPTY_BYTE = new byte[0];
        private const float SMOOTH_GLINT_TIME_SPEED = 5.0f;
        private const int MAX_PD_PER_EYE = 6;
        private const int fuse_stream_buffer_size = 30;
        private const int ID_LEFT = 0;
        private const int ID_RIGHT = 1;
        private const float MAX_GLINT_TIMEOUT = 0.25f;
        private const float MAX_TIME_RESET_GLINT = 0.1f;
        private const float DPCM_TO_DPI = 2.54f;
        private const float DPI_TO_DPCM = 1.0f / 2.54f;
        private const float TRACKING_LOST_THRESH = 0.05f;

        private static EyeTrackerAPI _instance;
        /// <summary> 
        /// Eye Tracker API public static instance - there should only ever be one of these. 
        /// </summary>
        public static EyeTrackerAPI Instance { get { return _instance; } }

    }
}