using Mirror;
using UnityEngine;

namespace MMC.Network
{
    public class NetworkOverlay : MonoBehaviour
    {
        public TextMember status;
        public GameObjectMember notStarted;
        public GameObjectMember started;

        public NetNetworkManager networkManager => NetNetworkManager.instance;

        private void Update()
        {
            status.text = networkManager.GetStatus();
            started.SetActive(NetworkServer.active);
            notStarted.SetActive(!NetworkServer.active);
        }

        [Member]
        public void StartServer()
        {
            networkManager.StartServer();
        }

        [Member]
        public void StopServer()
        {
            networkManager.StopServer();
        }
    }
}