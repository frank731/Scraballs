// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using UnityEngine;

// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{
    public partial class EyeTrackerAPI
    {

        public void RegisterDeviceInterface(DeviceInterface device)
        {
            DeviceHandler = device;
        }


        /// <summary>
        /// During built-in calibration, this lets us register a point and move to the next
        /// via a different means than the built-in calibration
        /// </summary>
        public void CalibratorMoveNextPoint()
        {
            if (DeviceHandler != null)
                DeviceHandler.RequestNextCalibrationPoint();
        }

        /// <summary>
        /// Cancel the current calibration
        /// </summary>
        public void StopCancelCalibration()
        {
            DeviceHandler.StopCurrentSequence();
            Calibrating = false;
        }

        /// <summary>
        /// Cancel the current validation procedure
        /// </summary>
        public void StopCancelValidation()
        {
            DeviceHandler.StopCurrentSequence();
            Calibrating = false;
        }

        public Coroutine RunCalibrationProcedure()
        {
            Debug.Log("Starting calibration routine");
            if (DeviceHandler == null) return null;
            return DeviceHandler.RunCalibration();
        }

        public Coroutine RunValidationProcedure()
        {
            Debug.Log("Starting validation routine");
            if (DeviceHandler == null) return null;
            return DeviceHandler.RunValidation();
        }

    }
}