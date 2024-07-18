using UnityEngine;
using UnityEngine.UI;

namespace MMC.Game.GameplayPlayerViews
{
    public class BoosterView : GameplayPlayerViewPart
    {
        public Member<Slider> slider;
        public Member<Image> boosterImage;
        public TextMember fillText;
        public GameObjectMember activate;

        public override void Render()
        {
            base.Render();
            boosterImage.For(e => e.sprite = player.booster.icon);
            slider.value.value = (float)player.boosterScore / player.booster.requiredScore;
            if (player.boosterScore >= player.booster.requiredScore)
                fillText.text = "Full";
            else
                fillText.text = $"{player.boosterScore} / {player.booster.requiredScore}";

            activate.SetActive(player.boosterScore >= player.booster.requiredScore && player.isTurn);
        }

        [Member]
        public async void Activate()
        {
            if (player.isMyPlayer)
            {
                await player.UseBooster(null);
            }
        }
    }
}