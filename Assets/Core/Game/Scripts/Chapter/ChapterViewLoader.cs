using UnityEngine;

namespace MMC.Game
{
    public class ChapterViewLoader : MonoBehaviour
    {
        private ChapterView currentView;

        public void Setup(Chapter chapter)
        {
            Clear();
            currentView = ObjectPool.global.Spawn(chapter.chapterViewPrefab, transform);
            currentView.transform.localPosition = Vector3.zero;
            currentView.transform.localRotation = Quaternion.identity;
            currentView.Setup(chapter, this);
        }

        public void Clear()
        {
            if (currentView != null)
            {
                currentView.Pool();
            }
        }
    }
}