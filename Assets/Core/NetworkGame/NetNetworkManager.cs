using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    public partial class NetNetworkManager : NetworkManager
    {
        public static NetNetworkManager instance { get; private set; }

        public override void Awake()
        {
            instance = this;
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
            foreach (var config in configs)
            {
                config._Setup(this);
            }
        }

        public string GetStatus()
        {
            if (NetworkServer.active && NetworkClient.active)
                return $"<b>Host</b>: running via {Transport.active}";
            else if (NetworkServer.active)
                return $"<b>Server</b>: running via {Transport.active}";
            else if (NetworkClient.isConnected)
                return $"<b>Client</b>: connected to {networkAddress} via {Transport.active}";
            else if (NetworkClient.active)
                return $"<b>Client</b>: connecting to {networkAddress}...";
            return "Not connected";
        }

        public T _Instantiate<T>(T prefab) where T : Object
        {
            return Instantiate(prefab);
        }
    }
}
