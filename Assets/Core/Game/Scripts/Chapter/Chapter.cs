using UnityEngine;

namespace MMC.Game
{
    public class Chapter : MonoBehaviour
    {
        public string key => name;
        public int trophy;
        public ChapterView chapterViewPrefab;
        public Sprite icon;

        public void Apply(Gameplay gameplay)
        {
            Apply(new GameplayIns()
            {
                gameplay = gameplay,
            });
        }

        protected virtual void Apply(GameplayIns ins) { }
    }
}