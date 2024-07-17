using System.Threading.Tasks;

namespace MMC.Game
{
    public class HammerPerk : Perk
    {
        protected override async Task Use()
        {
            await base.Use();
            // await ReadTile();
        }
    }
}