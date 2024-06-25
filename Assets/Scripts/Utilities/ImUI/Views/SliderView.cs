namespace ImUI
{
    public class SliderView : View<SliderViewState>
    {
        public TextAdaptor label;
        public TextAdaptor value;
        public SliderHelper slider;

        protected override void OnCreated()
        {
            base.OnCreated();
            slider.onValueChanged.AddListener((n) => Changed());
            slider.onStartEdit.AddListener(StartEdit);
            slider.onEndEdit.AddListener(EndEdit);
        }

        protected override void LoadState(SliderViewState state)
        {
            base.LoadState(state);
            label.text = state.label;
            slider.slider.interactable = canEdit;
            slider.slider.minValue = state.min;
            slider.slider.maxValue = state.max;
            slider.SetValueWithoutNotify(state.value);
            slider.slider.wholeNumbers = state.wholeNumbers;
            if (state.wholeNumbers)
                value.text = ((int)state.value).ToString();
            else
                value.text = state.value.ToString(".00");
        }
        public override ViewState GetState()
        {
            return new SliderViewState(state.label, slider.value, state.min, state.max, state.wholeNumbers);
        }
    }

    public class SliderViewState : ViewState
    {
        public string label;
        public float value;
        public float min;
        public float max;
        public bool wholeNumbers;

        public SliderViewState(string label, float value, float min, float max, bool wholeNumbers)
        {
            this.label = label;
            this.min = min;
            this.max = max;
            this.value = value < min ? min : value > max ? max : value;
            this.wholeNumbers = wholeNumbers;
        }
    }
}