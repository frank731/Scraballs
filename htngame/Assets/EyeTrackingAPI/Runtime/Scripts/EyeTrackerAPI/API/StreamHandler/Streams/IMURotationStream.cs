using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AdhawkApi
{
    public struct ImuRotationDataStruct : PacketDataStruct
    {
        //timestamp, gyro_x, gyro_y, gyro_z, accel_x, accel_y, accel_z
        public ImuRotationDataStruct(byte[] data)
        {
            int i = 0;
            data.ReadNextSingle(ref i, out Timestamp);
            float x;
            float y;
            float z;
            data.ReadNextSingle(ref i, out z);
            data.ReadNextSingle(ref i, out y);
            data.ReadNextSingle(ref i, out x);
            Rotation = new Vector3(x, -y, z);
        }
        public float Timestamp;
        public Vector3 Rotation;
    }
    public class IMURotationStream : AHTrackingStream<ImuRotationDataStruct>
    {
        public IMURotationStream(UDPBehaviour udpClient) : base(udpClient) { }
        public override byte StreamPacketByte { get { return 0x19; } }

        public override udpInfo.StreamControl StreamControlBit { get { return udpInfo.StreamControl.IMU_ROTATION; } }
    }
}
