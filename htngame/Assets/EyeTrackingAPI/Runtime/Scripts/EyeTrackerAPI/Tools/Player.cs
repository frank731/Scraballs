using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdhawkApi.FileIO;

namespace AdhawkApi
{

    public class Player : MonoBehaviour
    {

        public static Player Instance {
            get
            {
                return _instance;
            }
        }

        private static Player _instance;

        /// <summary>
        /// reference to EyeCenter position
        /// </summary>
        public Transform EyeCenter;

        /// <summary>
        /// Player camera object.
        /// </summary>
        public Camera Cam;

        public EyeTrackingDevice Device { get; private set; }

        [SerializeField] private string deviceOverride = "";

        // Start is called before the first frame update
        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                _instance.transform.position = transform.position;
                _instance.transform.rotation = transform.rotation;
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }

            if (EyeCenter == null)
            {
                EyeCenter = transform;
            }
            if (Cam == null)
            {
                Cam = Camera.main;
            }
        }

        private void Start()
        {
            StartCoroutine("DetectAndAssignDeviceHandler");
        }

        public IEnumerator DetectAndAssignDeviceHandler()
        {
            string deviceName = "";
            EyeTrackingDevice targetDevice = null;
            DeviceListScriptableObject deviceList = Resources.Load<DeviceListScriptableObject>("AdhawkDeviceList");

            yield return new WaitUntil(() => EyeTrackerAPI.Instance.BackendConnected);
            Debug.Log("getting system info...");
            yield return EyeTrackerAPI.Instance.RequestDeviceSerial((string result) => 
            {
                deviceName = result;
                Debug.Log("Backend returned device name: " + deviceName);
            });
            Debug.Log("end request: " +  deviceName);
            if (deviceName == "")
            {
                deviceOverride = INIWorker.IniReadValue(INIWorker.Sections.info, INIWorker.Keys.deviceName, "");
                if (deviceOverride != "")
                {
                    deviceName = deviceOverride;
                }
            }

            for (int i = 0; i < deviceList.DeviceHandlers.Length; i++)
            {
                if (deviceList.DeviceHandlers[i].serialIdentifier == "") continue;
                if (deviceName.ToLower().StartsWith(deviceList.DeviceHandlers[i].serialIdentifier.ToLower()))
                {
                    targetDevice = deviceList.DeviceHandlers[i].prefab;
                    break;
                }
            }

            if (targetDevice == null)
            {
                Debug.Log("Assigning fallback device handler");
                targetDevice = deviceList.DefaultDevice;
            }

            if (EyeTrackerAPI.Instance.SimulatedWithMouse)
            {
                targetDevice = deviceList.DefaultDevice;
            }

            Device = InstantiateDevice(targetDevice);
            EyeTrackerAPI.Instance.RegisterDeviceInterface(Device);
        }

        /// <summary>
        /// Manually assign/switch the default Device Manager Device
        /// </summary>
        /// <param name="targetDevice"></param>
        public void ManuallyAssignDevice(DeviceListScriptableObject.DeviceHandler targetDevice)
        {
            StopAllCoroutines();
            EyeTrackingDevice newDevice = targetDevice.prefab;
            Device = InstantiateDevice(newDevice);
            EyeTrackerAPI.Instance.RegisterDeviceInterface(Device);
        }

        public EyeTrackingDevice InstantiateDevice(EyeTrackingDevice newDevice)
        {
            if (Device != null)
            {
                Destroy(Device.gameObject);
            }
            Device = Instantiate(original: newDevice, parent: this.transform);
            EyeTrackerAPI.Instance.RegisterDeviceInterface(Device);
            return Device;
        }
    }

}