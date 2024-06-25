using System;

namespace ImUI
{
    public class VPGetView : ViewParam
    {
        public Action<View> getView;

        public VPGetView(Action<View> getView)
        {
            this.getView = getView;
        }

        public override void Apply()
        {
            getView.Invoke(view);
        }

        public override void Clear()
        {
            getView.Invoke(null);
        }
    }
}