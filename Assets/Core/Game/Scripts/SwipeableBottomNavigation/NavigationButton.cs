using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwipeableBottomNavigation
{
    public class NavigationButton : NavigationSegment
    {
        public LayoutElement layoutElement;
        public Member<Transform> icon;
        public Member<Transform> a;
        public Member<Transform> b;

        private void Update()
        {
            layoutElement.flexibleWidth = Mathf.Lerp(layoutElement.flexibleWidth, weight, manager.smooth * Time.deltaTime);
            icon.value.position = Vector3.Lerp(icon.value.position, isSelected ? b.value.position : a.value.position, manager.smooth * Time.deltaTime);
            icon.value.localScale = Vector3.Lerp(icon.value.localScale, isSelected ? b.value.localScale : a.value.localScale, manager.smooth * Time.deltaTime);
        }

        public void Jump()
        {
            layoutElement.flexibleWidth = weight;
            icon.value.position = isSelected ? b.value.position : a.value.position;
            icon.value.localScale = isSelected ? b.value.localScale : a.value.localScale;
        }

        [Member]
        public void Select()
        {
            manager.SetPanel(index);
        }
    }
}