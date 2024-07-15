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

        public Hash128 lastHash;
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

            gameplay = new TwoPlayerGameplay();
            var options = config.gameOptions.JsonCopy();
            options.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            gameplay.Setup(gameplayViewPrefab, options);
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
            gameplay.ShowStartMessage();
            gameplay.onTrySwap += (a, b) =>
            {
                client.CmdSwap(
                    gameplay.GetHash().ToString(), a, b);
                gameplay.StartMove();
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

        [TargetRpc]
        private void TargetLoadGameplayData(NetworkConnectionToClient _, string gameplayDataJson)
        {
            var gameplayData = gameplayDataJson.FromJson<GameplayData>();
            gameplay.Load(gameplayData);
            gameplay.Evaluate();
            client.CmdEvaluatingFinished();
        }

        [TargetRpc]
        public async void TargetTrySwap(NetworkConnectionToClient _, string hash, Int2 a, Int2 b)
        {
            var myHash = gameplay.GetHash().ToString();
            if (myHash != hash)
            {
                Debug.Log("!!! client has wrong hash");
                networkManager.game.client.client.CmdRequestGameplayData();
            }
            else
            {
                var tile = gameplay.gameEntity.game.GetTileAt(a);
                if (tile != null)
                {
                    gameplayView.SetHandAt(tile.id);
                    await new WaitForSeconds(0.4f);
                }
                gameplay.StartMove();
                await gameplay.TrySwap(a, b);
            }
        }
    }
}