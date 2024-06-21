using UnityEngine;

namespace ImUI
{
    public class SpaceView : View<SpaceViewState>
    {
        public RectTransform rect;

        protected override void LoadState(SpaceViewState state)
        {
            base.LoadState(state);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, state.height);
        }
    }

    public class SpaceViewState : ViewState
    {
        public float height;

        public SpaceViewState(float height)
        {
            this.height = height;
        }
    }
}