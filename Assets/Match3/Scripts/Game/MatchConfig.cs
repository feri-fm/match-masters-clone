using UnityEngine;
using Core;

namespace Match3
{
    [CreateAssetMenu(fileName = "GameConfigMatch", menuName = "GameConfigMatch")]
    public class MatchConfig : ScriptableObject
    {
        public MatchPattern[] search;
        public MatchPattern[] patterns;
    }

    [System.Serializable]
    public class MatchPattern
    {
        public Reward reward;
        public int width = 5;
        public int height = 5;
        public Int2[] rewardPoints = new Int2[] { };
        public Int2[] points = new Int2[] { };

        public bool hasReward => rewardPoints.Length > 0;

        public enum Reward
        {
            None, Horizontal, Vertical, Bomb, Lightning
        }
    }
}
