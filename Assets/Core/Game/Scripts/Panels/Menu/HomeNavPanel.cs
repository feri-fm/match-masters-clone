using SwipeableBottomNavigation;
using UnityEngine;

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

        private void Update()
        {
            chapterViewLoader.offset = Vector2.left * (navigation.scroll - index);
        }

        [Member]
        public void Play()
        {
            game.Play();
        }
    }
}