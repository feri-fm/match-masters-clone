using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetPlayer : NetBehaviour
    {
        [SyncVar]
        public Guid id;

        [SyncVar]
        public int index;

        public bool hasClient => client != null;

        public NetClient client { get; set; }
        public NetGame game { get; set; }

        public NetGameManager gameManager => NetGameManager.instance;
        public NetNetworkManager networkManager => NetNetworkManager.instance;

        public void _Setup()
        {
            id = Guid.NewGuid();
        }

        public void _SetupClient(NetClient client)
        {
            this.client = client;
            client._SetupPlayer(this);
        }

        public void _Setup(NetGame game, int index)
        {
            this.index = index;
            this.game = game;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            gameManager._AddPlayer(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            gameManager._RemovePlayer(this);
        }

        public void Disconnect()
        {
            if (client != null)
            {
                client.Destroy();
                client = null;
            }
        }

        public void Destroy()
        {
            NetworkServer.Destroy(gameObject);
            if (client != null)
                client.Destroy();
        }
    }

    public class NetPlayer<T> : NetPlayer where T : NetGame
    {
        public new T game => base.game as T;
    }
}
