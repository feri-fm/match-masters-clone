namespace ImUI
{
    public abstract class ViewParam
    {
        public View view;

        public void Setup(View view)
        {
            this.view = view;
        }
        public abstract void Apply();
        public abstract void Clear();
    }
}