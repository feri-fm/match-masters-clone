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

        public EngineView engineViewPrefab;
        public EngineView engineView;
        public EngineConfig engineConfig;

        public Engine engine;
        public GameEntity game;

        public readonly SyncList<Guid> playersId = new SyncList<Guid>();

        public NetRoom room { get; private set; }
        public NetConfig config { get; private set; }
        public List<NetPlayer> players { get; private set; } = new List<NetPlayer>();

        public GameManager gameManager => GameManager.instance;

        public void Setup(NetRoom room, List<NetPlayer> players)
        {
            this.room = room;
            this.players = players;
            config = room.config;

            GetComponent<NetworkMatch>().matchId = id;
            foreach (var player in players)
            {
                player.GetComponent<NetworkMatch>().matchId = id;
                if (player.hasClient)
                    player.client.GetComponent<NetworkMatch>().matchId = id;
            }

            engine = new Engine(engineConfig);
            game = engine.CreateEntity("game") as GameEntity;
            var options = config.gameOptions.JsonCopy();
            options.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            game.Setup(options);

            engine.Evaluate();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            config = networkManager.game.configs.Find(e => e.key == configKey);
            networkManager.game._SetGame(this);

            engineView = Instantiate(engineViewPrefab, transform);
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

        public void SendEngineData(NetworkConnectionToClient conn)
        {
            TargetLoadEngineData(conn, engine.Save().ToJson());
        }
        [TargetRpc]
        private void TargetLoadEngineData(NetworkConnectionToClient conn, string engineDataJson)
        {
            var engineData = engineDataJson.FromJson<EngineData>();
            LoadEngine(engineData);
        }

        [TargetRpc]
        public void TargetSwap(NetworkConnectionToClient conn, Int2 a, Int2 b)
        {
            game.game.TrySwap(a, b);
        }

        public void LoadEngine(EngineData data)
        {
            if (engine != null)
                engine.Clear();

            engine = new Engine(engineConfig);
            engine.waiter = Wait;
            engineView.Setup(engine);
            engine.Load(data);
            engine.Evaluate();

            game = engine.GetEntity<GameEntity>();
            game.onSwap += (a, b) =>
            {
                gameManager.networkManager.game.client.CmdSwap(a, b);
            };
        }

        public Task Wait(float time)
        {
            var tcs = new TaskCompletionSource<byte>();
            StartCoroutine(IWait(time, () =>
            {
                tcs.SetResult(0);
            }));
            return tcs.Task;

            IEnumerator IWait(float time, Action callback)
            {
                yield return new WaitForSeconds(time);
                callback.Invoke();
            }
        }
    }
}