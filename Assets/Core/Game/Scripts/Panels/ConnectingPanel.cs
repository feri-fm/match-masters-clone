namespace MMC.Game
{
    public class ConnectingPanel : BasePanel
    {
        public GameObjectMember connecting;
        public GameObjectMember notConnected;

        public override void OnRender()
        {
            base.OnRender();
            connecting.SetActive(game.isConnecting);
            notConnected.SetActive(!game.isConnecting);
        }

        [Member]
        public void Connect()
        {
            game.Startup();
        }
    }
}