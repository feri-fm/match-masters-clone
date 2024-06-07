using System;

namespace Match3
{

    public abstract class LifecycleObjectView : PoolObject
    {
        private bool dirty;

        protected virtual void OnSetup() { }
        protected virtual void OnRemoved() { }
        protected virtual void OnRender() { }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (dirty)
            {
                dirty = false;
                __Render();
            }
        }

        public void MarkDirty()
        {
            dirty = true;
        }

        protected void __Setup(LifecycleObject lifecycle)
        {
            lifecycle.onChanged += MarkDirty;
            OnSetup();
        }
        protected void __Remove(LifecycleObject lifecycle)
        {
            lifecycle.onChanged -= MarkDirty;
            OnRemoved();
        }
        private void __Render()
        {
            OnRender();
        }
    }
    public abstract class LifecycleObject
    {
        public event Action onSetup = delegate { };
        public event Action onRemoved = delegate { };
        public event Action onChanged = delegate { };

        protected virtual void OnSetup() { }
        protected virtual void OnRemoved() { }
        protected virtual void OnChanged() { }

        protected void __Setup()
        {
            onSetup.Invoke();
            OnSetup();
        }
        protected void __Remove()
        {
            onRemoved.Invoke();
            OnRemoved();
        }
        protected void __Changed()
        {
            onChanged.Invoke();
            OnChanged();
        }
    }
}
