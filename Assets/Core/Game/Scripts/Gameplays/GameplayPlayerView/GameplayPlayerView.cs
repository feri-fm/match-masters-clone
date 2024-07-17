using MMC.Game.GameplayPlayerViews;
using UnityEngine;

namespace MMC.Game
{
    public class GameplayPlayerView : MonoBehaviour
    {
        public Member<ScoreView> score;
        public Member<RoundsView> rounds;
        public Member<MovesView> moves;
        public Member<TimerView> timer;
        public Member<BoosterView> booster;
        public Member<PerksView> perks;

        public void Render(GameplayPlayer player)
        {
            score.value.Render(player);
            rounds.value.Render(player);
            moves.value.Render(player);
            timer.value.Render(player);
            booster.value.Render(player);
            perks.value.Render(player);

            rounds.value.gameObject.SetActive(player.isTurn);
            timer.value.gameObject.SetActive(player.isTurn);
        }
    }
}