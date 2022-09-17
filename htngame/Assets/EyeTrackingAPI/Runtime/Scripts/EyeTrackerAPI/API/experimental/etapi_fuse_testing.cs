// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using System;
using System.Collections;
using UnityEngine;
using AdhawkApi.Numerics.Filters;
using AdhawkApi.FileIO;
using UnityEngine.SceneManagement;
using static UnityEngine.Mathf;
using UnityEngine.Events;

// disable warnings to do with unassigned and un-used fields
#pragma warning disable CS0649, CS0168, CS0414

namespace AdhawkApi
{
    public partial class EyeTrackerAPI
    {
        /*
        /// <summary>
        /// Update fuse data information and calculate fixation based on data.
        /// </summary>
        private void UpdateFuseHandler()
        {
            //so, for each active glint stream, we want to run through the data.
            //and, only after they are all full.

            bool trackingLeft = Time.time - lastFuseTimes[ID_LEFT] < MAX_GLINT_TIMEOUT ? true : false;
            bool trackingRight = Time.time - lastFuseTimes[ID_RIGHT] < MAX_GLINT_TIMEOUT ? true : false;

            if (trackingRight && trackingLeft)
            {
                UncalibratedTrackingState = TrackingState.TrackingBoth;
            }
            else if (trackingRight && !trackingLeft)
            {
                UncalibratedTrackingState = TrackingState.TrackingRight;
            }
            else if (!trackingRight && trackingLeft)
            {
                UncalibratedTrackingState = TrackingState.TrackingLeft;
            }
            else
            {
                UncalibratedTrackingState = TrackingState.TrackingLost;
            }

            Vector2 speedTotal = Vector2.zero;

            int divisor = 0;

            if (lastFuseTimeStamp - fuseStreams[ID_LEFT][fuse_stream_buffer_size - 1].Timestamp < 0.15f)
            {
                speedTotal += GetFuseVelocity(ID_LEFT);
                divisor += 1;
            }

            if (lastFuseTimeStamp - fuseStreams[ID_RIGHT][fuse_stream_buffer_size - 1].Timestamp < 0.15f)
            {
                speedTotal += GetFuseVelocity(ID_RIGHT);
                divisor += 1;
            }

            if (divisor > 0)
            {
                speedTotal = speedTotal / divisor;
                GlintMoveDeltasAverage = speedTotal.magnitude * 100;
            }
            else
            {
                GlintMoveDeltasAverage = 0;
            }

            fuseVelocityHighPass.Calculate(GlintMoveDeltasAverage);
            fuseVelocityLowPass.Calculate(GlintMoveDeltasAverage);
            // compare a high pass filter to a low-pass filter.
            if (fuseVelocityHighPass.value < fuseVelocityLowPass.value)
            {
                if (FixationTime < 0)
                {
                    FixationTime = 0;
                }
                FixationTime += Time.deltaTime;
            }
            else if (FixationTime > 0)
            {
                FixationTime = 0;
            }
            else
            {
                FixationTime -= Time.deltaTime;
            }
            //reset fuse dot positions if they are no longer available
            if (Time.time - lastFuseTimes[ID_LEFT] > MAX_TIME_RESET_GLINT)
            {
                FusePositions[ID_LEFT] = Vector2.zero;
            }
            if (Time.time - lastFuseTimes[ID_RIGHT] > MAX_TIME_RESET_GLINT)
            {
                FusePositions[ID_RIGHT] = Vector2.zero;
            }

        }

        private void UpdateGlintHandler()
        {
            for (int i = 0; i < GlintPositions.Length; i++)
            {
                if (Time.time - lastGlintTimes[i] > MAX_TIME_RESET_GLINT)
                {
                    GlintPositions[i] = Vector2.zero;
                }
            }
        }
        /// <summary>
        /// Calculate fuse dot velocity for the purpose of fixation detection in pre-calibration state
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Vector2 GetFuseVelocity(int id)
        {
            return fuseStreams[id][0].Position - fuseStreams[id][fuse_stream_buffer_size - 1].Position;
        }

        */

    }
}