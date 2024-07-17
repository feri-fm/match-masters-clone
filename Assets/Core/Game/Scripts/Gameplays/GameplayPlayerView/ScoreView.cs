using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class ScoreView : GameplayPlayerViewPart
    {
        public TextMember score;
        public TextMember counterScore;
        public GameObjectMember counter;

        public override void Render()
        {
            base.Render();
            score.text = player.score.ToString();
        }
    }
}