namespace MMC.Game
{
    public class MenuPanel : BasePanel
    {
        public TextMember username;

        public override void OnRender()
        {
            base.OnRender();
            username.text = game.user.username;
        }

        [Member]
        public void Join()
        {
            game.Join();
        }
    }
}