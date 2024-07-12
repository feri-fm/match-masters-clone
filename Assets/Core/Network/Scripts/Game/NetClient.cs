using System;
using Mirror;
using MMC.EngineCore;
using MMC.Network.SessionMiddleware;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetClient : NetBehaviour
    {
        [SyncVar] public Guid id;

        public NetPlayer player;
        public RoomPlayer roomPlayer => player.roomPlayer;
        public Session session => roomPlayer.session;
        public NetGame game => player.game;

        public void Setup(NetPlayer player)
        {
            this.player = player;
            id = player.id;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            networkManager.game._AddClient(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            networkManager.game._RemoveClient(this);
        }

        public void ServerAction(string hash, Action action)
        {
            if (game.lastHash.ToString() != hash.ToString())
            {
                Debug.Log("!!!! Invalid hash before move");
                game.SendGameplayData(session.conn);
            }
            else
            {
                action.Invoke();
            }
        }

        [Command]
        public void CmdRequestGameplayStartData()
        {
            game.SendGameplayStartData(session.conn);
        }

        [Command]
        public void CmdRequestGameplayData()
        {
            game.SendGameplayData(session.conn);
        }

        [Command]
        public void CmdSwap(string hash, Int2 a, Int2 b)
            => ServerAction(hash, () => player.TrySwap(a, b));
    }
}