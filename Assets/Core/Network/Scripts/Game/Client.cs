using System;
using Mirror;
using MMC.EngineCore;
using MMC.Network.SessionMiddleware;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class Client : NetBehaviour
    {
        [SyncVar] public Guid id;

        public Player player;
        public RoomPlayer roomPlayer => player.roomPlayer;
        public Session session => roomPlayer.session;
        public Game game => player.game;

        public void Setup(Player player)
        {
            this.player = player;
            id = player.id;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            networkManager.game._AddClient(this);
            CmdRequestEngineData();
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            networkManager.game._RemoveClient(this);
        }

        [Command]
        public void CmdRequestEngineData()
        {
            game.SendEngineData(session.conn);
        }

        [Command]
        public void CmdSwap(Int2 a, Int2 b)
        {
            game.game.game.TrySwap(a, b);
            foreach (var player in game.players)
            {
                if (player.hasClient && player.client != this)
                {
                    game.TargetSwap(player.client.session.conn, a, b);
                }
            }
        }
    }
}