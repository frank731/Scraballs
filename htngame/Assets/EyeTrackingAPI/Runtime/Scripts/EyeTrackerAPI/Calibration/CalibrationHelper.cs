// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public static class CalibrationHelper
    {
        public const string button_calibration_next = "CalibrationNext";
        
        public static readonly Vector2[] fixedPoints = {
            new Vector2(-1f,-1f),new Vector2( 0f,-1f),new Vector2( 1f,-1f),
            new Vector2(-1f, 0f),new Vector2( 0f, 0f),new Vector2( 1f, 0f),
            new Vector2(-1f, 1f),new Vector2( 0f, 1f),new Vector2( 1f, 1f),
        };

        public static readonly Vector2[] fixedPoints4x4 = {
            new Vector2(-1.00f,-1.00f), new Vector2(-0.33f,-1.00f), new Vector2( 0.33f,-1.00f), new Vector2( 1.00f,-1.00f),
            new Vector2(-1.00f,-0.33f), new Vector2(-0.33f,-0.33f), new Vector2( 0.33f,-0.33f), new Vector2( 1.00f,-0.33f),
            new Vector2(-1.00f, 0.33f), new Vector2(-0.33f, 0.33f), new Vector2( 0.33f, 0.33f), new Vector2( 1.00f, 0.33f),
            new Vector2(-1.00f, 1.00f), new Vector2(-0.33f, 1.00f), new Vector2( 0.33f, 1.00f), new Vector2( 1.00f, 1.00f),
        };

        public static readonly Vector2[] fixedPoints6x6 =
        {
            new Vector2( 0.00f, 0.00f),
            new Vector2(-1.00f,-1.00f), new Vector2(-0.60f,-1.00f), new Vector2(-0.20f,-1.00f), new Vector2( 0.20f,-1.00f), new Vector2( 0.60f,-1.00f), new Vector2( 1.00f,-1.00f),
            new Vector2(-1.00f,-0.60f), new Vector2(-0.60f,-0.60f), new Vector2(-0.20f,-0.60f), new Vector2( 0.20f,-0.60f), new Vector2( 0.60f,-0.60f), new Vector2( 1.00f,-0.60f),
            new Vector2(-1.00f,-0.20f), new Vector2(-0.60f,-0.20f), new Vector2(-0.20f,-0.20f), new Vector2( 0.20f,-0.20f), new Vector2( 0.60f,-0.20f), new Vector2( 1.00f,-0.20f),
            new Vector2(-1.00f, 0.20f), new Vector2(-0.60f, 0.20f), new Vector2(-0.20f, 0.20f), new Vector2( 0.20f, 0.20f), new Vector2( 0.60f, 0.20f), new Vector2( 1.00f, 0.20f),
            new Vector2(-1.00f, 0.60f), new Vector2(-0.60f, 0.60f), new Vector2(-0.20f, 0.60f), new Vector2( 0.20f, 0.60f), new Vector2( 0.60f, 0.60f), new Vector2( 1.00f, 0.60f),
            new Vector2(-1.00f, 1.00f), new Vector2(-0.60f, 1.00f), new Vector2(-0.20f, 1.00f), new Vector2( 0.20f, 1.00f), new Vector2( 0.60f, 1.00f), new Vector2( 1.00f, 1.00f),
            new Vector2( 0.00f, 0.00f),
        };

        public static readonly Vector2[] fixedPoints7x7 =
        {
            new Vector2( 0.00f, 0.00f),
            new Vector2(-1.00f,-1.00f), new Vector2(-0.67f,-1.00f), new Vector2(-0.33f,-1.00f), new Vector2( 0.00f,-1.00f), new Vector2( 0.33f,-1.00f), new Vector2( 0.67f,-1.00f), new Vector2( 1.00f,-1.00f),
            new Vector2(-1.00f,-0.67f), new Vector2(-0.67f,-0.67f), new Vector2(-0.33f,-0.67f), new Vector2( 0.00f,-0.67f), new Vector2( 0.33f,-0.67f), new Vector2( 0.67f,-0.67f), new Vector2( 1.00f,-0.67f),
            new Vector2(-1.00f,-0.33f), new Vector2(-0.67f,-0.33f), new Vector2(-0.33f,-0.33f), new Vector2( 0.00f,-0.33f), new Vector2( 0.33f,-0.33f), new Vector2( 0.67f,-0.33f), new Vector2( 1.00f,-0.33f),
            new Vector2(-1.00f, 0.00f), new Vector2(-0.67f, 0.00f), new Vector2(-0.33f, 0.00f), new Vector2( 0.00f, 0.00f), new Vector2( 0.33f, 0.00f), new Vector2( 0.67f, 0.00f), new Vector2( 1.00f, 0.00f),
            new Vector2(-1.00f, 0.33f), new Vector2(-0.67f, 0.33f), new Vector2(-0.33f, 0.33f), new Vector2( 0.00f, 0.33f), new Vector2( 0.33f, 0.33f), new Vector2( 0.67f, 0.33f), new Vector2( 1.00f, 0.33f),
            new Vector2(-1.00f, 0.67f), new Vector2(-0.67f, 0.67f), new Vector2(-0.33f, 0.67f), new Vector2( 0.00f, 0.67f), new Vector2( 0.33f, 0.67f), new Vector2( 0.67f, 0.67f), new Vector2( 1.00f, 0.67f),
            new Vector2(-1.00f, 1.00f), new Vector2(-0.67f, 1.00f), new Vector2(-0.33f, 1.00f), new Vector2( 0.00f, 1.00f), new Vector2( 0.33f, 1.00f), new Vector2( 0.67f, 1.00f), new Vector2( 1.00f, 1.00f),
            new Vector2( 0.00f, 0.00f),
        };
    }
}