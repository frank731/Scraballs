// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using System.Collections.Generic;
using UnityEngine;

namespace AdhawkApi
{
    ///<summary> Creates a UDP request object that holds information for a UDP message and what to do when an acknowledgement is recieved. </summary>
    public struct UDPRequest{
        public byte packetType; 
        public bool ackRecieved {get; private set;}
        public UDPRequestCallback ackCallback;
        public IEnumerable<byte> data;
        public byte[] returnData;
        public float timestamp;
        public UDPRequestStatus status;

        public UDPRequest(byte packetType, UDPRequestCallback ackCallback, IEnumerable<byte> data){
            this.packetType = packetType;
            this.ackCallback = ackCallback;
            this.ackRecieved = false;
            this.data = data;
            returnData = new byte[0];
            timestamp = Time.time;
            status = UDPRequestStatus.Unsent;
        }

        public void SetAckRecieved(byte[] data){
            this.ackRecieved = true;
            this.returnData = data;
            if (data.Length >= 1){
                if (data[0] == udpInfo.ACK_ERROR){
                    this.status = UDPRequestStatus.AckError;
                } else if (data[0] == udpInfo.ACK_SUCCESS){
                    this.status = UDPRequestStatus.AckSuccess;
                }
            } else {
                this.status = UDPRequestStatus.Recieved;
            }
            if (ackCallback != null){
                ackCallback(data,this.status);
            }
        }
    }
    /// <summary>
    /// Result of the UDPRequest.
    /// </summary>
    public enum UDPRequestStatus{
        ///<summary>Acknowledgement returned with no error</summary>
        AckSuccess,
        ///<summary>Acknowledgement returned with error code.</summary>
        AckError,
        ///<summary>Empty Acknowledgement recieved.</summary>
        Recieved,
        ///<summary>Acknowledgement timed out.</summary>
        Timeout,
        ///<summary>UDPRequest has been sent, we are waiting for a return.</summary>
        Waiting,
        ///<summary>UDPRequest has not yet been sent.</summary>
        Unsent,
        ///<summary>unused.</summary>
        None,
    }

    public enum StreamRate
    {
        Default = 60,
        Fast = 240
    }
    
    /// <summary>
    /// Callback to be used when sending UDPRequests via coroutine
    /// </summary>
    /// <param name="result">the result of the UDPRequest.</param>
    public delegate void UDPRequestCallback(byte[] data, UDPRequestStatus result);
}