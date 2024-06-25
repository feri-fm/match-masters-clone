using System.Threading.Tasks;

namespace Match3
{
    public class ShuffleCommand : GameCommand
    {
        public int searchCount = 30;
        public int targetCount = 20;

        protected override async Task Run()
        {
            var swapped = 0;
            for (int i = 0; i < searchCount && swapped < targetCount; i++)
            {
                var a = game.RandPoint();
                var b = game.RandPoint();
                if (game.IsNotEmptyAt(a) && game.IsNotEmptyAt(b))
                {
                    game.Swap(a, b);
                    if (game.AnyMatch())
                        game.Swap(a, b);
                    else
                        swapped++;
                }
            }
            await game.Wait(0.6f);
        }
    }
}