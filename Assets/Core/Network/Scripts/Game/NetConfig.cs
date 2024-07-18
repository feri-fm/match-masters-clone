using System.Collections.Generic;
using MMC.Game;
using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [CreateAssetMenu(fileName = "NetGameConfig", menuName = "NetGame/NetGameConfig")]
    public class NetConfig : ScriptableObject
    {
        public string key;

        public GameOptions gameOptions;

        public List<Chapter> chapters;

        public NetGame gamePrefab;
        public NetPlayer playerPrefab;
        public NetClient clientPrefab;
        public NetBot botPrefab;

        public NetRoom CreateRoom()
        {
            return new NetRoom();
        }
    }
}
