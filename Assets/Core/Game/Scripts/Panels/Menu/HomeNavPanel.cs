using SwipeableBottomNavigation;

namespace MMC.Game
{
    public class HomeNavPanel : NavigationPanel
    {
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
            chapterViewLoader.Setup(game.config.GetChapter(game.user.trophies));
        }

        [Member]
        public void Play()
        {
            game.Play();
        }
    }
}