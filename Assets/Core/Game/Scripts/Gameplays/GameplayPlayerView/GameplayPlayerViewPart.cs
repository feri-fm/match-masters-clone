using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class GameplayPlayerViewPart : MonoBehaviour
    {
        public GameplayPlayer player { get; private set; }

        public virtual void Render() { }

        public void Render(GameplayPlayer player)
        {
            this.player = player;
            Render();
        }
    }
}