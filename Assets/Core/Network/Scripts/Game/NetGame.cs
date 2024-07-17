using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using MMC.EngineCore;
using MMC.Game;
using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetGame : NetBehaviour
    {
        [SyncVar] public Guid id;
        [SyncVar] public string configKey;

        public TwoPlayerGameplayView gameplayViewPrefab;
        public EngineConfig engineConfig;

        public TwoPlayerGameplayView gameplayView;
        public TwoPlayerGameplay gameplay;

        public readonly SyncList<Guid> playersId = new SyncList<Guid>();

        public NetRoom room { get; private set; }
        public NetConfig config { get; private set; }
        public List<NetPlayer> players { get; private set; } = new List<NetPlayer>();

        public GameManager gameManager => GameManager.instance;

        public NetClient client => networkManager.game.client.client;

        public string lastHash;
        public float lastEvaluateTime;

        private GameplayData startData;

        public void Setup(NetRoom room, List<NetPlayer> players)
        {
            this.room = room;
            this.players = players;
            config = room.config;
            configKey = config.key;

            playersId.Clear();
            GetComponent<NetworkMatch>().matchId = id;
            foreach (var player in players)
            {
                player.GetComponent<NetworkMatch>().matchId = id;
                if (player.hasClient)
                    player.client.GetComponent<NetworkMatch>().matchId = id;
                playersId.Add(player.id);
            }

            var options = config.gameOptions.JsonCopy();
            options.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            gameplay = new TwoPlayerGameplay();
            gameplay.Setup(gameplayViewPrefab, options);
            gameplay.myPlayer.Setup(gameplay, players[0].booster, players[0].perks);
            gameplay.opponentPlayer.Setup(gameplay, players[1].booster, players[1].perks);
            startData = gameplay.Save();
            gameplay.Evaluate();
            lastHash = gameplay.GetHash();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            networkManager.game.client._SetGame(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            networkManager.game.client._RemoveGame(this);
        }

        public void StartGameClient()
        {
            config = networkManager.game.configs.Find(e => e.key == configKey);

            gameplay = new TwoPlayerGameplay();
            gameplayView = Instantiate(gameplayViewPrefab, transform);

            gameplayView.Setup(gameplay);
            gameplay.Setup(gameplayViewPrefab, config.gameOptions);
            gameplay.SetIsOpponent(networkManager.game.client.player.index != 0);
            gameplay.myPlayer.Setup(gameplay, players[gameplay.MyIndex()].booster, players[gameplay.MyIndex()].perks);
            gameplay.opponentPlayer.Setup(gameplay, players[gameplay.OpponentIndex()].booster, players[gameplay.OpponentIndex()].perks);
            gameplay.ShowStartMessage();
            gameplay.onTrySwap += (a, b) =>
            {
                client.CmdSwap(gameplay.GetHash(), a, b);
                gameplay.StartMove();
            };
            gameplay.onTryUseBooster += () =>
            {
                var hash = gameplay.GetHash();
                client.CmdUseBooster(hash);
            };
            gameplay.onTryUsePerk += (index) =>
            {
                var hash = gameplay.GetHash();
                client.CmdUsePerk(hash, index);
            };
            gameplay.onEvaluatingFinished += () =>
            {
                client.CmdEvaluatingFinished();
            };
        }

        public void Leave(NetworkConnectionToClient conn)
        {
            var player = players.FirstOrDefault(e => e.hasClient && e.client.session.conn == conn);
            if (player != null)
            {
                var client = player.client;
                player.client = null;
                var bot = Instantiate(config.botPrefab);
                bot.Setup(player);
                NetworkServer.DestroyPlayerForConnection(conn);
            }
        }

        public void Destroy()
        {
            foreach (var player in players)
            {
                if (player.hasClient)
                    NetworkServer.DestroyPlayerForConnection(player.client.session.conn);
                NetworkServer.Destroy(player.gameObject);
            }
            NetworkServer.Destroy(gameObject);
            players.Clear();
        }

        public void PlayerAction(Action<NetGame> action)
        {
            lastEvaluateTime = Time.time + 20;
            action.Invoke(this);
            lastHash = gameplay.GetHash();
        }

        public void SendGameplayData(NetworkConnectionToClient conn)
        {
            TargetLoadGameplayData(conn, gameplay.Save().ToJson());
        }
        public void SendGameplayStartData(NetworkConnectionToClient conn)
        {
            TargetLoadGameplayData(conn, startData.ToJson());
        }

        private void TargetAction(string hash, Action action)
        {
            var myHash = gameplay.GetHash();
            if (myHash != hash)
            {
                Debug.Log("!!! client has wrong hash");
                networkManager.game.client.client.CmdRequestGameplayData();
            }
            else
            {
                action.Invoke();
            }
        }

        [TargetRpc]
        private void TargetLoadGameplayData(NetworkConnectionToClient _, string gameplayDataJson)
        {
            var gameplayData = gameplayDataJson.FromJson<GameplayData>();
            gameplay.Load(gameplayData);
            gameplay.Evaluate();
            client.CmdEvaluatingFinished();
            Debug.Log("Gameplay data loaded from server");
        }

        [TargetRpc]
        public void TargetTrySwap(NetworkConnectionToClient _, string hash, Int2 a, Int2 b)
        {
            TargetAction(hash, async () =>
            {
                var tile = gameplay.gameEntity.game.GetTileAt(a);
                if (tile != null)
                {
                    gameplayView.SetHandAt(tile.id);
                    await new WaitForSeconds(0.4f);
                }
                gameplay.StartMove();
                await gameplay.TrySwap(a, b);
            });
        }

        [TargetRpc]
        public void TargetUseBooster(NetworkConnectionToClient _, string hash)
        {
            TargetAction(hash, async () =>
            {
                await gameplay.GetCurrentPlayer().UseBooster(true);
            });
        }

        [TargetRpc]
        public void TargetUsePerk(NetworkConnectionToClient _, string hash, int index)
        {
            TargetAction(hash, async () =>
            {
                await gameplay.GetCurrentPlayer().UsePerk(index, true);
            });
        }
    }
}