using System.Linq;
using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    [System.Serializable]
    public class HatCommand : GameCommand
    {
        [JsonDataInt] public int count = 6;
        [JsonDataFloat] public float timeDelay = 0.1f;

        protected override async Task Run()
        {
            await game.ScanRandom(count, t => t is BeadTile, async tile =>
            {
                var bead = tile as BeadTile;
                var color = bead.color;
                var prefab = game.RandElement(game.config.rewardTiles.Where(e => e.color == color).ToArray());
                game.engine.RemoveEntity(bead);
                var newTile = game.CreateTile(prefab);
                game.SetTileAt(tile.position, newTile);
                newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                await game.Wait(timeDelay);
            });

            // var swapped = 0;
            // for (int i = 0; i < searchCount && swapped < targetCount; i++)
            // {
            //     var point = game.RandPoint();
            //     var tile = game.GetTileAt(point);
            //     if (tile != null && tile is BeadTile beadTile)
            //     {
            //         swapped++;
            //         var color = beadTile.color;
            //         var prefab = game.RandElement(game.config.rewardTiles.Where(e => e.color == color).ToArray());
            //         game.engine.RemoveEntity(beadTile);
            //         var newTile = game.CreateTile(prefab);
            //         game.SetTileAt(point, newTile);
            //         newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
            //         await game.Wait(timeDelay);
            //     }
            // }
            await game.Wait(0.4f);
        }
    }
}