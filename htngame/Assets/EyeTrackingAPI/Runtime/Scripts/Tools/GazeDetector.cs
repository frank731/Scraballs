// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace AdhawkApi {

    public class GazeDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler
    {
        ///<summary> Essentially the "button action" of the gaze detector, works on blink. </summary>
        public UnityEvent OnBlink;

        public UnityEvent OnGazeEnter;

        public UnityEvent OnGazeExit;

        public UnityEvent OnGazeStay;

        bool pointerStay = false;

        // the rest of these are for the input system in other cases:
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnGazeEnter.Invoke();
            pointerStay = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnGazeExit.Invoke();
            pointerStay = false;
        }

        public void Update()
        {
            if (pointerStay)
            {
                OnGazeStay.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData){
            OnBlink.Invoke();
        }
        
        public void OnSelect(BaseEventData eventData){
            OnGazeEnter.Invoke();
        }
    }
}