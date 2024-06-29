using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetClient : NetBehaviour
    {
        public NetPlayer player { get; set; }

        public NetworkConnectionToClient conn { get; private set; }

        public NetGameManager gameManager => NetGameManager.instance;
        public NetNetworkManager networkManager => NetNetworkManager.instance;

        public void _Setup(NetworkConnectionToClient conn)
        {
            this.conn = conn;
        }
        public void _SetupPlayer(NetPlayer player)
        {
            this.player = player;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            gameManager._AddNetClient(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            gameManager._RemoveNetClient(this);
        }

        public void Destroy()
        {
            NetworkServer.RemovePlayerForConnection(connectionToClient, RemovePlayerOptions.Destroy);
        }
    }

    public class NetClient<TGame, TPlayer> : NetClient where TGame : NetGame where TPlayer : NetPlayer
    {
        public TGame game => base.player.game as TGame;
        public new TPlayer player => base.player as TPlayer;
    }
}
