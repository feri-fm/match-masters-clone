using UnityEngine;
using Core;

namespace Match3
{
    using System;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public MatchConfig match;
        public CellView cell;
        public BeadTileView[] beads;
        public LifetimeEffect[] effects;
        public ColoredTileView[] rewardTiles;

        public BeadTileView GetBeadTile(TileColor color)
        {
            return beads.First(e => e.color == color);
        }
        public T GetRewardTile<T>(TileColor color) where T : ColoredTileView
        {
            return rewardTiles.First(e => e is T && e.color == color) as T;
        }
        public T GetRewardTile<T>(TileColor color, Func<T, bool> check) where T : ColoredTileView
        {
            return rewardTiles.First(e => e is T && e.color == color && check.Invoke(e as T)) as T;
        }
        public LifetimeEffect GetEffect(TileColor color)
        {
            return effects.First(e => e.color == color);
        }
    }
}
