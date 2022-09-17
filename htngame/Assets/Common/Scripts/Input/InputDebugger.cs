using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDebugger : MonoBehaviour
{
    ControllerInputHandler cInput;
    // Start is called before the first frame update
    void Start()
    {
        cInput = ControllerInputHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (cInput.RightCon.A.wasButtonPressedLastFrame) Debug.Log("Right A");
        if (cInput.RightCon.B.wasButtonPressedLastFrame) Debug.Log("Right B");
        if (cInput.RightCon.Trigger.wasButtonPressedLastFrame) Debug.Log("Right Trigger");
        if (cInput.RightCon.Grip.wasButtonPressedLastFrame) Debug.Log("Right Grip");
        if (cInput.RightCon.ThumbstickButton.wasButtonPressedLastFrame) Debug.Log("Right ThumbstickButton");

        if (cInput.LeftCon.X.wasButtonPressedLastFrame) Debug.Log("Left X");
        if (cInput.LeftCon.Y.wasButtonPressedLastFrame) Debug.Log("Left Y");
        if (cInput.LeftCon.Trigger.wasButtonPressedLastFrame) Debug.Log("Left Trigger");
        if (cInput.LeftCon.Grip.wasButtonPressedLastFrame) Debug.Log("Left Grip");
        if (cInput.LeftCon.Menu.wasButtonPressedLastFrame) Debug.Log("Left Menu");
        if (cInput.LeftCon.ThumbstickButton.wasButtonPressedLastFrame) Debug.Log("Left ThumbstickButton");

    }
}
/*
X
Y
Trigger
Grip
Menu
ThumbstickButton
*/