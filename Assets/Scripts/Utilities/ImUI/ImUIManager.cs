using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        public T BuildView<T>(string type, T state) where T : ViewState
        {
            var view = oldViews.Find(e => e.type == type);
            if (view == null)
            {
                var prefab = views.Find(e => e.type == type);
                view = pool.Spawn(prefab, container);
                view._Setup(this, state);
                currentViews.Add(view);
            }
            view.transform.SetAsLastSibling();
            oldViews.Remove(view);

            state.indent = builder.indent;

            if (view.changed)
            {
                state = view.GetState() as T;
                view.Used();
            }
            else
            {
                view.LoadState(state);
            }
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

    public class ImUIBuilder
    {
        public float indent = 0;

        public ImUIManager manager { get; }

        public ImUIBuilder(ImUIManager manager)
        {
            this.manager = manager;
        }

        public void Title(string label)
        {
            manager.BuildView("title", new TitleViewState(label));
        }
        public void Label(string label)
        {
            manager.BuildView("label", new LabelViewState(label));
        }
        public int Number(string label, int value)
        {
            return manager.BuildView("number", new NumberViewState(label, value)).intNumber;
        }
        public float Number(string label, float value)
        {
            return manager.BuildView("number", new NumberViewState(label, value)).floatNumber;
        }
        public string Text(string label, string text)
        {
            return manager.BuildView("text", new TextViewState(label, text)).text;
        }
        public bool Button(string label)
        {
            return manager.BuildView("button", new ButtonViewState(label, false)).pressed;
        }
        public void Image(Sprite sprite)
        {
            manager.BuildView("image", new ImageViewState(sprite));
        }
        public void Image(Texture texture)
        {
            manager.BuildView("image", new ImageViewState(texture));
        }
        public void Space(float height)
        {
            manager.BuildView("space", new SpaceViewState(height));
        }
        public int Slider(string label, int value, int min, int max)
        {
            return (int)manager.BuildView("slider", new SliderViewState(label, value, min, max, true)).value;
        }
        public float Slider(string label, float value, float min, float max)
        {
            return manager.BuildView("slider", new SliderViewState(label, value, min, max, false)).value;
        }
        public bool Toggle(string label, bool isOn)
        {
            return manager.BuildView("toggle", new ToggleViewState(label, isOn)).isOn;
        }
    }
}
