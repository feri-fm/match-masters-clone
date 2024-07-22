using System.Collections.Generic;
using MMC.Game;
using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    public class NetConfig : MonoBehaviour
    {
        public string key => name;

        public GameOptions gameOptions;
        public int minTrophies;
        public List<GamePreset> presets;

        public NetGame gamePrefab;
        public NetPlayer playerPrefab;
        public NetClient clientPrefab;
        public NetBot botPrefab;

        public NetRoom CreateRoom()
        {
            return new NetRoom();
        }

        [System.Serializable]
        public class GamePreset
        {
            public TwoPlayerGameplayView gameplayViewPrefab;
            public Chapter chapter;
        }
    }
}
