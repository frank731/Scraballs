// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS0649, CS0168

namespace AdhawkApi
{

    public delegate void UdpPacketHandler(byte[] data);

    /// <summary>
    /// This class is responsible for spawning a thread and managing communication with a backend endpoint.
    /// </summary>
    public class UDPCommHandler
    {
        public delegate void DebugCallBack(string logString);

        public DebugCallBack debugCallBackEvent = null;

        /// <summary>
        /// custom identifier for the commhandler.
        /// </summary>
        /// <value></value>
        public string Name { get; private set; }

        /// <summary>
        /// UDPCommHandler's endpoint for recieving and sending information.
        /// Essentially just port information
        /// </summary>
        /// <value></value>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// UDPCommHandler's System.Net.UdpClient - for handling information to the relevant port when being
        /// sent to this specific socket.
        /// </summary>
        private UdpClient _UdpClient;

        private Dictionary<byte, UdpPacketHandler> delegateMap = new Dictionary<byte, UdpPacketHandler>();
        
        private object ReadLockQueue = new object();
        private Queue<KeyValuePair<byte, byte[]>> recievedQueue;

        private bool running = true;

        public int LocalPort
        {
            get
            {
                return (_UdpClient.Client.LocalEndPoint as IPEndPoint).Port;
            }
        }

        public UDPCommHandler(string name, int port = -1)
        {
            this.Name = name;
            _UdpClient = new UdpClient(0);
            _UdpClient.Client.ReceiveTimeout = 1000;
            _UdpClient.Client.SendTimeout = 1000;

            if (port != -1)
            {
                EndPoint = new IPEndPoint(IPAddress.Loopback, port);
            }
            else
            {
                EndPoint = new IPEndPoint(IPAddress.Loopback, (_UdpClient.Client.LocalEndPoint as IPEndPoint).Port);
            }

            recievedQueue = new Queue<KeyValuePair<byte, byte[]>>();

            Task.Run(ReadPacketsAsync);
        }

        public void Kill()
        {
            running = false;
            _UdpClient.Close();
        }

        private async Task ReadPacketsAsync()
        {
            while (running)
            {
                try
                {
                    var receivedResults = await _UdpClient.ReceiveAsync();

                    /*  ---  debug packets  ---  */

                    // string toPrint = "";
                    // for (int i = 0; i < receivedResults.Buffer.Length; i++)
                    // {
                    //     toPrint = toPrint + receivedResults.Buffer[i].ToString() + ", ";
                    // }
                    // DebugLog(string.Concat("Recieved a new packet! ", toPrint));

                    /*  ---                 ---  */

                    byte[] receiveBytes = receivedResults.Buffer;

                    if (receiveBytes.Length > 0)
                    {
                        List<byte> payload = new List<byte>(receiveBytes);
                        payload.RemoveAt(0);

                        byte packetType = receiveBytes[0];
                        if (delegateMap.ContainsKey(packetType))
                        {
                            lock (ReadLockQueue)
                            {
                                recievedQueue.Enqueue(new KeyValuePair<byte, byte[]>(packetType, payload.ToArray()));
                            }
                        }
                    }
                } catch (TimeoutException te)
                {
                    // expected if backend is not running
                    if (debugCallBackEvent != null)
                        DebugLog("Timeout waiting for packets!");
                } catch (SocketException se)
                {
                    // expected
                }
                catch (ObjectDisposedException ode)
                {
                    if (debugCallBackEvent != null)
                        DebugLog(string.Concat("ERROR: (underlying socket has been closed) exception in async packet recieve for: ", Name, "\nError text: ", ode.ToString()));
                }
                catch (Exception e)
                {
                    if (debugCallBackEvent != null)
                        DebugLog(string.Concat("ERROR: exception in async packet recieve for: ", Name, "\nError text: ", e.ToString()));
                }
            }
        }

        /// <summary>
        /// This should be called in the main loop/main thread update to sort through and delegate the build up packet queue.
        /// Currently this is called in the main update loop by UDPBehaviour.
        /// </summary>
        /// <returns>Returns true if any packets are found in the queue.</returns>
        public bool ProcessPackets(float deltaTime = 0)
        {
            KeyValuePair<byte, byte[]> message;
            bool result = recievedQueue.Count > 0;

            if (!result && deltaTime > 0)
            {
                return result;
            }
            
            while (recievedQueue.Count > 0)
            {
                lock (ReadLockQueue)
                {
                    message = recievedQueue.Dequeue();
                }
                if (delegateMap.ContainsKey(message.Key))
                {
                    delegateMap[message.Key].Invoke(message.Value);
                }
            }

            return result;
        }
        public bool ProcessPackets()
        {
            KeyValuePair<byte, byte[]> message;
            bool result = (recievedQueue.Count > 0);
            
            while (recievedQueue.Count > 0)
            {
                lock (ReadLockQueue)
                {
                    message = recievedQueue.Dequeue();
                }
                if (delegateMap.ContainsKey(message.Key))
                {
                    delegateMap[message.Key].Invoke(message.Value);
                }
            }
            return result;
        }

        public void WriteBytes(byte[] data)
        {
            try
            {
                _UdpClient.Send(data, data.Length, EndPoint);
            }
            catch
            {
                // Do something
            }
        }
        public void WriteCommand(byte packetType, IEnumerable<byte> data)
        {
            WriteBytes(new byte[] { (byte)packetType }.Concat(data).ToArray());
        }

        public void WriteCommand(byte packetType, int data)
        {
            WriteBytes(new byte[] { (byte)packetType }.Concat(BitConverter.GetBytes(data)).ToArray());
        }

        private void DebugLog(string toLog)
        {
            if (debugCallBackEvent != null)
            {
                debugCallBackEvent(toLog);
            } else
            {
                // Console.WriteLine(toLog);
            }
        }

        /// <summary>
        /// Registers a listener for a packet with a specific header and runs a callback event when the packet is recieved.
        /// Multiple callbacks can be added.
        /// The function defined in handler will be given the data in the packet's payload as an argument
        /// </summary>
        /// <param name="packetType">Packet header type to look for</param>
        /// <param name="handler">The callback function that fulfills the requirements of 
        /// the delegate template UdpPacketHandler</param>
        public void RegisterPacketHandler(byte packetType, UdpPacketHandler handler)
        {
            if (delegateMap.ContainsKey(packetType))
            {
                delegateMap[packetType] += handler;
                return;
            }

            delegateMap.Add(packetType, handler);
        }

        /// <summary>
        /// Remove the listeners/callbacks for the specific packet type completely.
        /// </summary>
        /// <param name="packetType">Packet type to stop listening to</param>
        public void UnregisterPacketHandler(byte packetType)
        {
            delegateMap[packetType] = null;
            delegateMap.Remove(packetType);
        }

    }
}
