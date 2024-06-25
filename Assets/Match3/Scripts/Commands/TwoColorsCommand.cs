using System.Threading.Tasks;
using Core;

namespace Match3
{
    public class TwoColorsCommand : GameCommand
    {
        protected override async Task Run()
        {
            foreach (var tile in game.tiles)
            {
                engine.RemoveEntity(tile);
            }
            var s1 = game.RandInt(game.options.beads);
            var s2 = (s1 + 1 + game.RandInt(game.options.beads - 2)) % game.options.beads;
            for (int i = 0; i < game.width; i++)
            {
                for (int j = 0; j < game.height; j++)
                {
                    var black = (i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0);
                    var bead = black ? game.config.beads[s1] : game.config.beads[s2];
                    var tile = game.CreateColoredTile(bead);
                    var point = new Int2(i, j);
                    game.SetTileAt(point, tile);
                    tile.WithTrait<AnimatorTrait>(t => t.Jump());
                }
            }
            await game.Wait(0.1f);
        }
    }
}