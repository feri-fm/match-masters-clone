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

        public Hash128 lastHash;

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

        public void StartGameClient()
        {
            StartGameClient();
            config = networkManager.game.configs.Find(e => e.key == configKey);
            networkManager.game._SetGame(this);

            gameplay = new TwoPlayerGameplay();
            gameplayView = ObjectPool.global.Spawn(gameplayViewPrefab, transform);

            gameplayView.Setup(gameplay);
            gameplay.Setup(gameplayViewPrefab, config.gameOptions);
            gameplay.onTrySwap += (a, b) =>
            {
                var hash = gameplay.GetHash();
                var fast = gameplay.GetFastGameplay();
                fast.TrySwap(a, b);
                var afterHash = fast.GetHash();
                networkManager.game.client.CmdSwap(
                    hash.ToString(), afterHash.ToString(), a, b);
            };
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            networkManager.game._RemoveGame(this);
        }

        public void Leave(NetworkConnectionToClient conn)
        {
            var player = players.FirstOrDefault(e => e.hasClient && e.client.session.conn == conn);
            if (player != null)
            {
                var client = player.client;
                player.client = null;
                //TODO: activate bot here
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
        }
        [TargetRpc]
        public void TargetTrySwap(NetworkConnectionToClient _, Int2 a, Int2 b)
        {
            gameplay.TrySwap(a, b);
        }
    }
}