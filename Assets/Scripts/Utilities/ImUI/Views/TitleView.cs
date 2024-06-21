namespace ImUI
{
    public class TitleView : View<TitleViewState>
    {
        public TextAdaptor text;

        protected override void LoadState(TitleViewState state)
        {
            base.LoadState(state);
            text.text = state.text;
        }
    }

    public class TitleViewState : ViewState
    {
        public string text;

        public TitleViewState(string text)
        {
            this.text = text;
        }
    }
}