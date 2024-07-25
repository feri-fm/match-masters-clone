using System.Collections.Generic;
using SwipeableBottomNavigation;
using UnityEngine;

namespace MMC.Game
{
    public class MenuPanel : BasePanel
    {
        public TextMember username;
        public TextMember trophies;
        public TextMember coins;
        public Member<NavigationManager> navigation;
        public Member<RectTransform> topBar;

        public float smooth = 10;
        public List<int> hideTopBarAt = new();

        public override void Setup()
        {
            base.Setup();
            navigation.value.SetPanel(2);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            navigation.value.Jump();
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnRender()
        {
            base.OnRender();
            username.text = game.user.username;
            trophies.text = game.user.trophies.ToString();
            coins.text = game.user.coins.ToString();
            navigation.value.Render();
        }

        public override void Update()
        {
            base.Update();
            var hide = hideTopBarAt.Contains(navigation.value.currentPanelIndex);
            var height = topBar.value.sizeDelta.y + 10;
            topBar.value.anchoredPosition = Vector2.Lerp(topBar.value.anchoredPosition, hide ? Vector2.up * height : Vector2.zero, smooth * Time.deltaTime);
        }

        [Member]
        public void Profile()
        {
            game.profilePanel.OpenPanel();
        }
    }
}