using UnityEngine;

namespace MMC.Game
{
    public class ChapterView : PoolObject
    {
        public Chapter chapter { get; private set; }
        public ChapterViewLoader loader { get; private set; }

        public void Setup(Chapter chapter, ChapterViewLoader loader)
        {
            this.chapter = chapter;
            this.loader = loader;
        }
    }
}