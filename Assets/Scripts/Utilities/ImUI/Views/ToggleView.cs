using UnityEngine;

namespace ImUI
{
    public class ToggleView : View<ToggleViewState>
    {
        public TextAdaptor label;
        public ToggleHelper toggle;

        protected override void OnCreated()
        {
            base.OnCreated();
            toggle.onValueChanged.AddListener((n) => Changed());
            toggle.onStartEdit.AddListener(StartEdit);
            toggle.onEndEdit.AddListener(EndEdit);
        }

        protected override void LoadState(ToggleViewState state)
        {
            base.LoadState(state);
            label.text = state.label;
            toggle.SetIsOnWithoutNotify(state.isOn);
            toggle.toggle.interactable = canEdit;
        }
        public override ViewState GetState()
        {
            return new ToggleViewState(state.label, toggle.isOn);
        }
    }

    public class ToggleViewState : ViewState
    {
        public string label;
        public bool isOn;

        public ToggleViewState(string label, bool isOn)
        {
            this.label = label;
            this.isOn = isOn;
        }
    }
}