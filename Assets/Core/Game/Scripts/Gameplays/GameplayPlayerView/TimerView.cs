using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace MMC.Game.GameplayPlayerViews
{
    public class TimerView : GameplayPlayerViewPart
    {
        public Member<Slider> slider;

        private void Update()
        {
            if (player != null)
            {
                var gameplay = player.gameplay as TwoPlayerGameplay;
                var leftTime = gameplay.timerEndAt - NetworkTime.time;
                var t = (float)leftTime / gameplay.prefab.turnTime;
                slider.value.SetValueWithoutNotify(t);
            }
            else
            {
                slider.value.SetValueWithoutNotify(1);
            }
        }
    }
}