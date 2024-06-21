using UnityEngine;
using UnityEngine.UI;

namespace ImUI
{
    public class ButtonView : View<ButtonViewState>
    {
        public TextAdaptor label;
        public Selectable selectable;

        private bool pressed;

        protected override void LoadState(ButtonViewState state)
        {
            base.LoadState(state);
            label.text = state.label;
            selectable.interactable = canEdit;
        }

        public override ViewState GetState()
        {
            return new ButtonViewState(state.label, pressed);
        }

        public void Press()
        {
            if (canEdit)
            {
                StartEdit();
                pressed = true;
                Changed();
                EndEdit();
            }
        }

        public override void Used()
        {
            base.Used();
            pressed = false;
        }
    }

    public class ButtonViewState : ViewState
    {
        public string label;
        public bool pressed;

        public ButtonViewState(string label, bool pressed)
        {
            this.label = label;
            this.pressed = pressed;
        }
    }
}