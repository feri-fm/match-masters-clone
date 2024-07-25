using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game.Chapters.Cowboy
{
    public class CowboyChapterEntityView : ChapterEntityView<CowboyChapterEntity>
    {
        public int minCount = 4;
        public CowboyTileView tilePrefab;

        public override Entity CreateEntity() => new CowboyChapterEntity();
    }
    public class CowboyChapterEntity : ChapterEntity<CowboyChapterEntityView>
    {
        [JsonDataInt] public int first = 0;

        protected override void OnSetup()
        {
            base.OnSetup();
            var game = engine.GetEntity<GameEntity>().game;
            game.onEvaluatingFinished += () =>
            {
                if (first == 0)
                {
                    first = 1;
                    Do(game);
                }
            };
            engine.events.Listen(this, "turn", (evt) =>
            {
                Do(game);
            });
        }

        public override void Load(JsonData data)
        {
            base.Load(data);
        }

        private void Do(Match3.Game game)
        {
            var currentCount = game.CountTiles(e => e is CowboyTile);
            var neededCount = prefab.minCount - currentCount;
            if (neededCount > 0)
            {
                game.ScanRandom(neededCount, e => e is BeadTile bead && bead.color == prefab.tilePrefab.color, (tile) =>
                {
                    game.engine.RemoveEntity(tile);
                    var newTile = game.CreateTile(prefab.tilePrefab);
                    game.SetTileAt(tile.position, newTile);
                    newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                });
            }
        }
    }
}