namespace MMC.Game
{
    public class FinishGamePanel : BasePanel
    {
        [Member]
        public void Menu()
        {
            network.game.LeaveGame();
        }
    }
}