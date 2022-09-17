﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class OculusButton
{

    public static implicit operator bool(OculusButton value)
    {
        // assuming, that 1 is true;
        // somehow this method should deal with value == null case
        return value != null && value.isButtonDown;
    }

    static readonly Dictionary<string, InputFeatureUsage<bool>> availableButtons = new Dictionary<string, InputFeatureUsage<bool>>
        {
            {"triggerButton", CommonUsages.triggerButton },
            {"primary2DAxisClick", CommonUsages.primary2DAxisClick },
            {"primary2DAxisTouch", CommonUsages.primary2DAxisTouch },
            {"menuButton", CommonUsages.menuButton },
            {"gripButton", CommonUsages.gripButton },
            {"primaryButton", CommonUsages.primaryButton },
            {"secondaryButton", CommonUsages.secondaryButton },
        };

    public enum ButtonOption
    {
        triggerButton,
        primary2DAxisClick,
        primary2DAxisTouch,
        menuButton,
        gripButton,
        secondaryButton,
        secondaryTouch,
        primaryButton,
        primaryTouch
    };

    public bool isButtonDown { get; private set; } = false;
    public bool wasButtonPressedLastFrame { get; private set; } = false;

    private bool isPressed = false;

    private InputDeviceCharacteristics deviceCharacteristics;
    private List<InputDevice> inputDevices;
    private bool inputValue;
    private InputFeatureUsage<bool> inputFeature;

    public OculusButton(InputDeviceCharacteristics deviceCharacteristics, ButtonOption button)
    {
        this.deviceCharacteristics = deviceCharacteristics;

        // get label selected by the user
        string featureLabel = Enum.GetName(typeof(ButtonOption), button);

        // find dictionary entry
        availableButtons.TryGetValue(featureLabel, out inputFeature);

        // init list
        inputDevices = new List<InputDevice>();
    }

    /// <summary>
    /// Should be called in update function to track if button state was changed since last frame.
    /// </summary>
    public void UpdateCheckButtonPressed()
    {
        InputDevices.GetDevicesWithCharacteristics(deviceCharacteristics, inputDevices);
        if (wasButtonPressedLastFrame)
        {
            wasButtonPressedLastFrame = false;
        }
        for (int i = 0; i < inputDevices.Count; i++)
        {
            if (inputDevices[i].TryGetFeatureValue(inputFeature, out inputValue) && inputValue)
            {
                if (!isButtonDown)
                {
                    isButtonDown = true;
                    wasButtonPressedLastFrame = false;
                }
            }
            else if (isButtonDown)
            {
                isButtonDown = false;
                wasButtonPressedLastFrame = true;
            }
        }
    }
}