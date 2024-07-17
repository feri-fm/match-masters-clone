using System.Threading.Tasks;
using MMC.Match3;

namespace MMC.Game
{
    public class ShufflePerk : Perk
    {
        public ShuffleCommand command;

        protected override async Task Use()
        {
            await base.Use();
            await game.RunCommand(command);
        }
    }
}