namespace MMC.Game
{
    public class MenuPanel : BasePanel
    {
        public TextMember username;
        public TextMember trophies;
        public TextMember coins;

        public ChapterViewLoader chapterViewLoader;

        public override void Setup()
        {
            base.Setup();
            chapterViewLoader.Clear();
        }

        public override void OnClose()
        {
            base.OnClose();
            chapterViewLoader.Clear();
        }

        public override void OnRender()
        {
            base.OnRender();
            username.text = game.user.username;
            trophies.text = game.user.trophies.ToString();
            coins.text = game.user.coins.ToString();
            chapterViewLoader.Setup(game.config.GetChapter(game.user.trophies));
        }

        [Member]
        public void Play()
        {
            game.Play();
        }

        [Member]
        public void Profile()
        {
            game.profilePanel.OpenPanel();
        }
    }
}