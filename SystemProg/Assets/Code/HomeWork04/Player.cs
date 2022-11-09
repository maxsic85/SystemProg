using System;
using UnityEngine;
using UnityEngine.Networking;


namespace HomeWork04.HLAPI
{
    public class Player : NetworkBehaviour
    {
        public Action OnDisconectFromServer;
        [SerializeField] private GameObject playerPrefab;
        private GameObject playerCharacter;
        private Transform _spawn;


        private void Start()
        {
            SpawnCharacter();
        }

        
        public override void OnNetworkDestroy()
        {
            OnDisconectFromServer?.Invoke();
            Debug.LogWarning("Client Disconnected");
        }

        
        [Client]
        public void OnClientDisconnected(NetworkConnection conn, NetworkReader reader)
        {
            Debug.LogWarning("Client Disconnected");
            OnDisconectFromServer?.Invoke();
        }

        
        public void SpawnCharacter()
        {
            if (!isServer)
            {
                return;
            }

            playerCharacter = Instantiate(playerPrefab);
            NetworkServer.SpawnWithClientAuthority(playerCharacter,
                connectionToClient);
        }
    }
}