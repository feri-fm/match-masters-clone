namespace MMC.Game
{
    public class GamePanel : BasePanel
    {
        [Member]
        public void Leave()
        {
            network.game.client.LeaveGame();
        }
    }
}