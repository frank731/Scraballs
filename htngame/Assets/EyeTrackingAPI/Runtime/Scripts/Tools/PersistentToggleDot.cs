using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdhawkApi;

public class PersistentToggleDot : MonoBehaviour
{

    private static PersistentToggleDot _instance;
    private KeyCode toggleKey = KeyCode.Tab;
    private bool on = true;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(toggleKey))
        {
            on = !on;
        }

        if (on)
        {
            if (Player.Instance == null)
            {
                transform.position = Camera.main.transform.position + (Camera.main.transform.rotation * (EyeTrackerAPI.Instance.GazeVector.normalized * 1f));
            }
            else
            {

                //find a better position.
                RaycastHit hit;
                Ray ray = new Ray(Player.Instance.EyeCenter.position, EyeTrackerAPI.Instance.GazeVector.normalized);
                if (Physics.Raycast(ray, out hit, 2.0f))
                {
                    transform.position = hit.point;
                }
                else
                {
                    transform.position = Player.Instance.Cam.transform.position + (Player.Instance.Cam.transform.rotation * (EyeTrackerAPI.Instance.GazeVector.normalized * 1f));
                }

            }

        }
        else
        {
            transform.position = Vector3.down * 1000;
        }

    }
}
