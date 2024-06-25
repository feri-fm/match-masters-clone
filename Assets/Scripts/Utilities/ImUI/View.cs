using UnityEngine;

namespace ImUI
{
    public class View : PoolObject
    {
        public string type;
        public RectTransform padding;

        public ImUIManager manager { get; private set; }
        public ViewState state { get; private set; }

        public bool changed { get; private set; }

        public bool canEdit => !state.disabled && (!manager.isEditing || manager.editingView == this);

        public ViewParam[] viewParams = new ViewParam[] { };

        public void _Setup(ImUIManager manager, ViewState state)
        {
            this.manager = manager;
            this.state = state;
        }

        public void UseViewParams(ViewParam[] newViewParams)
        {
            foreach (var viewParam in viewParams)
            {
                viewParam.Clear();
            }
            viewParams = newViewParams;
            foreach (var viewParam in viewParams)
            {
                viewParam.Setup(this);
                viewParam.Apply();
            }
        }

        public virtual void LoadState(ViewState state)
        {
            padding.offsetMin = new Vector2(state.indent, 0);
            this.state = state;
        }

        public virtual ViewState GetState()
        {
            return state;
        }

        public void Changed()
        {
            if (canEdit)
            {
                changed = true;
                manager.Changed();
            }
        }

        public virtual void Used()
        {
            changed = false;
        }

        public void StartEdit()
        {
            manager.StartEdit(this);
        }

        public void EndEdit()
        {
            manager.EndEdit(this);
        }
    }

    public class View<T> : View where T : ViewState
    {
        public new T state => base.state as T;

        public sealed override void LoadState(ViewState state)
        {
            base.LoadState(state);
            LoadState(state as T);
        }

        protected virtual void LoadState(T state) { }
    }

    public class ViewState
    {
        public View view;

        public float indent;
        public bool disabled;

        public void Update(View view, ImUIBuilder builder)
        {
            this.view = view;

            indent = builder.indent;
            disabled = builder.disabled;
        }
    }
}