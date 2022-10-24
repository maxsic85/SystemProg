using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HoweWork03.NetworkServer;
using HomeWork03.NetworkClient;
using System.Threading.Tasks;
using System.Threading;
using System.Text;



namespace HomeWork03.View
{
    [System.Obsolete]
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button buttonStartServer;
        [SerializeField] private Button buttonShutDownServer;
        [SerializeField] private Button buttonConnectClient;
        [SerializeField] private Button buttonDisconnectClient;
        [SerializeField] private Button buttonSendMessage;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_InputField inputLogin;
        [SerializeField] private TextField textField;
        [SerializeField] private Server server;
        [SerializeField] private Client client;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;


        private async void Start()
        {
            await WaitForInputLogin();

            buttonStartServer.onClick.AddListener(() => StartServer());
            buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
            buttonConnectClient.onClick.AddListener(() => Connect());
            buttonDisconnectClient.onClick.AddListener(() => Disconnect());
            buttonSendMessage.onClick.AddListener(() => SendMessage());

            client.onMessageReceive += ReceiveMessage;
            client.ClientIsConnectedAction += LockClientsBtn;
            server.ServerIsStartedAction += LockServersBtn;
        }


        private async Task WaitForInputLogin()
        {
            LockBtnsOnStarT(false);
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;
            await GetLogin(_cancelToken);

            LockBtnsOnStarT(true);
        }


        private void LockBtnsOnStarT(bool permissive)
        {
            buttonStartServer.enabled = permissive;
            buttonShutDownServer.enabled = permissive;
            buttonConnectClient.enabled = permissive;
            buttonDisconnectClient.enabled = permissive;
        }


        private void LockServersBtn()
        {
            buttonStartServer.enabled = !server.IsStarted;
            buttonConnectClient.enabled = !server.IsStarted;
            buttonDisconnectClient.enabled = !server.IsStarted;
        }


        private void LockClientsBtn()
        {
            buttonStartServer.enabled = !client.IsConnected;
            buttonConnectClient.enabled = !client.IsConnected;
            buttonShutDownServer.enabled = !client.IsConnected;
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
            client.ClientLogin = inputLogin.text;
        }


        private void Disconnect()
        {
            client.Disconnect();
        }


        private void SendMessage()
        {
            var textmesage = new Message(inputField.text, MessageType.MESSAGE);
            client.SendMessage(textmesage);
            inputField.text = "";
            textmesage.Clear();
        }


        public void ReceiveMessage(object message)
        {
            textField.textObject.color = (message.ToString().Contains("LOGIN")) ? Color.red : Color.green;

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
            client.onMessageReceive -= ReceiveMessage;
            server.ServerIsStartedAction -= LockServersBtn;
        }
    }
}