using UnityEngine;

namespace MMC.Game
{
    public class ChapterViewLoader : MonoBehaviour
    {
        public CameraRig cameraRig;
        public Vector2 offset;

        private ChapterView currentView;

        public void Setup(Chapter chapter)
        {
            Clear();
            currentView = ObjectPool.global.Spawn(chapter.chapterViewPrefab, transform);
            currentView.transform.localPosition = Vector3.zero;
            currentView.transform.localRotation = Quaternion.identity;
            currentView.Setup(chapter, this);
        }

        private void LateUpdate()
        {
            if (currentView != null)
            {
                var rect = cameraRig.GetRect();
                currentView.transform.localPosition = new Vector3(offset.x * rect.width, offset.y * rect.height);
            }
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