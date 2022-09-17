using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdhawkApi;

public class KeyboardInput : MonoBehaviour
{
    // requires an active event system - probably wont ever be used in VR since VR doesn't usually have a user using a keyboard.
    // public static bool IsEditingInputField =>
    //     (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.TryGetComponent(out InputField _) ?? false) ||
    //     (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.TryGetComponent(out TMPro.TMP_InputField _) ?? false);

    public void CalibrateEyetracking()
    {
        EyeTrackerAPI.Instance.RunCalibrationProcedure();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            CalibrateEyetracking();
        }

        /*
        try
        {
            if (!IsEditingInputField)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    CalibrateEyetracking();
                }
            }
        }
        catch (MissingReferenceException e)
        {
            // in case some old tracked component is deleted where the user may have been editting an input field or something.
        }
        */

    }
}
 