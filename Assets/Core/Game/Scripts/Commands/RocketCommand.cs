using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    [System.Serializable]
    public class RocketBoxCommand : GameCommand
    {
        [JsonDataInt] public int targetCount = 4;
        [JsonDataFloat] public float timeDelay = 0.2f;

        protected override async Task Run()
        {
            for (int i = 0; i < targetCount; i++)
            {
                var tile = game.GetTileAt(game.RandPoint());
                if (tile != null)
                {
                    _ = tile.Hit();
                    await game.Wait(timeDelay);
                }
            }
            await game.Wait(0.3f);
        }
    }
}