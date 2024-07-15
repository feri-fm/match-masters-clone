using Mirror;
using UnityEngine;

namespace MMC.Network
{
    public class NetworkOverlay : MonoBehaviour
    {
        public TextMember status;

        public NetNetworkManager networkManager => NetNetworkManager.instance;

        private void Start()
        {
            if (!Application.isEditor)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            status.text = networkManager.GetStatus();
        }
    }
}