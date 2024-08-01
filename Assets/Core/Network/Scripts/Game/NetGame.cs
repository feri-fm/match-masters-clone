using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using MMC.EngineCore;
using MMC.Game;
using MMC.Match3;
using MMC.Server;
using MMC.Server.Models;
using MongoDB.Driver;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetGame : NetBehaviour
    {
        [SyncVar] public Guid id;
        [SyncVar] public string configKey;
        [SyncVar] public int presetIndex;

        public float clientDelay = 0.3f;

        public EngineConfig engineConfig;
        public NetConfig.GamePreset preset;

        public TwoPlayerGameplayView gameplayViewPrefab => preset.gameplayViewPrefab;
        public Chapter chapter => preset.chapter;

        public TwoPlayerGameplayView gameplayView { get; private set; }
        public TwoPlayerGameplay gameplay { get; private set; }

        public readonly SyncList<Guid> playersId = new SyncList<Guid>();

        public NetRoom room { get; private set; }
        public NetConfig config { get; private set; }
        public List<NetPlayer> players { get; private set; } = new List<NetPlayer>();

        public GameManager gameManager => GameManager.instance;

        public NetClient client => networkManager.game.client.client;

        public string lastHash;
        public float lastEvaluateTime;

        public bool isFinished;
        public GameModel gameModel;

        private GameplayData startData;

        public async void Setup(NetRoom room, List<NetPlayer> players)
        {
            this.room = room;
            this.players = players;
            config = room.config;
            configKey = config.key;

            presetIndex = UnityEngine.Random.Range(0, config.presets.Count);
            preset = config.presets[presetIndex];

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
            chapter.Apply(gameplay);
            startData = gameplay.Save();
            gameplay.Evaluate();


            lastHash = gameplay.GetHash();

            gameplay.onFinish += async () =>
            {
                isFinished = true;
                await gameModel.FinishGame(this);
                foreach (var player in players)
                {
                    if (player.hasClient && (gameModel.winnerIndex == -1 || player.index == gameModel.winnerIndex))
                    {
                        var user = player.client.session.user;
                        user.inventory.ChangeCount(player.booster.key, 1);
                        await user.Update(e => e.Inc(u => u.inventory.items[player.booster.key], 1));
                        networkManager.ServerEmit(player.client.session, "update-user", user);
                    }
                }
            };

            foreach (var player in players)
            {
                if (player.hasClient)
                {
                    var user = player.client.session.user;
                    user.inventory.ChangeCount(player.booster.key, -1);
                    user.inventory.ChangeCount(player.perks[0].key, -1);
                    user.inventory.ChangeCount(player.perks[1].key, -1);
                    await user.Update(e =>
                    {
                        var res = e.Inc(u => u.inventory.items[player.booster.key], -1);
                        if (user.inventory.HasItem(player.perks[0].key))
                            res = res.Inc(u => u.inventory.items[player.perks[0].key], -1);
                        if (user.inventory.HasItem(player.perks[1].key))
                            res = res.Inc(u => u.inventory.items[player.perks[1].key], -1);
                        return res;
                    });
                    networkManager.ServerEmit(player.client.session, "update-user", user);
                }
            }

            var games = networkManager.serverManager.app.Find<GamesService>();

            gameModel = await games.CreateNewGame(this);
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
            preset = config.presets[presetIndex];

            gameplay = new TwoPlayerGameplay();
            gameplayView = Instantiate(gameplayViewPrefab, transform);

            gameplayView.Setup(gameplay);
            gameplay.Setup(gameplayViewPrefab, config.gameOptions);
            if (networkManager.game.client.player.index != 0)
                gameplay.SetIsOpponent();
            gameplay.myPlayer.Setup(gameplay, players[gameplay.MyIndex()].booster, players[gameplay.MyIndex()].perks);
            gameplay.opponentPlayer.Setup(gameplay, players[gameplay.OpponentIndex()].booster, players[gameplay.OpponentIndex()].perks);
            gameplay.ShowStartMessage();
            gameplay.Changed();
            gameplayView.ForceRender();

            gameplay.onTrySwap += (a, b) =>
            {
                client.CmdSwap(gameplay.GetHash(), a, b);
                gameplay.StartMove();
            };
            gameplay.onTryUseBooster += (reader) =>
            {
                var hash = gameplay.GetHash();
                client.CmdUseBooster(hash, reader.Save());
            };
            gameplay.onTryUsePerk += (index, reader) =>
            {
                var hash = gameplay.GetHash();
                client.CmdUsePerk(hash, reader.Save(), index);
            };
            gameplay.onEvaluatingFinished += () =>
            {
                client.CmdEvaluatingFinished();
            };

            gameplay.onFinish += () =>
            {
                //TODO: show game results, who wins, who loses
                gameManager.finishGamePanel.OpenPanel();
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

        public async void Destroy()
        {
            if (!isFinished)
            {
                await gameModel.CancelGame(this);
            }

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
            var hash = Hash128.Compute(gameplayDataJson);
            var gameplayData = gameplayDataJson.FromJson<GameplayData>();
            gameplay.Load(gameplayData);
            gameplay.Evaluate();
            client.CmdEvaluatingFinished();

            Debug.Log("Gameplay data loaded from server");
        }

        [TargetRpc]
        public async void TargetTrySwap(NetworkConnectionToClient _, string hash, Int2 a, Int2 b)
        {
            var tile = gameplay.gameEntity.game.GetTileAt(a);
            if (tile != null)
            {
                gameplayView.SetHandAt(tile.id);
                await new WaitForSeconds(clientDelay);
                TargetAction(hash, async () =>
                {
                    gameplay.StartMove();
                    await gameplay.TrySwap(a, b);
                });
            }
        }

        [TargetRpc]
        public async void TargetUseBooster(NetworkConnectionToClient _, string hash, string reader)
        {
            await new WaitForSeconds(clientDelay);
            TargetAction(hash, async () =>
            {
                await gameplay.GetCurrentPlayer().UseBooster(GameplayReader.From(reader), true);
            });
        }

        [TargetRpc]
        public async void TargetUsePerk(NetworkConnectionToClient _, string hash, string reader, int index)
        {
            await new WaitForSeconds(clientDelay);
            TargetAction(hash, async () =>
            {
                await gameplay.GetCurrentPlayer().UsePerk(index, GameplayReader.From(reader), true);
            });
        }
    }
}