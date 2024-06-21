namespace ImUI
{
    public class NumberView : View<NumberViewState>
    {
        public TextAdaptor label;
        public NumberField numberField;

        protected override void OnCreated()
        {
            base.OnCreated();
            numberField.onValueChanged.AddListener((n) => Changed());
            numberField.onStartEdit.AddListener(StartEdit);
            numberField.onEndEdit.AddListener(EndEdit);
        }

        protected override void LoadState(NumberViewState state)
        {
            base.LoadState(state);
            label.text = state.label;
            numberField.step = state.isInt ? 1 : 0.1f;
            if (state.number != numberField.value)
                numberField.SetValueWithoutNotify(state.number);
            numberField.inputField.inputField.interactable = canEdit;
        }
        public override ViewState GetState()
        {
            if (state.isInt)
                return new NumberViewState(state.label, (int)numberField.value);
            else
                return new NumberViewState(state.label, numberField.value);
        }
    }

    public class NumberViewState : ViewState
    {
        public string label;

        public float floatNumber;
        public int intNumber;
        public bool isInt;

        public float number => isInt ? intNumber : floatNumber;

        public NumberViewState(string label, int number)
        {
            this.label = label;
            this.intNumber = number;
            isInt = true;
        }
        public NumberViewState(string label, float number)
        {
            this.label = label;
            this.floatNumber = number;
            isInt = false;
        }
    }
}