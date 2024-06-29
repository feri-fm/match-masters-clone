using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NetworkGame
{
    public class NetConfig : ScriptableObject
    {
        public string key => name;

        public NetGame gamePrefab;
        public NetPlayer playerPrefab;
        public NetClient clientPrefab;

        public NetNetworkManager networkManager { get; private set; }

        public void _Setup(NetNetworkManager networkManager)
        {
            this.networkManager = networkManager;
            Setup();
        }

        public virtual void Setup() { }

        public virtual NetRoom CreateRoom(NetworkConnectionToClient conn)
        {
            return new NetRoom();
        }

        public NetGame CreateGame(Guid id)
        {
            var game = networkManager._Instantiate(gamePrefab);
            game.GetComponent<NetworkMatch>().matchId = id;
            return game;
        }
        public NetPlayer CreatePlayer(Guid id)
        {
            var player = networkManager._Instantiate(playerPrefab);
            player.GetComponent<NetworkMatch>().matchId = id;
            return player;
        }
        public NetClient CreateClient(Guid id)
        {
            var client = networkManager._Instantiate(clientPrefab);
            client.GetComponent<NetworkMatch>().matchId = id;
            return client;
        }
    }
}
