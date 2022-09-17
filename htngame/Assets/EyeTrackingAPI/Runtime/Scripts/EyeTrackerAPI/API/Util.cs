// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.

using System;
using UnityEngine;

namespace AdhawkApi
{
    public static class Util
    {

        private static void Validate(byte[] data, int index, int size)
        {
            if (data.Length - index < size)
            {
                throw new IndexOutOfRangeException(
                    string.Format(
                        "Error converting byte[] data to Vector3. Data length: {0}, expected length to be at least: {1}",
                        data.Length,
                        size));
            }
        }

        #region Vector2 and Vector3 converters

        /// <summary>
        /// Convert Vector3 to byte array
        /// </summary>
        public static byte[] ToBytes(this Vector3 vec)
        {
            byte[] buff = new byte[sizeof(float) * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(vec.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(vec.y), 0, buff, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(vec.z), 0, buff, 2 * sizeof(float), sizeof(float));
            return buff;
        }

        public static Vector3 InvertZ(this Vector3 vec)
        {
            return new Vector3(vec.x, vec.y, -vec.z);
        }

        public static byte[] ToBytes(this Vector2 vec)
        {
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(vec.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(vec.y), 0, buff, 1 * sizeof(float), sizeof(float));
            return buff;
        }

        #endregion

        #region byte[] converters and sequential readers by provided ref index
        /// <summary>
        /// Convert byte[] to uint8_t
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the Vector3</param>
        /// <returns>Converted uint8_t</returns>
        public static byte ToInt8(this byte[] data, int index)
        {
            Validate(data, index, sizeof(byte));
            return data[index];
        }
        /// <summary>
        /// Convert byte[] to uint8_t
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the Vector3</param>
        public static void ReadNextInt8(this byte[] data, ref int index, out byte input)
        {
            Validate(data, index, sizeof(byte));
            input = data[index];
            index += 1;
        }
        /// <summary>
        /// Convert byte[] to Vector3
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the Vector3</param>
        /// <returns>Converted Vector3</returns>
        public static Vector3 ToVector3(this byte[] data, int startIndex)
        {
            Validate(data, startIndex, 3 * sizeof(Single));
            return new Vector3(
                BitConverter.ToSingle(data, startIndex + (0 * sizeof(Single))),
                BitConverter.ToSingle(data, startIndex + (1 * sizeof(Single))),
                BitConverter.ToSingle(data, startIndex + (2 * sizeof(Single)))
            );
        }

        /// <summary>
        /// Convert byte[] to Vector3
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the Vector3</param>
        public static void ReadNextVector3(this byte[] data, ref int index, out Vector3 input)
        {
            Validate(data, index, 3 * sizeof(Single));
            input = new Vector3(
                BitConverter.ToSingle(data, index + (0 * sizeof(Single))),
                BitConverter.ToSingle(data, index + (1 * sizeof(Single))),
                BitConverter.ToSingle(data, index + (2 * sizeof(Single)))
            );
            index += sizeof(Single) * 3;
        }

        /// <summary>
        /// Convert byte[] to Vector2
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the Vector2</param>
        /// <returns>Converted Vector2</returns>
        public static Vector2 ToVector2(this byte[] data, int startIndex)
        {
            Validate(data, startIndex, 2 * sizeof(Single));

            return new Vector2(
                BitConverter.ToSingle(data, startIndex + (0 * sizeof(Single))),
                BitConverter.ToSingle(data, startIndex + (1 * sizeof(Single)))
            );
        }

        /// <summary>
        /// Convert byte[] to Vector2
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the Vector2</param>
        /// <returns>Integer size of requested type in bytes</returns>
        public static void ReadNextVector2(this byte[] data, ref int index, out Vector2 input)
        {
            Validate(data, index, 2 * sizeof(Single));
            input = new Vector2(
                BitConverter.ToSingle(data, index + (0 * sizeof(Single))),
                BitConverter.ToSingle(data, index + (1 * sizeof(Single)))
            );
            index += sizeof(float) * 2;
        }

        /// <summary>
        /// Convert byte[] to float
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the float</param>
        /// <returns>Converted float</returns>
        public static float ToSingle(this byte[] data, int startIndex)
        {
            Validate(data, startIndex, sizeof(Single));
            return BitConverter.ToSingle(data, startIndex);
        }

        /// <summary>
        /// Convert byte[] to float, increments index by size of float
        /// </summary>
        /// <param name="startIndex">index in the array in which to start looking for the float</param>
        /// <returns>Integer size of requested type in bytes</returns>
        public static void ReadNextSingle(this byte[] data, ref int index, out float input)
        {
            Validate(data, index, sizeof(Single));
            input = BitConverter.ToSingle(data, index);
            index += sizeof(Single);
        }
        #endregion

    }
}
