namespace MMC.Game
{
    public class StartupPanel : BasePanel
    {
        public GameObjectMember loading;
        public GameObjectMember error;

        public override void OnRender()
        {
            base.OnRender();
            loading.SetActive(game.isConnecting);
            error.SetActive(!game.isConnecting);
        }

        [Member]
        public void Retry()
        {
            game.Startup();
        }
    }
}