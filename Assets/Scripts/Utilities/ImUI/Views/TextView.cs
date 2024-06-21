using UnityEngine;

namespace ImUI
{
    public class TextView : View<TextViewState>
    {
        public TextAdaptor label;
        public InputFieldHelper inputField;

        protected override void OnCreated()
        {
            base.OnCreated();
            inputField.inputField.onValueChanged.AddListener((n) => Changed());
            inputField.onStartEdit.AddListener(StartEdit);
            inputField.inputField.onEndEdit.AddListener((e) => EndEdit());
        }

        protected override void LoadState(TextViewState state)
        {
            base.LoadState(state);
            label.text = state.label;
            if (state.text != inputField.text)
                inputField.SetTextWithoutNotify(state.text);
            inputField.inputField.interactable = canEdit;
        }
        public override ViewState GetState()
        {
            return new TextViewState(state.label, inputField.text);
        }
    }

    public class TextViewState : ViewState
    {
        public string label;
        public string text;

        public TextViewState(string label, string text)
        {
            this.label = label;
            this.text = text;
        }
    }
}