using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    [RequireComponent(typeof(NetworkMatch))]
    public class NetGame : NetBehaviour
    {
        public Guid id { get; private set; }
        public NetRoom room { get; private set; }
        public NetConfig config => room.config;
        public List<NetPlayer> players { get; private set; } = new List<NetPlayer>();

        public bool valid { get; private set; }

        public NetNetworkManager networkManager => NetNetworkManager.instance;

        public NetGameManager game => NetGameManager.instance;

        public void _Setup(NetRoom room, List<NetPlayer> players)
        {
            id = Guid.NewGuid();
            valid = true;
            this.room = room;
            this.players = players;
            for (int i = 0; i < players.Count; i++)
            {
                players[i]._Setup(this, i);
            }
            Setup();
        }
        public void _Spawned()
        {
            OnSpawned();
        }

        public virtual void Setup() { }
        public virtual void OnSpawned() { }
        public virtual void StartGame() { }

        public override void OnStartClient()
        {
            base.OnStartClient();
            // config = networkManager.configs.Find(e => e.key == configKey);
            game._SetGame(this);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            game._RemoveGame(this);
        }

        public virtual void OnLeave(NetClient client)
        {

        }

        public void Destroy()
        {
            valid = false;
            networkManager.RemoveGame(this);
            foreach (var player in players)
            {
                player.Destroy();
            }
            NetworkServer.Destroy(gameObject);
            players.Clear();
        }
    }

    public class NetGame<TRoom, TConfig, TPlayer> : NetGame where TRoom : NetRoom where TConfig : NetConfig where TPlayer : NetPlayer
    {
        public new TConfig config => base.config as TConfig;
        public new TRoom room => base.room as TRoom;

        private List<TPlayer> _players = new List<TPlayer>();
        public new List<TPlayer> players => _players.Count > 0 ? _players : _LoadPlayers();

        public List<TPlayer> _LoadPlayers()
        {
            _players.Clear();
            _players.AddRange(base.players.ConvertAll(e => e as TPlayer));
            return _players;
        }
    }
}
