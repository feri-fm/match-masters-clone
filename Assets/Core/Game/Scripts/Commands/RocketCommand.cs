using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    [System.Serializable]
    public class RocketBoxCommand : GameCommand
    {
        [JsonDataInt] public int count = 4;
        [JsonDataFloat] public float timeDelay = 0.2f;

        protected override async Task Run()
        {
            await game.ScanRandom(count, t => true, async tile =>
            {
                _ = tile.Hit();
                await game.Wait(timeDelay);
            });

            // for (int i = 0; i < targetCount; i++)
            // {
            //     var tile = game.GetTileAt(game.RandPoint());
            //     if (tile != null)
            //     {
            //         _ = tile.Hit();
            //         await game.Wait(timeDelay);
            //     }
            // }
            await game.Wait(0.3f);
        }
    }
}