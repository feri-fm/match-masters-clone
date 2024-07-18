using UnityEngine;

namespace MMC.Game
{
    public class Chapter : MonoBehaviour
    {
        public string key => name;
        public int trophy;
        public Sprite icon;

        protected Gameplay gameplay;
        protected Match3.Game game => gameplay.gameEntity.game;

        public void Apply(Gameplay gameplay)
        {
            this.gameplay = gameplay;
            Apply();
        }

        protected virtual void Apply() { }
    }
}