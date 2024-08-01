using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    [System.Serializable]
    public class BucketCommand : GameCommand
    {
        [JsonDataInt] public int count = 6;
        [JsonDataFloat] public float timeDelay = 0.1f;
        [JsonDataInt] public int colorValue;

        protected override async Task Run()
        {
            // var color = game.config.beads[2 + game.RandInt(game.options.beads - 2)].color;
            var color = new TileColor(colorValue);
            var prefab = game.config.GetBeadTile(color);

            await game.ScanRandom(count, t => t is BeadTile bead && bead.color != color, async tile =>
            {
                var bead = tile as BeadTile;
                game.engine.RemoveEntity(bead);
                var newTile = game.CreateColoredTile(prefab);
                game.SetTileAt(tile.position, newTile);
                newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                await game.Wait(timeDelay);
            });

            // var swapped = 0;
            // for (int i = 0; i < searchCount && swapped < count; i++)
            // {
            //     var point = game.RandPoint();
            //     var tile = game.GetTileAt(point);
            //     if (tile != null && tile is BeadTile beadTile && beadTile.color != color)
            //     {
            //         swapped++;
            //         game.engine.RemoveEntity(beadTile);
            //         var newTile = game.CreateColoredTile(prefab);
            //         game.SetTileAt(point, newTile);
            //         newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
            //         await game.Wait(timeDelay);
            //     }
            // }
            await game.Wait(0.4f);
        }
    }
}