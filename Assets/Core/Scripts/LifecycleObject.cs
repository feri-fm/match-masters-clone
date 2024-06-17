using System;

namespace Core
{

    public abstract class LifecycleObjectView : PoolObject
    {
        private bool dirty;

        protected virtual void OnSetup() { }
        protected virtual void OnRemoved() { }
        protected virtual void OnChanged() { }
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
            MarkDirty();
            lifecycle.onChanged += MarkDirty;
            lifecycle.onChanged += OnChanged;
            OnSetup();
        }
        protected void __Remove(LifecycleObject lifecycle)
        {
            lifecycle.onChanged -= MarkDirty;
            lifecycle.onChanged -= OnChanged;
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
            OnSetup();
            onSetup.Invoke();
        }
        protected void __Remove()
        {
            OnRemoved();
            onRemoved.Invoke();
        }
        protected void __Changed()
        {
            OnChanged();
            onChanged.Invoke();
        }
    }
}
