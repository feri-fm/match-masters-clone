using System.Linq;
using System.Threading.Tasks;

namespace MMC.Match3
{
    public class HatCommand : GameCommand
    {
        [JsonDataInt] public int searchCount = 10;
        [JsonDataInt] public int targetCount = 6;
        [JsonDataFloat] public float timeDelay = 0.1f;

        protected override async Task Run()
        {
            var swapped = 0;
            for (int i = 0; i < searchCount && swapped < targetCount; i++)
            {
                var point = game.RandPoint();
                var tile = game.GetTileAt(point);
                if (tile != null && tile is BeadTile beadTile)
                {
                    swapped++;
                    var color = beadTile.color;
                    var prefab = game.RandElement(game.config.rewardTiles.Where(e => e.color == color).ToArray());
                    game.engine.RemoveEntity(beadTile);
                    var newTile = game.CreateTile(prefab);
                    game.SetTileAt(point, newTile);
                    newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                    await game.Wait(timeDelay);
                }
            }
            await game.Wait(0.4f);
        }
    }
}