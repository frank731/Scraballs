using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdhawkApi
{
    public class DefaultDevice : EyeTrackingDevice
    {
        /// <summary>
        /// How wide the calibration points should be spread
        /// </summary>
        [Tooltip("How wide the calibration points should be spread." +
            "\nX and Y angle are 3-D relative. (x is vertical, y is horizontal)")]
        [SerializeField] protected Vector2 calAngleSpread = new Vector2(20, 20);
        /// <summary>
        /// Offset the calibration by 3D angle. X is vertical, y is horizontal
        /// </summary>
        [SerializeField] protected Vector2 calAngleOffset;
        /// <summary>
        /// Distance from the camera to display the calibration target if it is in 3D space.
        /// </summary>
        [SerializeField] protected float calTargetDistance = 10.0f;
        /// <summary>
        /// Access to the text object to communicate errors to the player.
        /// </summary>
        [SerializeField] protected Text text;

        /// <summary>
        /// Returns true if calibration is in process
        /// </summary>
        public bool Calibrating { get; protected set; }

        /// <summary>
        /// For handling calibration stopping
        /// </summary>
        protected Coroutine calibrationHandler;

        /// <summary>
        /// For handling validation stopping
        /// </summary>
        protected Coroutine validationHandler;

        /// <summary>
        /// Access main player object
        /// </summary>
        protected Player player;

        /// <summary>
        /// For changing the displayed error text
        /// </summary>
        protected float errorTimer;

        protected bool nextPoint = false;

        /// <summary>
        /// Tracks the previous parent transform of the gaze target object so it can be put back after
        /// </summary>
        private Transform oldParent;

        protected virtual void Start()
        {
            player = Player.Instance;
            HideTarget();
            EyeTrackerAPI.Streams.Gaze.AddListener((screenGazeData) => {
                Vector4 timeGaze = EyeTrackerAPI.Instance.GazeVector;
                timeGaze.w = ((GazeDataStruct)screenGazeData).Timestamp;
                EyeTrackerAPI.Instance.OldGazeData.Append(timeGaze);
            });
        }
        protected virtual void Update()
        {
            if (errorTimer > 0)
            {
                errorTimer -= Time.deltaTime;
            }
            else if (errorTimer < 0)
            {
                errorTimer = 0;
                text.text = "";
            }
        }

        public override Coroutine RunCalibration()
        { // default 3x3 calibration
            calibrationHandler = StartCoroutine(RunCalibrationCoroutine());
            return calibrationHandler;
        }
        public override Coroutine RunValidation()
        {
            validationHandler = StartCoroutine(RunValidationCoroutine());
            return validationHandler;
        }
        public override Coroutine RunRecenter()
        {
            return StartCoroutine(RunRecenterCoroutine());
        }

        public override Coroutine RequestAutotune()
        {
            return StartCoroutine(RequestAutotuneCoroutine(0));
        }

        enum SequenceType
        {
            Calibration,
            Validation
        }

        public Coroutine RequestAutotuneOffset(float offset)
        {
            return StartCoroutine(RequestAutotuneCoroutine(offset));
        }

        public void Shuffle(Vector2[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n);
                Vector2 value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private IEnumerator RunCalibrationSequence(Vector2[] points, float hSpan, float vSpan, float hOffsetAngle, float vOffsetAngle, bool random=true)
        {
            if (Calibrating) yield break; Calibrating = true;

            Vector2[] pointsRan = points;
            Shuffle(pointsRan);

            Debug.Log("Calibration request.");

            UDPRequestStatus startReqStatus = UDPRequestStatus.None;

            yield return EyeTrackerAPI.Instance.QueryBeginCalibration(successCallback: (byte[] data, UDPRequestStatus result) => {
                startReqStatus = result;
            });

            ShowTarget();

            bool next = false;

            for (int curPoint = 0; curPoint < points.Length; curPoint = next ? curPoint + 1 : curPoint)
            {
                next = false;

                Vector3 targetPos = Quaternion.Euler(points[curPoint].y * (vSpan / 2) + vOffsetAngle, 0, 0) * Vector3.forward;
                targetPos = Quaternion.Euler(0, points[curPoint].x * (hSpan / 2) + hOffsetAngle, 0) * targetPos;

                targetPos = targetPos * calTargetDistance;

                target.transform.localPosition = targetPos;
                bool cancel = false;
                bool skip = false;
                Debug.Log("Listening for input...");
                yield return new WaitUntil(() =>
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        cancel = true;
                        nextPoint = false;
                        return true;
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        next = true;
                        nextPoint = false;
                        skip = true;
                        return true;
                    }
                    return Input.GetButtonDown(CalibrationHelper.button_calibration_next) || nextPoint;
                });
                if (skip || cancel) yield return null;
                if (skip) continue;
                if (cancel) break;
                nextPoint = false;

                UDPRequestCallback callback = (byte[] data, UDPRequestStatus status) =>
                {
                    if (status == UDPRequestStatus.AckSuccess)
                    {
                        next = true;
                    }
                    else if (status == UDPRequestStatus.Timeout)
                    {
                        SetErrorText("Error: Timeout when waiting for backend response.");
                    }
                    else
                    {
                        if (data.Length == 1)
                        {
                            SetErrorText("Error: " + udpInfo.GetAckPacketTypeInfo(data[0]));
                        }
                    }
                };

                Vector3 sendPos = targetPos;

                // override next and error for now

                yield return EyeTrackerAPI.Instance.RegisterCalibrationPoint(sendPos, callback);
                if (EyeTrackerAPI.Instance.autoSkipInvalidCalibrationPoints)
                {
                    target.PulseTarget();
                    next = true;
                }
                else
                {
                    if (next == true)
                    {
                        target.PulseTarget();
                    }
                    else
                    {
                        target.PulseError();
                    }
                }

                yield return new WaitForSeconds(0.45f);
            }

            HideTarget();
            Calibrating = false;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                yield return EyeTrackerAPI.Instance.AbortCalibration();
            }
            else
            {
                yield return EyeTrackerAPI.Instance.QueryEndCalibration(1, (byte[] data, UDPRequestStatus result) =>
                {
                    if (result == UDPRequestStatus.Timeout)
                    {
                        Debug.LogError("Error: Timeout when requesting calibration end.");
                        SetErrorText("Timeout when ending calibration");
                    }
                });
            }
        }

        public AnimationCurve targetMovementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve targetSizeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


        private IEnumerator AnimateTargetToPoint(Vector3 targetPos)
        {
            yield return null;

            float duration_seconds = 1.0f;

            float startTime = Time.time;

            Vector3 targetCurrentLocalPos = target.transform.localPosition;
            Vector3 scaleStart = target.transform.localScale;

            float curTime = 0;

            while (curTime < duration_seconds)
            {
                yield return null;
                curTime += Time.deltaTime;

                float progress = curTime / duration_seconds;

                progress = targetMovementCurve.Evaluate(progress);

                float scale = 1.0f;

                scale = targetSizeCurve.Evaluate(progress);

                target.transform.localScale = scaleStart * scale;

                target.transform.localPosition = Vector3.Lerp(targetCurrentLocalPos, targetPos, progress);
            }

            target.transform.localPosition = targetPos;
            target.transform.localScale = scaleStart;
        }

        private IEnumerator RunCalibrationCoroutine()
        {

            Vector2[] pointsRan = CalibrationHelper.fixedPoints;
            Shuffle(pointsRan);

            if (Calibrating) yield break; Calibrating = true;

            ShowTarget();

            target.transform.localPosition = Vector3.forward * calTargetDistance;

            yield return new WaitForSeconds(0.8f);
            yield return EyeTrackerAPI.Instance.RequestAutotune();
            HideTarget();

            Debug.Log("Calibration request.");

            UDPRequestStatus startReqStatus = UDPRequestStatus.None;

            yield return EyeTrackerAPI.Instance.QueryBeginCalibration(successCallback: (byte[] data, UDPRequestStatus result) => {
                startReqStatus = result;
            });

            ShowTarget();


            bool next = false;

            // current standard supported FOV for eyetracking:
            float horizontal_angle_spread = 40.0f;
            float vertical_angle_spread = 25.0f;

            for (int curPoint = 0; curPoint < pointsRan.Length; curPoint = next ? curPoint + 1 : curPoint)
            {
                next = false;

                Vector3 targetPos = Quaternion.Euler(pointsRan[curPoint].y * (vertical_angle_spread/2) + calAngleOffset.x, 0, 0) * Vector3.forward;
                targetPos = Quaternion.Euler(0, pointsRan[curPoint].x * (horizontal_angle_spread/2) + calAngleOffset.y, 0) * targetPos;

                targetPos = targetPos * calTargetDistance;


                // animate target here:
                yield return StartCoroutine(AnimateTargetToPoint(targetPos));

                bool cancel = false;
                bool skip = false;
                yield return new WaitUntil(() =>
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        cancel = true;
                        nextPoint = false;
                        return true;
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        next = true;
                        nextPoint = false;
                        skip = true;
                        return true;
                    }
                    return Input.GetButtonDown(CalibrationHelper.button_calibration_next) || nextPoint;
                });
                if (skip || cancel) yield return null;
                if (skip) continue;
                if (cancel) break;
                nextPoint = false;

                UDPRequestCallback callback = (byte[] data, UDPRequestStatus status) =>
                {
                    if (status == UDPRequestStatus.AckSuccess)
                    {
                        next = true;
                    }
                    else if (status == UDPRequestStatus.Timeout)
                    {
                        SetErrorText("Error: Timeout when waiting for backend response.");
                    }
                    else
                    {
                        if (data.Length == 1)
                        {
                            if (data[0] != 0x02)
                            {
                                SetErrorText("Error: " + udpInfo.GetAckPacketTypeInfo(data[0]));
                            }
                        }
                    }
                };

                Vector3 sendPos = targetPos;

                // override next and error for now

                yield return EyeTrackerAPI.Instance.RegisterCalibrationPoint(sendPos, callback);
                target.PulseTarget();
                next = true;
                // if (next == true)
                // {
                //     target.PulseTarget();
                // }
                // else
                // {
                //     target.PulseError();
                // }
                yield return new WaitForSeconds(0.30f);
            }

            HideTarget();
            Calibrating = false;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                yield return EyeTrackerAPI.Instance.AbortCalibration();
            }
            else
            {
                yield return EyeTrackerAPI.Instance.QueryEndCalibration(1, (byte[] data, UDPRequestStatus result) =>
                {
                    if (result == UDPRequestStatus.Timeout)
                    {
                        Debug.LogError("Error: Timeout when requesting calibration end.");
                        SetErrorText("Timeout when ending calibration");
                    }
                });
            }
        }

        private IEnumerator RunValidationCoroutine()
        {
            if (Calibrating) yield break; Calibrating = true;

            Debug.Log("Starting validation request");

            yield return EyeTrackerAPI.Instance.QueryBeginValidation();

            ShowTarget();
            bool next = false;

            for (int curPoint = 0; curPoint < CalibrationHelper.fixedPoints4x4.Length; curPoint = next ? curPoint + 1 : curPoint)
            {
                next = false;
                Vector3 targetPos = Quaternion.Euler(CalibrationHelper.fixedPoints4x4[curPoint].y * 12.5f + calAngleOffset.x, 0, 0) * Vector3.forward;
                targetPos = Quaternion.Euler(0, CalibrationHelper.fixedPoints4x4[curPoint].x * 20.0f + calAngleOffset.y, 0) * targetPos;
                Debug.Log("Validation Position: " + (CalibrationHelper.fixedPoints4x4[curPoint].y * calAngleSpread.x + calAngleOffset.x).ToString());

                target.transform.localPosition = targetPos;
                bool cancel = false;
                bool skip = false;
                nextPoint = false;
                yield return new WaitUntil(() =>
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        cancel = true;
                        nextPoint = false;
                        return true;
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        next = true;
                        nextPoint = false;
                        skip = true;
                        return true;
                    }
                    return Input.GetButtonDown(CalibrationHelper.button_calibration_next) || nextPoint;
                });
                if (skip || cancel) yield return null;
                if (skip) continue;
                if (cancel) break;
                nextPoint = false;

                UDPRequestCallback callback = (byte[] data, UDPRequestStatus status) =>
                {
                    if (status == UDPRequestStatus.AckSuccess)
                    {
                        next = true;
                    }
                    else if (status == UDPRequestStatus.Timeout)
                    {
                        SetErrorText("Error: Timeout when waiting for backend response.");
                    }
                    else
                    {
                        if (data.Length == 1)
                        {
                            SetErrorText("Error: " + udpInfo.GetAckPacketTypeInfo(data[0]));
                        }
                    }
                };

                Vector3 sendPos = targetPos;
                sendPos.z = -sendPos.z;

                yield return EyeTrackerAPI.Instance.RegisterValidationPoint(sendPos, callback);
                if (EyeTrackerAPI.Instance.autoSkipInvalidCalibrationPoints)
                {
                    target.PulseTarget();
                    next = true;
                }
                else
                {
                    if (next == true)
                    {
                        target.PulseTarget();
                    }
                    else
                    {
                        target.PulseError();
                    }
                }
                yield return new WaitForSeconds(0.45f);
            }
            HideTarget();
            Calibrating = false;
            yield return EyeTrackerAPI.Instance.QueryEndValidation(1, (byte[] data, UDPRequestStatus result) => {
                if (result == UDPRequestStatus.Timeout)
                {
                    Debug.LogError("Error: Timeout when requesting validation end.");
                    SetErrorText("Timeout when ending validation");
                }
            });
        }

        private IEnumerator RunRecenterCoroutine()
        {
            ShowTarget();
            target.transform.localPosition = Vector3.forward * calTargetDistance;
            yield return new WaitForSeconds(1.5f);
            HideTarget();
            yield return EyeTrackerAPI.Instance.RegisterRecenterPoint(Vector3.forward * calTargetDistance);
        }

        private IEnumerator RequestAutotuneCoroutine(float verticalOffset)
        {
            ShowTarget();
            if (verticalOffset != 0)
            {
                target.transform.localPosition = (Quaternion.AngleAxis(verticalOffset, Vector3.right) * Vector3.forward) * calTargetDistance;
            } else
            {
                target.transform.localPosition = Vector3.forward * calTargetDistance;
            }
            
            yield return new WaitForSeconds(0.8f);
            yield return EyeTrackerAPI.Instance.RequestAutotune();
            HideTarget();
        }

        public override void StopCurrentSequence()
        {
            StopCoroutine(calibrationHandler);
            Calibrating = false;
            HideTarget();
            EyeTrackerAPI.Instance.QueryEndCalibration();
        }
        public override void RequestNextCalibrationPoint()
        {
            if (EyeTrackerAPI.Instance.Calibrating)
            {
                nextPoint = true;
            }
        }
        public override Vector3 ProcessGazeVector(GazeDataStruct gazeData)
        {
            return gazeData.Position;
        }
        protected void SetErrorText(string text)
        {
            errorTimer = 2.0f;
            // this.text.text = text; // this text object's text string to be set to this functions text parameter
        }
        protected void ShowTarget()
        {
            target.gameObject.SetActive(true);
            oldParent = target.transform.parent;
            target.transform.SetParent(Camera.main.transform);
        }
        protected void HideTarget()
        {
            target.gameObject.SetActive(false);
            if (oldParent)
            {
                target.transform.SetParent(oldParent);
            }
        }
        protected void PositionTarget(Vector3 newPos)
        {
            target.transform.position = newPos;
        }

    }
}
