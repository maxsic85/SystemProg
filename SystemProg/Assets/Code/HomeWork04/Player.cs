using UnityEngine;
using UnityEngine.Networking;


namespace HomeWork04.HLAPI
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        private GameObject playerCharacter;

        private void Start()
        {
            SpawnCharacter();
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



