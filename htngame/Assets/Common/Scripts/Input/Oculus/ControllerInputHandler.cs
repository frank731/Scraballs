using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class ControllerInputHandler : MonoBehaviour
{
    // singleton for ez controller access:
    public static ControllerInputHandler Instance { get { return _instance; } }
    private static ControllerInputHandler _instance = null;

    public VRConRight RightCon = new VRConRight();
    public VRConLeft LeftCon = new VRConLeft();

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
    }

    private void Update()
    {
        RightCon.UpdateInputs();
        LeftCon.UpdateInputs();
    }

    public bool AnyPressed()
    {
        return RightCon.any() || LeftCon.any();
    }

    public class VRConRight
    {
        public OculusButton A                   { get; private set; }
        public OculusButton B                   { get; private set; }
        public OculusButton Trigger             { get; private set; }
        public OculusButton Grip                { get; private set; }
        // public OculusButton Menu             { get; private set; }
        public OculusButton ThumbstickButton    { get; private set; }

        public VRConRight()
        {
            InputDeviceCharacteristics rightCon = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right;
            A = new OculusButton(rightCon, OculusButton.ButtonOption.primaryButton);
            B = new OculusButton(rightCon, OculusButton.ButtonOption.secondaryButton);
            Trigger = new OculusButton(rightCon, OculusButton.ButtonOption.triggerButton);
            Grip = new OculusButton(rightCon, OculusButton.ButtonOption.gripButton);
            // Menu = new OculusButton(rightCon, OculusButton.ButtonOption.menuButton); // right menu button is reserved
            ThumbstickButton = new OculusButton(rightCon, OculusButton.ButtonOption.primary2DAxisClick);
        }

        public void UpdateInputs()
        {
            A.UpdateCheckButtonPressed();
            B.UpdateCheckButtonPressed();
            Trigger.UpdateCheckButtonPressed();
            Grip.UpdateCheckButtonPressed();
            // Menu.UpdateCheckButtonPressed(); // right menu button is reserved
            ThumbstickButton.UpdateCheckButtonPressed();
        }

        public bool any()
        {
            return  A.isButtonDown ||
                    B.isButtonDown ||
                    Trigger.isButtonDown ||
                    Grip.isButtonDown ||
                    ThumbstickButton.isButtonDown;
        }
    }
    public class VRConLeft
    {
        public OculusButton X { get; private set; }
        public OculusButton Y { get; private set; }
        public OculusButton Trigger { get; private set; }
        public OculusButton Grip { get; private set; }
        public OculusButton Menu { get; private set; }
        public OculusButton ThumbstickButton { get; private set; }

        public VRConLeft()
        {
            InputDeviceCharacteristics leftCon = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left;
            X = new OculusButton(leftCon, OculusButton.ButtonOption.primaryButton);
            Y = new OculusButton(leftCon, OculusButton.ButtonOption.secondaryButton);
            Trigger = new OculusButton(leftCon, OculusButton.ButtonOption.triggerButton);
            Grip = new OculusButton(leftCon, OculusButton.ButtonOption.gripButton);
            Menu = new OculusButton(leftCon, OculusButton.ButtonOption.menuButton);
            ThumbstickButton = new OculusButton(leftCon, OculusButton.ButtonOption.primary2DAxisClick);
        }

        public void UpdateInputs()
        {
            X.UpdateCheckButtonPressed();
            Y.UpdateCheckButtonPressed();
            Trigger.UpdateCheckButtonPressed();
            Grip.UpdateCheckButtonPressed();
            Menu.UpdateCheckButtonPressed();
            ThumbstickButton.UpdateCheckButtonPressed();
        }

        public bool any()
        {
            return X.isButtonDown ||
                    Y.isButtonDown ||
                    Trigger.isButtonDown ||
                    Grip.isButtonDown ||
                    Menu.isButtonDown ||
                    ThumbstickButton.isButtonDown;
        }
    }
}