using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    public class AHStreamHandler
    {
        public AHStreamHandler(UDPBehaviour udpClient)
        {
            Gaze = new GazeStream(udpClient);
            GazeInScreen = new GazeInScreenStream(udpClient);
            PerEyeGaze = new PerEyeGazeStream(udpClient);
            PupilEllipse = new PupilEllipseStream(udpClient);
            PupilSize = new PupilSizeStream(udpClient);
            PupilPosition = new PupilPositionStream(udpClient);
            PupilXY = new PupilXYStream(udpClient);
            FuseXY = new FuseDataStream(udpClient);
            GlintXY = new GlintDataStream(udpClient);
            IMU_RAW = new IMUDataStream(udpClient);
            IMU = new IMURotationStream(udpClient);
            PulseXY = new PulseDataStream(udpClient);
            TrackerStatus = new TrackerStatusStream(udpClient);
            TrackerReady = new TrackerReadyStream(udpClient);
        }
        public GazeStream Gaze;
        public GazeInScreenStream GazeInScreen;
        public PerEyeGazeStream PerEyeGaze;
        public PupilEllipseStream PupilEllipse;
        public PupilSizeStream PupilSize;
        public PupilPositionStream PupilPosition;
        public PupilXYStream PupilXY;
        public FuseDataStream FuseXY;
        public GlintDataStream GlintXY;
        public IMUDataStream IMU_RAW;
        public IMURotationStream IMU;
        public PulseDataStream PulseXY;
        public TrackerStatusStream TrackerStatus;
        public TrackerReadyStream TrackerReady;
    }
}