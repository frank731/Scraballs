using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdhawkApi;

public class ControllerInput : MonoBehaviour
{
    ControllerInputHandler cInput;

    public void CalibrateEyetracking()
    {
        EyeTrackerAPI.Instance.RunCalibrationProcedure();
    }

    private void Start()
    {
        cInput = ControllerInputHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!EyeTrackerAPI.Instance.Calibrating)
        {
            if (cInput.RightCon.ThumbstickButton.wasButtonPressedLastFrame || cInput.LeftCon.ThumbstickButton.wasButtonPressedLastFrame)
            {
                CalibrateEyetracking();
            }
        }
        else
        {
            if (cInput.LeftCon.Trigger.wasButtonPressedLastFrame)
            {
                EyeTrackerAPI.Instance.CalibratorMoveNextPoint();
            }
        }
    }
}
