using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    [System.Serializable]
    public class BucketCommand : GameCommand
    {
        [JsonDataInt] public int searchCount = 10;
        [JsonDataInt] public int targetCount = 6;
        [JsonDataFloat] public float timeDelay = 0.1f;
        [JsonDataInt] public int colorValue;

        protected override async Task Run()
        {
            var swapped = 0;
            // var color = game.config.beads[2 + game.RandInt(game.options.beads - 2)].color;
            var color = new TileColor(colorValue);
            var prefab = game.config.GetBeadTile(color);
            for (int i = 0; i < searchCount && swapped < targetCount; i++)
            {
                var point = game.RandPoint();
                var tile = game.GetTileAt(point);
                if (tile != null && tile is BeadTile beadTile && beadTile.color != color)
                {
                    swapped++;
                    game.engine.RemoveEntity(beadTile);
                    var newTile = game.CreateColoredTile(prefab);
                    game.SetTileAt(point, newTile);
                    newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                    await game.Wait(timeDelay);
                }
            }
            await game.Wait(0.4f);
        }
    }
}