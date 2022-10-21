using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



namespace HoweWork03.NetworkServer
{
    [System.Obsolete]
    public class Server : MonoBehaviour
    {
        private const int MAX_CONNECTION = 10;
        private int port = 8888;
        private int hostID;
        private int reliableChannel;
        private bool isStarted = false;
        private byte error;

        private Dictionary<int, string> _users = new Dictionary<int, string>();
        private  List<int> connectionIDs = new List<int>();


        public void StartServer()
        {
            NetworkTransport.Init();
            ConnectionConfig cc = new ConnectionConfig();
            cc.SendDelay = 5;
            cc.ConnectTimeout=200;

            reliableChannel = cc.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
            hostID = NetworkTransport.AddHost(topology, port);
            isStarted = true;
        }


          public void ShutDownServer()
        {
            if (!isStarted) return;
            NetworkTransport.RemoveHost(hostID);
            NetworkTransport.Shutdown();
            isStarted = false;
        }


     
        public void SendMessage(string message, int connectionID)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length *
            sizeof(char), out error);
            if ((NetworkError)error != NetworkError.Ok) Debug.Log((NetworkError)error);
        }


           public void SendMessageToAll(string message)
        {
            for (int i = 0; i < connectionIDs.Count; i++)
            {
                SendMessage(message, connectionIDs[i]);
            }
        }

       
        private void Update()
        {
            if (!isStarted) return;
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
                        connectionIDs.Add(connectionId);
                        SendMessageToAll($"Player {connectionId} has connected.");
                    //    SendMessageToAll($"Player {connectionId} has connected.");
                        Debug.Log($"Player {connectionId} has connected.");
                  //      string message1 = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                  //      _users.Add(connectionId, message1);
                     //   Debug.Log($"Player {connectionId} has connected.");

                        break;
                    case NetworkEventType.DataEvent:
                        string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        SendMessageToAll($"Player {connectionId}: {message}");
                        Debug.Log($"Player {connectionId}: {message}");
                        break;
                    case NetworkEventType.DisconnectEvent:
                        connectionIDs.Remove(connectionId);
                        SendMessageToAll($"Player {connectionId} has disconnected.");
                        Debug.Log($"Player {connectionId} has disconnected.");
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out error);
            }
        }
    }
}