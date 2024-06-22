using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ImUI
{
    [RequireComponent(typeof(ObjectPool))]
    public class ImUIManager : MonoBehaviour
    {
        public Transform container;
        public List<View> views = new List<View>();

        public List<View> oldViews { get; } = new List<View>();
        public List<View> currentViews { get; } = new List<View>();

        public ViewBuilder viewBuilder;

        public UnityAction onEditStart = () => { };
        public UnityAction onEditEnd = () => { };

        public ObjectPool pool { get; private set; }

        private bool changed;

        public List<LayoutView> layouts = new();

        public View editingView { get; private set; }
        public bool isEditing { get; private set; }

        public ImUIBuilder builder { get; private set; }

        public delegate void ViewBuilder(ImUIBuilder builder);

        private void Awake()
        {
            pool = GetComponent<ObjectPool>();
            foreach (var view in views)
            {
                view.SetActive(false);
            }
        }

        private void Update()
        {
            if (changed)
            {
                changed = false;
                Build();
                Build();
            }
        }

        public void Build()
        {
            Begin();
            viewBuilder.Invoke(builder);
            End();
        }

        public void SetViewBuilder(ViewBuilder viewBuilder)
        {
            Clear();
            builder = new ImUIBuilder(this);
            this.viewBuilder = viewBuilder;
            Changed();
        }

        public void Clear()
        {
            if (isEditing)
                EndEdit(editingView);

            foreach (var view in currentViews)
                view.Pool();
            currentViews.Clear();
        }

        public void Begin()
        {
            oldViews.Clear();
            oldViews.AddRange(currentViews);
        }

        public void End()
        {
            foreach (var view in oldViews.ToArray())
            {
                currentViews.Remove(view);
                oldViews.Remove(view);
                view.Pool();
            }
        }

        public void EndLayout()
        {
            if (layouts.Count > 0)
            {
                var layout = layouts[layouts.Count - 1];
                layouts.Remove(layout);
            }
        }
        public T BuildLayoutView<T>(string type, T state, ViewParam[] viewParams) where T : LayoutViewState
        {
            var newState = BuildView(type, state, viewParams);
            var view = newState.view as LayoutView;
            if (!layouts.Contains(view))
                layouts.Add(view);
            return newState;
        }
        public T BuildView<T>(string type, T state, ViewParam[] viewParams) where T : ViewState
        {
            var view = oldViews.Find(e => e.type == type);
            if (view == null)
            {
                var prefab = views.Find(e => e.type == type);
                view = pool.Spawn(prefab, container);
                view._Setup(this, state);
                currentViews.Add(view);
            }
            if (layouts.Count > 0)
            {
                var layout = layouts[layouts.Count - 1];
                layout._LoadView(view);
            }
            view.transform.SetAsLastSibling();
            oldViews.Remove(view);

            state.Update(view, builder);

            if (view.changed)
            {
                state = view.GetState() as T;
                view.Used();
            }
            else
            {
                view.LoadState(state);
            }

            view.UseViewParams(viewParams);

            return state;
        }

        public void LoadState()
        {
            foreach (var view in currentViews)
            {
                view.LoadState(view.state);
            }
        }

        public void Changed()
        {
            changed = true;
        }

        public void StartEdit(View view)
        {
            if (isEditing)
            {
                EndEdit(editingView);
            }
            isEditing = true;
            editingView = view;
            LoadState();
            onEditStart.Invoke();
        }
        public void EndEdit(View view)
        {
            isEditing = false;
            editingView = null;
            Build();
            onEditEnd.Invoke();
        }
    }
}
