// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine;
using System.Collections;

/// <summary>
/// The gaze input module replaces the standard cursor control with the user's gaze
/// Handlers such as IPointerEnter and IPointerExit will be called when the user looks at
/// and away from designated objects
/// TO USE:
/// Attach this script to the eventsystem object and assign the main camera to this script
/// Select the layers to be raycasted from the layer mask dropdown menu
/// By default this will raycast to UI elements only
/// To raycast to both 3D mesh colliders and UI elements attach a physics raycaster component
/// to the main camera assigned to this script
/// ensure raycasted objects have the correct handlers implemented or event trigger components created
/// </summary>
namespace AdhawkApi
{
    public class GazeEventSystem : PointerInputModule
    {
        [SerializeField] private string SubmitButton = "Submit";

        [SerializeField] private bool debug = false;

        public LayerMask SelectablesLayer;
        private EyeTrackerAPI eyeTrackerAPI = EyeTrackerAPI.Instance;
        private PointerEventData pointerEventData;

        public RectTransform blinkTarget;


        protected override void Awake()
        {
            base.Awake();
            if (blinkTarget != null)
            {
                blinkTarget.gameObject.SetActive(false);

            }
        }

        /// <summary>
        /// Process inputs and trigger handlers
        /// </summary>
        public override void Process()
        {
            UpdateGaze();
            UpdateSelection();
        }

        /// <summary>
        /// Take the latest gaze position from trackers and raycast
        /// Update handlers from the first valid hit from raycasting
        /// </summary>
        private void UpdateGaze()
        {
            if (eyeTrackerAPI == null)
            {
                eyeTrackerAPI = EyeTrackerAPI.Instance;
            }

            if (pointerEventData == null)
            {
                pointerEventData = new PointerEventData(EventSystem.current);
            }

            Vector3 gazeVector = eyeTrackerAPI.GazeVector;
            if (eyeTrackerAPI.DidBlinkLastFrame)
            {
                gazeVector = eyeTrackerAPI.GazeBeforeBlink;
                StopCoroutine("ShowBlinkTarget");
                StartCoroutine("ShowBlinkTarget");
            }

            float scale = 1f;
            if (Player.Instance == null)
            {
                pointerEventData.position = Camera.main.WorldToScreenPoint(
                    Camera.main.transform.position + (Camera.main.transform.rotation * gazeVector)) * scale;
            }
            else
            {
                pointerEventData.position = Player.Instance.Cam.WorldToScreenPoint(
                    Player.Instance.EyeCenter.position + (Player.Instance.EyeCenter.rotation * gazeVector)) * scale;
#if UNITY_EDITOR
                Debug.DrawRay(Player.Instance.EyeCenter.position, (Player.Instance.EyeCenter.rotation * gazeVector) * 100, Color.blue, 0.1f, true);
#endif
            }

            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            pointerEventData.pointerCurrentRaycast = new RaycastResult();

            //find closest hit object with valid layer
            float closestHit = Mathf.Infinity;
            foreach (RaycastResult hit in raycastResults)
            {
                if (hit.gameObject != null && hit.distance < closestHit)
                {
                    pointerEventData.pointerCurrentRaycast = hit;
                    closestHit = hit.distance;
                }
            }
            ProcessMove(pointerEventData);
        }

        IEnumerator ShowBlinkTarget()
        {
            if (blinkTarget == null)
            {
                yield break;
            }
            blinkTarget.gameObject.SetActive(true);
            blinkTarget.transform.position = Camera.main.WorldToScreenPoint(
                    Camera.main.transform.position + (Camera.main.transform.rotation * EyeTrackerAPI.Instance.GazeBeforeBlink));
            yield return new WaitForSeconds(1.5f);
            blinkTarget.gameObject.SetActive(false);
        }

        private void DebugMessage(string message)
        {
#if UNITY_EDITOR
            if (debug) { Debug.Log(message); }
#endif
        }

        private void UpdateSelection()
        {
            //Check gaze has raycast to valid object
            if (pointerEventData.pointerCurrentRaycast.gameObject != null)
            {
                // Upwards traversal to find the first object that implements ISelectHandler
                // Useful for UI objects with children in front that do not implement ISelectHandler (e.g. text field in button)
                GameObject handler = ExecuteEvents.GetEventHandler<ISelectHandler>(pointerEventData.pointerCurrentRaycast.gameObject);

                if ((Input.GetButtonDown(SubmitButton) || EyeTrackerAPI.Instance.DidBlinkLastFrame) && handler != null)
                {

                    EventSystem.current.SetSelectedGameObject(handler);
                    // Upwards traversal to find object that implements click handler and send it an event
                    ExecuteEvents.ExecuteHierarchy(handler, pointerEventData, ExecuteEvents.pointerClickHandler);
#if UNITY_EDITOR
                    DebugMessage(string.Format("Blinked! In Object: {0}", handler.name));
#endif
                }
            }
        }
    }
}