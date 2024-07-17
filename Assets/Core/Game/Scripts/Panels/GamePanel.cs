namespace MMC.Game
{
    public class GamePanel : BasePanel
    {
        [Member]
        public void Leave()
        {
            network.game.client.LeaveGame();
        }

        [Member]
        public void RequestGameplay()
        {
            network.game.client.client.CmdRequestGameplayData();
        }
    }
}