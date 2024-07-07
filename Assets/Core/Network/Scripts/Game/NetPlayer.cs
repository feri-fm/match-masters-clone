using System;
using Mirror;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetPlayer : NetBehaviour
    {
        [SyncVar] public Guid id;

        public NetClient client;
        public NetGame game;
        public RoomPlayer roomPlayer;

        public bool hasClient => client != null;

        public void SetupClient(NetClient client)
        {
            this.client = client;
        }
        public void Setup(NetGame game, RoomPlayer roomPlayer)
        {
            id = Guid.NewGuid();
            this.game = game;
            this.roomPlayer = roomPlayer;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            networkManager.game._AddPlayer(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            networkManager.game._RemovePlayer(this);
        }

    }
}