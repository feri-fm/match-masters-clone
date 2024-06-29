using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    public class NetGameManager : MonoBehaviour
    {
        public NetGame game { get; private set; }
        public NetPlayer player { get; private set; }
        public NetClient client { get; private set; }
        public List<NetPlayer> players { get; private set; } = new List<NetPlayer>();
        public List<NetClient> clients { get; private set; } = new List<NetClient>();

        private bool dirtyFetch = false;
        private bool gameStarted = false;

        public static NetGameManager instance { get; private set; }

        public virtual void Awake()
        {
            instance = this;
        }

        public virtual void Start() { }
        public virtual void Update() { }

        private void LateUpdate()
        {
            if (dirtyFetch)
            {
                dirtyFetch = false;
                FetchConnections();
            }
        }

        public virtual void OnGameStart() { }
        public virtual void OnGameStop() { }

        public virtual void OnClearConnection()
        {
            if (NetworkServer.active)
            {
                foreach (var player in players.ToArray())
                {
                    player.OnStopClient();
                    Destroy(player.gameObject);
                }
                foreach (var client in clients.ToArray())
                {
                    client.OnStopClient();
                    Destroy(client.gameObject);
                }
                var g = game;
                if (g != null)
                {
                    g.OnStopClient();
                    Destroy(g.gameObject);
                }
            }
        }

        public void _SetGame(NetGame game)
        {
            this.game = game;
            MarkDirtyFetch();
        }
        public void _RemoveGame(NetGame game)
        {
            if (this.game == game)
                this.game = null;
            MarkDirtyFetch();
        }
        public void _AddPlayer(NetPlayer player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            MarkDirtyFetch();
        }
        public void _RemovePlayer(NetPlayer player)
        {
            players.Remove(player);
            MarkDirtyFetch();
        }
        public void _AddNetClient(NetClient player)
        {
            if (!clients.Contains(player))
            {
                clients.Add(player);
            }
            MarkDirtyFetch();
        }
        public void _RemoveNetClient(NetClient player)
        {
            clients.Remove(player);
            MarkDirtyFetch();
        }

        public void MarkDirtyFetch()
        {
            dirtyFetch = true;
        }

        public void FetchConnections()
        {
            // if (game != null)
            // {
            //     game.players.Clear();
            //     foreach (var id in game.playersId)
            //     {
            //         var player = players.Find(e => e.id == id);
            //         if (player != null)
            //         {
            //             game.players.Add(player);
            //         }
            //     }
            //     client = clients.Find(e => e.isLocalPlayer);
            //     if (client != null)
            //     {
            //         player = players.Find(e => e.id == client.id);
            //     }
            //     foreach (var player in players)
            //     {
            //         player.game = game;
            //         player.client = clients.Find(e => e.id == player.id);
            //         if (player.client != null)
            //             player.client.player = player;
            //     }
            // }
            // if (game != null
            //     && player != null
            //     && client != null
            //     && players.Count == game.playersId.Count)
            // {
            //     if (!gameStarted)
            //     {
            //         gameStarted = true;
            //         OnGameStart();
            //     }
            // }
            // else if (gameStarted)
            // {
            //     gameStarted = false;
            //     OnGameStop();
            // }
        }
    }

    public class NetGameManager<TGame, TPlayer, TClient> : NetGameManager where TPlayer : NetPlayer where TGame : NetGame where TClient : NetClient
    {
        public new TGame game => base.game as TGame;
        public new TPlayer player => base.player as TPlayer;
        public new TClient client => base.client as TClient;
    }
}
