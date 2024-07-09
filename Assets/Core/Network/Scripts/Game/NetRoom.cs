using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MMC.Network.SessionMiddleware;

namespace MMC.Network.GameMiddleware
{
    public class NetRoom
    {
        public Guid id;
        public NetConfig config;
        public List<RoomPlayer> players = new();

        public void Setup(NetConfig config)
        {
            id = Guid.NewGuid();
            this.config = config;
        }

        public bool CanJoin(RoomPlayer player)
        {
            return players.Count < 2 && player.config == config.key;
        }

        public void Join(RoomPlayer player)
        {
            players.Add(player);
        }
        public void Leave(Guid id)
        {
            var player = players.Find(e => e.data.id == id);
            Leave(player);
        }
        public void Leave(NetworkConnectionToClient conn)
        {
            var player = players.Find(e => e.hasSession && e.session.conn == conn);
            Leave(player);
        }
        public void Leave(RoomPlayer player)
        {
            players.Remove(player);
        }

        public bool CanLaunch()
        {
            return players.Count >= 2;
        }
        public NetGame Launch()
        {
            var game = UnityEngine.Object.Instantiate(config.gamePrefab);
            var gamePlayers = new List<NetPlayer>();
            game.id = id;
            for (int i = 0; i < players.Count; i++)
            {
                var player = UnityEngine.Object.Instantiate(config.playerPrefab);
                gamePlayers.Add(player);
                player.Setup(game, i, players[i]);
                if (players[i].hasSession)
                {
                    var client = UnityEngine.Object.Instantiate(config.clientPrefab);
                    client.Setup(player);
                    player.SetupClient(client);
                }
            }
            game.Setup(this, gamePlayers);

            NetworkServer.Spawn(game.gameObject);
            foreach (var player in gamePlayers)
            {
                NetworkServer.Spawn(player.gameObject);
                if (player.hasClient)
                {
                    NetworkServer.Spawn(player.client.gameObject);
                    NetworkServer.AddPlayerForConnection(player.client.session.conn, player.client.gameObject);
                }
            }

            return game;
        }

        public void SendUpdate()
        {
            SendMessage(new UpdateRoomClientMessage(players.Select(e => e.data).ToArray()));
        }

        public void SendMessage<T>(T message) where T : struct, NetworkMessage
        {
            foreach (var p in players)
            {
                if (p.hasSession)
                {
                    p.conn.Send(message);
                }
            }
        }
    }

    public class RoomPlayer
    {
        public string config;
        public RoomPlayerData data;
        public Session session;

        public bool hasSession => session != null;
        public NetworkConnectionToClient conn => session.conn;

        public RoomPlayer(string config, RoomPlayerData data)
        {
            this.config = config;
            this.data = data;
        }
        public RoomPlayer(string config, Session session)
        {
            this.config = config;
            this.session = session;
            data = new RoomPlayerData()
            {
                id = Guid.NewGuid(),
                username = session.user.username
            };
        }
    }
    public struct RoomPlayerData

    {
        public Guid id;
        public string username;
    }
}