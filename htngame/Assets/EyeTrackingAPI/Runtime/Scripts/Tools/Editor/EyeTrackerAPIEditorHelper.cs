
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AdhawkApi {

    public class EyeTrackerAPIEditorHelper : MonoBehaviour
    {
        [MenuItem("EyeTracking/Add EyeTracking")]
        static void AddEyeTrackerAPI()
        {
            GameObject o = GameObject.FindObjectOfType<EyeTrackerAPI>()?.gameObject;

            if (o != null)
            {
                Debug.Log("EyeTrackerAPI already exists in scene!");
                Selection.activeObject = o;
                return;
            }

            o = new GameObject();
            o.name = "[EyeTrackerAPI]";
            o.AddComponent<EyeTrackerAPI>();
            Selection.activeObject = o;
        }

        [MenuItem("EyeTracking/Add a Player DeviceManager")]
        static void AddDeviceManager()
        {
            GameObject o = FindObjectOfType<AdhawkApi.Player>()?.gameObject;

            if (o != null)
            {
                Debug.Log("Player/DeviceManager already exists in scene!");
                Selection.activeObject = o;
                return;
            }

            o = new GameObject();
            o.name = "Player";
            o.AddComponent<AdhawkApi.Player>();

            Transform cam = Camera.main.transform;

            if (cam != null)
            {
                cam.SetParent(o.transform);
                Debug.Log("Moved Camera to the Player object. See Documentation for details.");
            }

            Selection.activeObject = o;
        }

        [MenuItem("GameObject/EyeTracking/GazeEventSystem", false, 10)]
        static void AddGazeEventSystem()
        {
            GameObject o = FindObjectOfType<AdhawkApi.GazeEventSystem>()?.gameObject;

            if (o != null)
            {
                Debug.Log("GazeEventSystem already exists in scene!");
                Selection.activeObject = o;
                return;
            }

            o = new GameObject();
            o.name = "EventSystem";
            o.AddComponent<AdhawkApi.GazeEventSystem>();

            Selection.activeObject = o;
        }

        [MenuItem("GameObject/EyeTracking/GazeDetector", false, 10)]
        static void AddGazeDetector()
        {
            GameObject o = FindObjectOfType<AdhawkApi.GazeEventSystem>()?.gameObject;

            if (o != null)
            {
                Debug.Log("GazeEventSystem already exists in scene!");
                Selection.activeObject = o;
                return;
            }

            o = new GameObject();
            o.name = "EventSystem";
            o.AddComponent<AdhawkApi.GazeDetector>();
            o.AddComponent<SphereCollider>();

            Selection.activeObject = o;
        }
    }
}
#endif