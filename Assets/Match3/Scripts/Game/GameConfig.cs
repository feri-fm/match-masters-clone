using UnityEngine;
using Core;

namespace Match3
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int width = 7;
        public int height = 7;
        public CellView cell;
        public BeadTileView[] beads;
        public MatchConfig match;
    }
}
