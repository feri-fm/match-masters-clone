using System.Threading.Tasks;

namespace Match3
{
    public class RocketCommand : GameCommand
    {
        public int searchCount = 8;
        public int targetCount = 4;
        public float timeDelay = 0.2f;

        protected override async Task Run()
        {
            var swapped = 0;
            for (int i = 0; i < searchCount && swapped < targetCount; i++)
            {
                var tile = game.GetTileAt(game.RandPoint());
                if (tile != null)
                {
                    swapped++;
                    _ = tile.Hit();
                    await game.Wait(timeDelay);
                }
            }
            await game.Wait(0.3f);
        }
    }
}