using System;
using Mirror;
using MMC.EngineCore;
using MMC.Game;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetPlayer : NetBehaviour
    {
        [SyncVar] public Guid id;
        [SyncVar] public int index;

        public NetClient client;
        public NetGame game;
        public RoomPlayer roomPlayer;

        public bool hasClient => client != null;

        public TwoPlayerGameplay gameplay => game.gameplay;

        public void SetupClient(NetClient client)
        {
            this.client = client;
        }
        public void Setup(NetGame game, int index, RoomPlayer roomPlayer)
        {
            id = Guid.NewGuid();
            this.index = index;
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

        public void ServerAction(Action<NetGame> action)
        {
            game.PlayerAction(action);
        }
        public void OnOthers(Action<NetworkConnectionToClient> action)
        {
            foreach (var player in game.players)
            {
                if (player.hasClient && player.client != client)
                {
                    action.Invoke(player.client.session.conn);
                }
            }
        }

        public void TrySwap(Int2 a, Int2 b)
        {
            if (gameplay.IsTurn(index))
            {
                ServerAction(game =>
                {
                    var hash = gameplay.GetHash().ToString();
                    gameplay.StartMove();
                    gameplay.TrySwap(a, b);
                    OnOthers(conn =>
                    {
                        game.TargetTrySwap(conn, hash, a, b);
                    });
                });
            }
        }
    }
}