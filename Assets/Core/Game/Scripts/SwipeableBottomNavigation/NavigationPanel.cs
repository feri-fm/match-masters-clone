using System.Collections;
using System.Collections.Generic;
using MMC.Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SwipeableBottomNavigation
{
    public class NavigationPanel : NavigationSegment
    {
        public RectTransform rect { get; private set; }

        public GameManager game => GameManager.instance;

        public bool isActive;

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
