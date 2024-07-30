using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwipeableBottomNavigation
{
    public class NavigationLine : NavigationSegment
    {
        public LayoutElement layoutElement;

        private void Update()
        {
            layoutElement.flexibleWidth = Mathf.Lerp(layoutElement.flexibleWidth, weight, navigation.smooth * Time.deltaTime);
        }

        public void Jump()
        {
            layoutElement.flexibleWidth = weight;
        }
    }
}