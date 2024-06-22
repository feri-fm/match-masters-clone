using UnityEngine;

namespace ImUI
{
    public class HorizontalLayoutView : LayoutView<HorizontalLayoutViewState>
    {
        public RectTransform body;
        public Transform container;

        protected override void LoadView(View view)
        {
            base.LoadView(view);
            view.transform.parent = container;
        }

        protected override void LoadState(HorizontalLayoutViewState state)
        {
            base.LoadState(state);
            body.sizeDelta = new Vector2(body.sizeDelta.x, state.height);
        }
        public override ViewState GetState()
        {
            return new HorizontalLayoutViewState(state.height);
        }
    }

    public class HorizontalLayoutViewState : LayoutViewState
    {
        public float height;

        public HorizontalLayoutViewState(float height)
        {
            this.height = height;
        }
    }
}