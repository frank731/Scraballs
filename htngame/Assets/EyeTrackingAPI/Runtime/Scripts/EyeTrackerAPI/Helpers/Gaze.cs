// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using UnityEngine;

namespace AdhawkApi.Numerics
{
    class Gaze
    {
        public static Vector3 Angles2Vector(Vector2 angles)
        {
            //return Quaternion.Euler(angles.x, angles.y, 0) * Vector3.forward;
            return new Vector3(
                Mathf.Sin(angles.x) * Mathf.Cos(angles.y),
                Mathf.Sin(angles.y),
                Mathf.Cos(angles.x) * Mathf.Cos(angles.y));
            
        }

        public static Vector2 Vector2Angles(Vector3 vector)
        {
            //return Quaternion.LookRotation(vector).eulerAngles;
            float alpha = Mathf.Atan2(vector.x, vector.z);
            float beta = Mathf.Atan2(vector.y, Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z));
            return new Vector2(alpha, beta);
        }
    }
}
