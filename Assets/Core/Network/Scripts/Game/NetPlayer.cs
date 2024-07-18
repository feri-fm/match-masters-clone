using System;
using System.Linq;
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
        [SyncVar] public RoomPlayerData playerData;

        public NetClient client;
        public NetGame game;
        public RoomPlayer roomPlayer;
        public Booster booster;
        public Perk[] perks;

        public bool hasClient => client != null;

        public TwoPlayerGameplay gameplay => game.gameplay;

        public TwoPlayerGameplayPlayer gameplayPlayer => index == 0 ? gameplay.myPlayer : gameplay.opponentPlayer;

        public GameManager gameManager => GameManager.instance;

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
            playerData = roomPlayer.data;

            booster = networkManager.config.GetBooster(playerData.booster);
            perks = playerData.perks.Select(e => networkManager.config.GetPerk(e)).ToArray();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            booster = gameManager.config.GetBooster(playerData.booster);
            perks = playerData.perks.Select(e => gameManager.config.GetPerk(e)).ToArray();
            networkManager.game.client._AddPlayer(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            networkManager.game.client._RemovePlayer(this);
        }

        public void TurnAction(Action<NetGame> action)
        {
            if (gameplayPlayer.isTurn)
            {
                game.PlayerAction(action);
            }
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
            TurnAction(game =>
            {
                var hash = gameplay.GetHash();
                gameplay.StartMove();
                gameplay.TrySwap(a, b);
                OnOthers(conn =>
                {
                    game.TargetTrySwap(conn, hash, a, b);
                });
            });
        }
        public void UseBooster(string reader)
        {
            TurnAction(game =>
            {
                var hash = gameplay.GetHash();
                _ = gameplayPlayer.UseBooster(GameplayReader.From(reader));
                OnOthers(conn =>
                {
                    game.TargetUseBooster(conn, hash, reader);
                });
            });
        }
        public void UsePerk(int index, string reader)
        {
            TurnAction(game =>
            {
                var hash = gameplay.GetHash();
                _ = gameplayPlayer.UsePerk(index, GameplayReader.From(reader));
                OnOthers(conn =>
                {
                    game.TargetUsePerk(conn, hash, reader, index);
                });
            });
        }
    }
}