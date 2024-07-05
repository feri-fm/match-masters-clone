using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 0)]
    public class Config : ScriptableObject
    {
        public string key => name;

        public GameOptions gameOptions;

        public Game gamePrefab;
        public Player playerPrefab;
        public Client clientPrefab;
        public Bot botPrefab;

        public Room CreateRoom()
        {
            return new Room();
        }
    }
}
