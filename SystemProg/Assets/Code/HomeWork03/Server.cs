using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



namespace HoweWork03.NetworkServer
{

    [System.Obsolete]
    public class Server : MonoBehaviour
    {
        public Action ServerIsStartedAction;
        private const int MAX_CONNECTION = 10;
        private int _port = 8888;
        private int _hostID;
        private int _reliableChannel;
        private bool _isStarted = false;
        private byte _error;
        private Dictionary<int, string> _users = new Dictionary<int, string>();
        private List<int> _connectionIDs = new List<int>();

        public bool IsStarted { get => _isStarted; set => _isStarted = value; }


        public void StartServer()
        {
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
            cc.SendDelay = 5;
            cc.ConnectTimeout = 200;

            _reliableChannel = cc.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
            _hostID = NetworkTransport.AddHost(topology, _port);
            IsStarted = true;
            ServerIsStartedAction?.Invoke();
        }


        public void ShutDownServer()
        {
            if (!IsStarted) return;
            _users.Clear();
            NetworkTransport.RemoveHost(_hostID);
            NetworkTransport.Shutdown();
            IsStarted = false;
            ServerIsStartedAction?.Invoke();
        }


        public void SendMessage(string message, int connectionID)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(_hostID, connectionID, _reliableChannel, buffer, message.Length *
            sizeof(char), out _error);
            if ((NetworkError)_error != NetworkError.Ok) Debug.Log((NetworkError)_error);
        }


        public void SendMessageToAll(string message)
        {
            for (int i = 0; i < _connectionIDs.Count; i++)
            {
                SendMessage(message, _connectionIDs[i]);
            }
        }


        private void Update()
        {
            if (!IsStarted) return;
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
                        _connectionIDs.Add(connectionId);
                        string message1 = GetMessageByType(recBuffer, dataSize);
                        _users.Add(connectionId, message1);
                        Debug.Log($" {_users[connectionId]} has connected.");
                        break;
                    case NetworkEventType.DataEvent:

                        string message = GetMessageByType(recBuffer, dataSize);
                        SendMessageToAll($" {_users[connectionId]}: {message}");
                        AddNewUser(connectionId, message);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        RemoveUser(connectionId);
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out _error);
            }
        }

        private string GetMessageByType(byte[] recBuffer, int dataSize)
        {
            var type = Encoding.Unicode.GetString(recBuffer, 0, 2);
            switch (type)
            {

                case "1":
                    Debug.Log($"Login");
                    break;

                case "2":
                    Debug.Log($"Text");
                    break;

                default:
                    return "";
    
            }
            return Encoding.Unicode.GetString(recBuffer, 2, dataSize - 2);
        }

        private void RemoveUser(int connectionId)
        {
            _users.Remove(connectionId);
            _connectionIDs.Remove(connectionId);
            SendMessageToAll($"Player {connectionId} has disconnected.");
            Debug.Log($"Player {connectionId} has disconnected.");
        }


        private void AddNewUser(int connectionId, string message)
        {
            if (_users[connectionId] == "")
            {
                _users[connectionId] = message;
                SendMessageToAll($" {_users[connectionId]} has connected.");
            }
            Debug.Log($"Player {connectionId}: {_users[connectionId]}");
        }


        private void OnDestroy()
        {
            ShutDownServer();
        }
    }
}