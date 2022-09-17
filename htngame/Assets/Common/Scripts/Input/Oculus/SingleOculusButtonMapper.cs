using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

/// <summary>
/// Potential usage for wanting access to a single-button inside of unity using inspector drag-and-drop events
/// </summary>
public class SingleOculusButtonMapper : MonoBehaviour
{

    [Tooltip("Input device role (left or right controller)")]
    public InputDeviceCharacteristics deviceCharacteristics;

    [Tooltip("Select the button")]
    public OculusButton.ButtonOption buttonToTrack;

    [Tooltip("Event when the button starts being pressed")]
    public UnityEvent OnPress;

    OculusButton button;

    void Awake()
    {
        button = new OculusButton(deviceCharacteristics, buttonToTrack);
    }

    void Update()
    {
        button.UpdateCheckButtonPressed();
        if (button.wasButtonPressedLastFrame)
        {
            OnPress.Invoke();
        }
    }
}