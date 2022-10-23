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
        public Action ClientIsConnectedAction;
        private const int MAX_CONNECTION = 10;
        private int _port = 0;
        private int _serverPort = 8888;
        private int _hostID;
        private int _reliableChannel;
        private int _connectionID;
        private byte _error;

        public string ClientLogin { get; internal set; }
        public bool IsConnected { get; private set; } = false;


        public void Connect()
        {
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
            _reliableChannel = cc.AddChannel(QosType.Unreliable);
            cc.SendDelay = 5;
            cc.ConnectTimeout = 200;
            HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
            _hostID = NetworkTransport.AddHost(topology, _port);
            _connectionID = NetworkTransport.Connect(_hostID, "127.0.0.1", _serverPort, 0, out _error);
            if ((NetworkError)_error == NetworkError.Ok)
            {
                IsConnected = true;
                ClientIsConnectedAction?.Invoke();
            }

            else
                Debug.Log((NetworkError)_error);
        }


        public void Disconnect()
        {
            if (!IsConnected) return;
            NetworkTransport.Disconnect(_hostID, _connectionID, out _error);
            IsConnected = false;
            ClientIsConnectedAction?.Invoke();
        }


        private void Update()
        {
            if (!IsConnected) return;
            int recHostId;
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out
                channelId, recBuffer, bufferSize, out dataSize, out _error);
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
                        // string message1 = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        //  onMessageReceive?.Invoke(message1);
                        SendMessage(ClientLogin);
                        Debug.Log($"You have been connected to server.");
                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        onMessageReceive?.Invoke(message);
                        Debug.Log(message);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        IsConnected = false;
                        onMessageReceive?.Invoke($"You have been disconnected from server.");
                        Debug.Log($"You have been disconnected from server.");
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                    bufferSize, out dataSize, out _error);
            }
        }


        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(_hostID, _connectionID, _reliableChannel, buffer, message.Length *
                sizeof(char), out _error);
            if ((NetworkError)_error != NetworkError.Ok) Debug.Log((NetworkError)_error);
        }
    }
}