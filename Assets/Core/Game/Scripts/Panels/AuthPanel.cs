namespace MMC.Game
{
    public class AuthPanel : BasePanel
    {
        public GameObjectMember connecting;

        public override void OnRender()
        {
            base.OnRender();
            connecting.SetActive(game.isConnecting);
        }

        [Member]
        public void Register()
        {
            game.Register();
        }
    }
}