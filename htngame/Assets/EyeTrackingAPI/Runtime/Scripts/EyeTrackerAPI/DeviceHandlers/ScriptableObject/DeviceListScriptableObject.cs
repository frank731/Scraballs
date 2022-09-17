using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi {

    [CreateAssetMenu(fileName = "DeviceHandler", menuName = "DeviceHandler", order = 1)]
    public class DeviceListScriptableObject : ScriptableObject
    {
        [System.Serializable]
        public class DeviceHandler
        {
            public EyeTrackingDevice prefab;
            public string serialIdentifier;
        }

        [SerializeField] public DeviceHandler[] DeviceHandlers;
        [Header("_________________________________________________________________________________________")]// add a little space
        [SerializeField] public EyeTrackingDevice DefaultDevice;
    }
}