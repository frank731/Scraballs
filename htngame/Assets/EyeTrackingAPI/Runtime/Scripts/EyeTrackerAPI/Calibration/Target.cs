// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AdhawkApi
{
    public class Target : MonoBehaviour
    {
        private const string anim_param_pulse = "Pulse";
        private const string anim_param_error = "Error";

        public enum TargetType
        {
            VR,
            ScreenSpace,
            Aruco
        }
        public TargetType targetType;
        private Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }

        public void PulseTarget()
        {
            if (anim)
            {
                anim.SetTrigger(anim_param_pulse);
            }
        }
        public void PulseError()
        {
            if (anim)
            {
                anim.SetTrigger(anim_param_error);
            }
        }
    }
}
