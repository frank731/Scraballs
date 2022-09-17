using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct ImuDataStruct : PacketDataStruct
    {
        //timestamp, gyro_x, gyro_y, gyro_z, accel_x, accel_y, accel_z
        public ImuDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            data.ReadNextVector3(ref i, out Rotation);
            data.ReadNextVector3(ref i, out Acceleration);
        }
        public float Timestamp;
        public Vector3 Rotation;
        public Vector3 Acceleration;
    }
    public class IMUDataStream : AHTrackingStream<ImuDataStruct>
    {
        public IMUDataStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x17; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.IMU; } }
    }
}
