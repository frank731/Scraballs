using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdhawkApi;
using UnityEngine.SceneManagement;

public class CalibrationHandler : MonoBehaviour
{

    public Transform instructions;

    ControllerInputHandler controls;

    // Start is called before the first frame update
    void Start()
    {
        controls = ControllerInputHandler.Instance;
        StartCoroutine(WaitForDeviceAndStart());
    }

    IEnumerator WaitForDeviceAndStart()
    {
        yield return null;
        //. handle device connect and disconnect if it happens:
        while(Player.Instance.Device == null)
        {
            yield return null;
        }

        while (!EyeTrackerAPI.Instance.Calibrated)
        {
            yield return new WaitUntil(() => controls.RightCon.Trigger || controls.LeftCon.Trigger);

            instructions.gameObject.SetActive(false);

            // can use either-or:

            // EyeTrackerAPI.Instance.RunCalibrationProcedure();
            yield return (Player.Instance.Device as DefaultDevice).RunCalibration();
        }

        SceneManager.LoadScene(2);
        
    }

    // Update is called once per frame
    void Update()
    {
       if (EyeTrackerAPI.Instance.Calibrating)
        {
            if (controls.RightCon.A || controls.LeftCon.X)
            {
                (Player.Instance.Device as DefaultDevice).RequestNextCalibrationPoint();
            }
        } 
    }
}
