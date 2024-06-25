using System.Threading.Tasks;
using Core;

namespace Match3
{
    public class DuckCommand : GameCommand
    {
        public int height = 3;
        public float timeDelay = 0.05f;

        protected override async Task Run()
        {
            for (int i = 0; i < game.width; i++)
            {
                var tile = game.GetTileAt(new Int2(i, height));
                if (tile != null)
                {
                    _ = tile.Hit();
                    await game.Wait(timeDelay);
                }
            }
            await game.Wait(0.2f);
        }
    }
}