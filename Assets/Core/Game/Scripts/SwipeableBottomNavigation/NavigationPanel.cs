using System.Collections;
using System.Collections.Generic;
using MMC.Game;
using MMC.Network;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SwipeableBottomNavigation
{
    public class NavigationPanel : NavigationSegment
    {
        public RectTransform rect { get; private set; }

        public GameManager game => GameManager.instance;
        public NetNetworkManager network => NetNetworkManager.instance;

        public bool isActive { get; private set; }

        public virtual void OnSelected() { }
        public virtual void OnOpen() { }
        public virtual void OnClose() { }

        public override void Setup()
        {
            base.Setup();
            rect = GetComponent<RectTransform>();
        }

        public void SetActive(bool value)
        {
            isActive = value;
            gameObject.SetActive(value);
        }
    }
}
