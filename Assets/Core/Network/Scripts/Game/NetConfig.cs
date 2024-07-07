using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 0)]
    public class NetConfig : ScriptableObject
    {
        public string key => name;

        public GameOptions gameOptions;

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
