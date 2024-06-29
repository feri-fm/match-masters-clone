using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NetworkGame
{
    public class NetRoom
    {
        public Dictionary<NetworkConnectionToClient, NetRoomPlayer> playersByConn { get; } = new Dictionary<NetworkConnectionToClient, NetRoomPlayer>();
        public NetConfig config { get; private set; }
        public Guid id { get; private set; }

        public Dictionary<string, EmitterAction> emitters = new Dictionary<string, EmitterAction>();
        public Dictionary<string, UnityAction<NetRoomPlayer, JObject>> receivers = new Dictionary<string, UnityAction<NetRoomPlayer, JObject>>();

        public NetNetworkManager networkManager { get; private set; }

        private int lastId;

        public delegate object EmitterAction();

        public void _Setup(NetConfig config, NetNetworkManager networkManager)
        {
            id = Guid.NewGuid();
            this.config = config;
            this.networkManager = networkManager;
            Setup();
        }

        public void _Update()
        {
            Update();
        }

        public virtual void Setup() { }
        public virtual void Update() { }
        public virtual void OnLaunched() { }
        public virtual void OnJoined(NetRoomPlayer player) { }
        public virtual void OnLeaved(NetRoomPlayer player) { }

        public virtual bool CanJoin(NetworkConnectionToClient conn)
        {
            return true;
        }

        public virtual void Join(NetworkConnectionToClient conn)
        {
            var player = new NetRoomPlayer();
            var id = lastId++;
            player.Setup(id.ToString(), this, conn);
            playersByConn[conn] = player;
            conn.Send(new JoinRoomClientMessage());
            OnJoined(player);
        }

        public virtual void Leave(NetworkConnectionToClient conn)
        {
            if (playersByConn.TryGetValue(conn, out var player))
            {
                playersByConn.Remove(conn);
                OnLeaved(player);
            }
        }

        public virtual void CreateGamePlayers(List<NetPlayer> gamePlayers)
        {
            // Example implementation
            // foreach (var item in playersByConn)
            // {
            //     var player = CreateConnectedGamePlayer(item.Value);
            //     gamePlayers.Add(player);
            // }
        }

        public virtual NetPlayer CreateConnectedGamePlayer(NetRoomPlayer roomPlayer)
        {
            var player = CreateGamePlayer();
            var client = config.CreateClient(id);
            NetworkServer.AddPlayerForConnection(roomPlayer.conn, client.gameObject);
            client._Setup(roomPlayer.conn);
            // player.netIdentity.AssignClientAuthority(roomPlayer.conn);
            player._SetupClient(client);
            return player;
        }
        public virtual NetPlayer CreateGamePlayer()
        {
            var player = config.CreatePlayer(id);
            player._Setup();
            return player;
        }

        public virtual void _UpdateData(NetworkConnectionToClient conn, UpdateRoomServerMessage msg)
        {
            var player = playersByConn[conn];
            if (receivers.TryGetValue(msg.action, out var receiver))
            {
                receiver.Invoke(player, JObject.Parse(msg.data));
            }
        }

        public void Emit(string action)
        {
            object data = new { };
            if (emitters.TryGetValue(action, out var emitter))
            {
                data = emitter.Invoke();
            }
            foreach (var item in playersByConn)
            {
                var msg = new UpdateRoomClientMessage(action, item.Value.id, data);
                item.Value.conn.Send(msg);
            }
        }

        public void Remove()
        {
            networkManager.RemoveRoom(this);
        }

        public void Clear()
        {
            foreach (var player in playersByConn)
            {
                networkManager.ClearConnection(player.Value.conn);
            }
            _Update();
            Remove();
        }

        public virtual void Launch()
        {
            OnLaunched();
            var game = config.CreateGame(id);
            var players = new List<NetPlayer>();
            CreateGamePlayers(players);
            game._Setup(this, players);
            Remove();
            networkManager.LaunchGame(game);
        }

        public virtual object GetLobbyData()
        {
            return new { };
        }

        public NetRoomData _GetLobbyData()
        {
            return new NetRoomData()
            {
                id = id,
                players = playersByConn.Count,
                data = GetLobbyData().ToJson(),
            };
        }
    }

    public struct NetRoomData
    {
        public Guid id;
        public int players;
        public string data;

        public JObject GetData()
        {
            return JObject.Parse(data);
        }
    }

    public class NetRoomPlayer
    {
        public string id;
        public NetRoom room;
        public NetworkConnectionToClient conn;

        public void Setup(string id, NetRoom room, NetworkConnectionToClient conn)
        {
            this.id = id;
            this.room = room;
            this.conn = conn;
        }
    }
}
