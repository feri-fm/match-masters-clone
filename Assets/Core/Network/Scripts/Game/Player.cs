using System;
using Mirror;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class Player : NetBehaviour
    {
        [SyncVar] public Guid id;

        public Client client;
        public Game game;
        public RoomPlayer roomPlayer;

        public bool hasClient => client != null;

        public void SetupClient(Client client)
        {
            this.client = client;
        }
        public void Setup(Game game, RoomPlayer roomPlayer)
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