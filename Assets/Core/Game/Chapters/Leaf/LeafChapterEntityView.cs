using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game.Chapters.Leaf
{
    public class LeafChapterEntityView : ChapterEntityView<LeafChapterEntity>
    {
        public int minCount = 4;
        public LeafTileView tilePrefab;

        public override Entity CreateEntity() => new LeafChapterEntity();
    }
    public class LeafChapterEntity : ChapterEntity<LeafChapterEntityView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            var game = engine.GetEntity<GameEntity>().game;
            game.generationMap = (tile) =>
            {
                if (tile is ColoredTileView coloredTile && coloredTile.color == prefab.tilePrefab.color)
                {
                    var currentCount = game.CountTiles(e => e is LeafTile);
                    var neededCount = prefab.minCount - currentCount;
                    if (neededCount > 0)
                        return prefab.tilePrefab;
                }
                return tile;
            };
        }
    }
}