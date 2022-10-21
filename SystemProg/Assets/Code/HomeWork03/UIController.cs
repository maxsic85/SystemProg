using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HoweWork03.NetworkServer;
using HomeWork03.NetworkClient;
using System.Threading.Tasks;
using System.Threading;

namespace HomeWork03.View
{
    [System.Obsolete]
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private Button buttonStartServer;
        [SerializeField]
        private Button buttonShutDownServer;
        [SerializeField]
        private Button buttonConnectClient;
        [SerializeField]
        private Button buttonDisconnectClient;
        [SerializeField]
        private Button buttonSendMessage;
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private TMP_InputField inputLogin;
        [SerializeField]
        private TextField textField;
        [SerializeField]
        private Server server;
        [SerializeField]
        private Client client;

        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private async void Start()
        {
            LockBtns(false);
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
            await GetLogin(_cancelToken);

            LockBtns(true);
            buttonStartServer.onClick.AddListener(() => StartServer());
            buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
            buttonConnectClient.onClick.AddListener(() => Connect());
            buttonDisconnectClient.onClick.AddListener(() => Disconnect());
            buttonSendMessage.onClick.AddListener(() => SendMessage());
            client.onMessageReceive += ReceiveMessage;
        }

        private void LockBtns(bool permissive)
        {
            buttonStartServer.enabled = permissive;
            buttonShutDownServer.enabled = permissive;
            buttonConnectClient.enabled = permissive;
            buttonDisconnectClient.enabled = permissive;
        }

        private async Task<string> GetLogin(CancellationToken cancellationToken)
        {
            while (inputLogin.text == "")
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log($"Операция прервана токеном.");
                    return "token";
                }
                await Task.Yield();

            }
            return inputField.text;
        }


        private void StartServer()
        {
            server.StartServer();

        }

        private void ShutDownServer()
        {
            server.ShutDownServer();
        }

        private void Connect()
        {
            client.Connect();
            client.MyLogin = inputLogin.text;
        }

        private void Disconnect()
        {
            client.Disconnect();
        }

        private void SendMessage()
        {
            client.SendMessage(inputField.text);
            inputField.text = "";
        }

        public void ReceiveMessage(object message)
        {
            textField.ReceiveMessage(message);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _cancelTokenSource.Cancel();
            }
        }


        private void OnDestroy()
        {

            _cancelTokenSource.Cancel();

        }
    }
}
