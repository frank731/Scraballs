using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Deprecated
*/

namespace AdhawkApi.Logging
{
    class FloatData
    {
        public Dictionary<string, object> Data = new Dictionary<string, object>();

        public void Record(string key, float value)
        {
            try
            {
                ((List<float>)Data[key]).Add(value);
            }
            catch (KeyNotFoundException)
            {
                Data.Add(key, new List<float> { value });
            }
        }

        /// <summary>
        /// Returns Tracker Data in dictionary format to aid serialization
        /// </summary>
        /// <returns>dictionary of tracker data</returns>
        public Dictionary<string, object> ConvertToDict()
        {
            return Data;
        }
    }
}