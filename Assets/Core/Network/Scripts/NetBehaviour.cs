using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace MMC.Network
{
    public class NetBehaviour : NetworkBehaviour
    {
        public new string name
        {
            get => _name;
            set => SetName(value);
        }

        private string _name;

        public NetNetworkManager networkManager => NetNetworkManager.instance;

        private void Awake()
        {
            SetName(base.name);
        }

        public override void OnStartClient() { base.OnStartClient(); UpdateName(); }
        public override void OnStopClient() { base.OnStopClient(); UpdateName(); }
        public override void OnStartServer() { base.OnStartServer(); UpdateName(); }
        public override void OnStopServer() { base.OnStopServer(); UpdateName(); }

        private void SetName(string value)
        {
            _name = value;
            UpdateName();
        }

        private void UpdateName()
        {
            if (isServer)
            {
                base.name = $"Server {_name}";
            }
            else if (isClient)
            {
                base.name = $"Client {_name}";
            }
            else
            {
                base.name = $"{_name}";
            }
        }
    }
}
