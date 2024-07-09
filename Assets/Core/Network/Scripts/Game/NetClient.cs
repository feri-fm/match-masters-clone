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

        public void ClientAction(string hash, string afterHash, Action action)
        {
            if (game.lastHash.ToString() != hash.ToString())
            {
                Debug.Log("!!!! Invalid hash before move");
                game.SendGameplayData(session.conn);
            }
            else
            {
                action.Invoke();
                if (game.lastHash.ToString() != afterHash)
                {
                    Debug.Log("!!!! Invalid hash after moved");
                    game.SendGameplayData(session.conn);
                }
            }
        }

        [Command]
        public void CmdRequestGameplayStartData()
        {
            game.SendGameplayStartData(session.conn);
        }

        [Command]
        public void CmdSwap(string hash, string afterHash, Int2 a, Int2 b)
            => ClientAction(hash, afterHash, () => player.TrySwap(a, b));
    }
}