// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdhawkApi
{
    public partial class UDPBehaviour : MonoBehaviour
    {
        private UnityEngine.Events.UnityEvent onConnect = new UnityEngine.Events.UnityEvent();
        private UnityEngine.Events.UnityEvent onDisconnect = new UnityEngine.Events.UnityEvent();
        
        /// <summary>
        /// UDPBehaviour Connection State to server
        /// </summary>
        public enum ConnectionState
        {
            connected,
            disconnected,
            unknown
        }
        public ConnectionState BackendState { get; private set; } = ConnectionState.unknown;
        /// <summary>
        /// Whether or not this udp client is connected to the backend server
        /// </summary>
        /// <value></value>
        public bool ServerConnected
        {
            get
            {
                return BackendState == ConnectionState.connected;
            }
        }
        
        private static UInt32 streamControlMap = 0b_0000_0000_0000_0000_0000_0000_0000_0000; // explicity a 32-length bitmap
        private static UInt32 eventControlMap = 0b_0000_0000_0000_0000_0000_0000_0000_0000;


        private byte[] currentRegisterEndpointPacket = new byte[]{
            udpInfo.REGISTER_ENDPOINT, 0, 0, 0, 0
        };

        private float eyeTrackingRate;

        private UDPCommHandler streamHandler;
        private UDPCommHandler controller;
        // make sure there's only one of these around at once, since it only serves a single purpose for now.
        private static UDPBehaviour _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
                return;
            }
            controller = new UDPCommHandler("Controller", udpInfo.PORT_CONTROL_DATA);
            controller.debugCallBackEvent = Debug.Log;
            streamHandler = new UDPCommHandler("StreamHandler"); // do not define port since we want backend to have our generated one.
            // streamHandler.debugCallBackEvent = Debug.Log;
            Debug.Log("controller UDP port : " + controller.LocalPort);
            Debug.Log("streamHandler UDP port : " + streamHandler.LocalPort);
        }

        void Update()
        {
            controller.ProcessPackets();
            streamHandler.ProcessPackets(Time.deltaTime);
            LookForBackend();
        }

        private void OnDisable()
        {
            if (LoggingEnabled) SaveLog();
        }

        private string PacketInfo(byte commandType)
        {
            string packetType;
            if (udpInfo.CommandTypePacketLookup.TryGetValue(commandType, out packetType))
            {
                return string.Concat(commandType.ToString(packet_hex_format), " (", packetType, ")");
            } else
            {
                return commandType.ToString(packet_hex_format);
            }
        }
        
        public void RegisterOnConnect(UnityEngine.Events.UnityAction toRegister)
        {
            onConnect.AddListener(toRegister);
        }

        public void RegisterOnDisconnect(UnityEngine.Events.UnityAction toRegister)
        {
            onDisconnect.AddListener(toRegister);
        }

        private void StopAllStreams()
        {
            streamControlMap = 0b_0000_0000_0000_0000_0000_0000_0000_0000;
            UpdateStreamControl();
        }

        private void OnDestroy()
        {
            WriteCommand(udpInfo.STOP_LOG_SESSION, new byte[0]);
            WriteCommand(udpInfo.UNREGISTER_ENDPOINT, new byte[0]);
            controller.debugCallBackEvent -= Debug.Log;
            streamHandler.debugCallBackEvent -= Debug.Log;
            controller.Kill();
            streamHandler.Kill();
        }

        public void RegisterStreamHandler(byte packetType, UdpPacketHandler handler)
        {
            streamHandler.RegisterPacketHandler(packetType, handler);
        }

        public void UnregisterStreamHandler(byte packetType)
        {
            streamHandler.UnregisterPacketHandler(packetType);
        }

        /// <summary>
        /// Send a packet to backed with data
        /// </summary>
        public void WriteCommand(byte packetType, IEnumerable<byte> data)
        {
            controller.WriteBytes(new byte[] { (byte)packetType }.Concat(data).ToArray());
        }

        /// <summary>
        /// Send a packet to backend with data.
        /// </summary>
        public void WriteCommand(byte packetType, Int32 data)
        {
            byte[] packet = new byte[sizeof(Int32) + 1];
            packet[0] = packetType;
            Buffer.BlockCopy(BitConverter.GetBytes(data), 0, packet, 1, sizeof(Int32));
            controller.WriteBytes(packet);
        }

        /// <summary>
        /// Send a UDPRequest as a coroutine. Does not change the passed UDPRequest.
        /// </summary>
        /// <param name="request">UDPRequest struct filled out with information and callback information to run
        /// when the acknowledgement packet is recieved.</param>
        /// <param name="customReturnPacket">Custom expected packet type to return.</param>
        public Coroutine SendUDPRequest(UDPRequest request, float timeout = 4, byte customReturnPacket = 0)
        {
            return StartCoroutine(SendUDPRequestCoroutine(request, timeout, customReturnPacket));
        }

        private IEnumerator SendUDPRequestCoroutine(UDPRequest request, float timeout = 4, byte customReturnPacket = 0)
        {
            byte returnPacketType;

            // register our request
            if (customReturnPacket == 0)
            {
                returnPacketType = request.packetType;
            }
            else
            {
                returnPacketType = customReturnPacket;
            }

            // cleanup any old handles if they exist
            controller.UnregisterPacketHandler(returnPacketType);
            controller.RegisterPacketHandler(returnPacketType, (data) => {
                request.SetAckRecieved(data);
                LogRecievePacket(returnPacketType, data);
            });

            //set status to waiting.
            request.status = UDPRequestStatus.Waiting;
            // send command
            LogSentPacket(request.packetType, request.data);
            controller.WriteCommand(request.packetType, request.data);

            yield return new WaitUntil(() => {
                if (request.ackRecieved)
                { // result came back!
                    return true;
                }
                if (Time.time - request.timestamp > timeout)
                { // check for timeout.
                    request.status = UDPRequestStatus.Timeout; // set status incase this is read somewhere.
                    request.ackCallback?.Invoke(new byte[0], request.status);
                    LogTimeout(request.packetType);
                    controller.UnregisterPacketHandler(returnPacketType);
                    return true;
                }
                return false; // continue to next frame if nothing has happened yet.
            }
            );
        }


        public Coroutine RegisterRecenterPoint(byte[] data)
        {
            return StartCoroutine(RegisterRecenterPointCoroutine(data));
        }

        private IEnumerator RegisterRecenterPointCoroutine(byte[] data)
        {
            yield return SendUDPRequest(new UDPRequest(
                        udpInfo.START_RECENTER_PROCEDURE,
                        null,
                        data
                    )
                );
        }
        
        /// <summary>
        /// Toggle a stream on or off.
        /// </summary>
        /// <param name="control_stream_type"></param>
        /// <param name="enabled"></param>
        public void ToggleStream(UInt32 control_stream_type, bool enabled, StreamRate rate = StreamRate.Default)
        {
            if (enabled)
            {
                streamControlMap |= control_stream_type;
            }
            else
            {
                streamControlMap &= ~(control_stream_type);
            }

            // build packet data:

            byte[] result = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(control_stream_type), 0, result, 0, 4);
            if ((streamControlMap & control_stream_type) == control_stream_type)
            {
                Buffer.BlockCopy(BitConverter.GetBytes((float)rate), 0, result, 4, 4);
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(0f), 0, result, 4, 4);
            }

            // Debug.Log("Updating stream control: " + Convert.ToString(control_stream_type, 2));

            SetProperty(udpInfo.PropertyType.STREAM_CONTROL, result);
            // UpdateStreamControl(); // can no longer update multiple streams at the same time.
        }

        public void ToggleStream(udpInfo.StreamControl streamControlBit, bool enabled, StreamRate rate = StreamRate.Default)
        {
            ToggleStream((uint)streamControlBit, enabled, rate);
        }

        public void ToggleEvent(UInt32 eventControlBit, bool enabled)
        {
            // eventControlMap
            if (enabled)
            {
                eventControlMap |= eventControlBit;
            }
            else
            {
                eventControlMap &= ~(eventControlBit);
            }

            // build packet data:
            byte[] result = new byte[5];
            Buffer.BlockCopy(BitConverter.GetBytes(eventControlBit), 0, result, 0, 4);
            
            result[4] = enabled ? (byte)0b_1 : (byte)0b_0;

            // Debug.Log("Updating stream control: " + Convert.ToString(control_stream_type, 2));

            SetProperty(udpInfo.PropertyType.EVENT_CONTROL, result);
        }

        public void ToggleEvent(udpInfo.EventControl eventControlBit, bool enabled)
        {
            ToggleEvent((uint)eventControlBit, enabled);
        }

        private Coroutine SetProperty(udpInfo.PropertyType propertyType, IEnumerable<byte> data)
        {
            return StartCoroutine(SetPropertyCoroutine(propertyType, data));
        }

        private IEnumerator SetPropertyCoroutine(udpInfo.PropertyType propertyType, IEnumerable<byte> data)
        {
            UDPRequestCallback callback = (byte[] ack_data, UDPRequestStatus result) =>
            {
                if (result == UDPRequestStatus.AckError)
                {
                    //Debug.LogError(string.Concat("UDP Request Error! \ninfo: ",info));
                    Debug.Log(string.Concat("Error when setting property! \ninfo: ", udpInfo.GetAckPacketTypeInfo(ack_data[0])));
                }
            };

            yield return SendUDPRequest(new UDPRequest(
                udpInfo.PROPERTY_SET,
                callback,
                new byte[] { (byte)propertyType }.Concat(data)
            ));
        }
    }
}