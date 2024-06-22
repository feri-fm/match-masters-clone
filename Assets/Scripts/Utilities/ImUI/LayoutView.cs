using UnityEngine;

namespace ImUI
{
    public class LayoutView : View<LayoutViewState>
    {
        public void _LoadView(View view)
        {
            LoadView(view);
        }

        protected virtual void LoadView(View view) { }
    }

    public class LayoutView<T> : LayoutView where T : LayoutViewState
    {
        public new T state => base.state as T;

        protected sealed override void LoadState(LayoutViewState state)
        {
            base.LoadState(state);
            LoadState(state as T);
        }

        protected virtual void LoadState(T state) { }
    }

    public class LayoutViewState : ViewState
    {

    }
}