using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



namespace HomeWork03.NetworkClient
{
    [System.Obsolete]
    public class Client : MonoBehaviour
    {
        public delegate void OnMessageReceive(object message);
        public event OnMessageReceive onMessageReceive;

        private const int MAX_CONNECTION = 10;
        private int port = 0;
        private int serverPort = 8888;
        private int hostID;
        private int reliableChannel;
        private int connectionID;
        private bool isConnected = false;
        private byte error;

        public string MyLogin { get; set; }


        public void Connect()
        {
           
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
            reliableChannel = cc.AddChannel(QosType.Unreliable);
            cc.SendDelay = 5;
            cc.ConnectTimeout = 200;
            HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
            hostID = NetworkTransport.AddHost(topology, port);
            connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", serverPort, 0, out error);
            if ((NetworkError)error == NetworkError.Ok)
                isConnected = true;
            else
                Debug.Log((NetworkError)error);
        }


        public void Disconnect()
        {
            if (!isConnected) return;
            NetworkTransport.Disconnect(hostID, connectionID, out error);
            isConnected = false;
        }


        private void Update()
        {
            if (!isConnected) return;
            int recHostId;
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out
            channelId, recBuffer, bufferSize, out dataSize, out error);
            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:
                        break;

                    case NetworkEventType.ConnectEvent:
                        onMessageReceive?.Invoke($"You have been connected to server.");
                        //var bbb = MyLogin.ToCharArray();

                        //for (int i = 0; i < bbb.Length; i++)
                        //{
                        //    recBuffer[100+i] = ((byte)bbb[i]);
                        //}

                        //   onMessageReceive?.Invoke(MyLogin);
                        SendMessage(MyLogin);
                       // string message1 = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                      //  onMessageReceive?.Invoke(message1);
                        Debug.Log($"You have been connected to server.");
                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        onMessageReceive?.Invoke(message);
                        Debug.Log(message);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        isConnected = false;
                        onMessageReceive?.Invoke($"You have been disconnected from server.");
                        Debug.Log($"You have been disconnected from server.");
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out error);
            }
        }


        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length *
            sizeof(char), out error);
            if ((NetworkError)error != NetworkError.Ok) Debug.Log((NetworkError)error);
        }
    }
}