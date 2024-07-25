using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwipeableBottomNavigation
{
    public class NavigationCursor : MonoBehaviour
    {
        public float smooth = 10;

        public NavigationManager manager { get; private set; }

        public void Setup(NavigationManager manager)
        {
            this.manager = manager;
        }

        private void Update()
        {
            var target = manager.buttons[manager.currentPanelIndex];
            transform.position = Vector2.Lerp(transform.position, target.transform.position, smooth * Time.deltaTime);
        }

        public void Jump()
        {
            var target = manager.buttons[manager.currentPanelIndex];
            transform.position = target.transform.position;
        }
    }
}