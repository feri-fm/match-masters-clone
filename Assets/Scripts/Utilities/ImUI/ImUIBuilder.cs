using UnityEngine;

namespace ImUI
{

    public class ImUIBuilder
    {
        public float indent = 0;

        public ImUIManager manager { get; }

        public ImUIBuilder(ImUIManager manager)
        {
            this.manager = manager;
        }

        public void Title(string label, params ViewParam[] viewParams)
        {
            manager.BuildView("title", new TitleViewState(label), viewParams);
        }
        public void Label(string label, params ViewParam[] viewParams)
        {
            manager.BuildView("label", new LabelViewState(label), viewParams);
        }
        public int Number(string label, int value, params ViewParam[] viewParams)
        {
            return manager.BuildView("number", new NumberViewState(label, value), viewParams).intNumber;
        }
        public float Number(string label, float value, params ViewParam[] viewParams)
        {
            return manager.BuildView("number", new NumberViewState(label, value), viewParams).floatNumber;
        }
        public string Text(string label, string text, params ViewParam[] viewParams)
        {
            return manager.BuildView("text", new TextViewState(label, text), viewParams).text;
        }
        public bool Button(string label, params ViewParam[] viewParams)
        {
            return manager.BuildView("button", new ButtonViewState(label, false), viewParams).pressed;
        }
        public void Image(Sprite sprite, params ViewParam[] viewParams)
        {
            manager.BuildView("image", new ImageViewState(sprite), viewParams);
        }
        public void Image(Texture texture, params ViewParam[] viewParams)
        {
            manager.BuildView("image", new ImageViewState(texture), viewParams);
        }
        public void Space(float height, params ViewParam[] viewParams)
        {
            manager.BuildView("space", new SpaceViewState(height), viewParams);
        }
        public int Slider(string label, int value, int min, int max, params ViewParam[] viewParams)
        {
            return (int)manager.BuildView("slider", new SliderViewState(label, value, min, max, true), viewParams).value;
        }
        public float Slider(string label, float value, float min, float max, params ViewParam[] viewParams)
        {
            return manager.BuildView("slider", new SliderViewState(label, value, min, max, false), viewParams).value;
        }
        public bool Toggle(string label, bool isOn, params ViewParam[] viewParams)
        {
            return manager.BuildView("toggle", new ToggleViewState(label, isOn), viewParams).isOn;
        }

        public void EndLayout()
        {
            manager.EndLayout();
        }

        public void BeginHorizontal(float height, params ViewParam[] viewParams)
        {
            manager.BuildLayoutView("horizontal", new HorizontalLayoutViewState(height), viewParams);
        }
    }
}