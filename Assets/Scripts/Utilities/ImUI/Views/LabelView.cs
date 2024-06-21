namespace ImUI
{
    public class LabelView : View<LabelViewState>
    {
        public TextAdaptor text;

        protected override void LoadState(LabelViewState state)
        {
            base.LoadState(state);
            text.text = state.text;
        }
    }

    public class LabelViewState : ViewState
    {
        public string text;

        public LabelViewState(string text)
        {
            this.text = text;
        }
    }
}