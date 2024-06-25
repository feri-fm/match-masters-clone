using UnityEngine;
using UnityEngine.UI;

namespace ImUI
{

    public class VPDisabled : ViewParam
    {
        public bool disabled;

        public VPDisabled()
        {
            disabled = true;
        }
        public VPDisabled(bool disabled)
        {
            this.disabled = disabled;
        }

        public override void Apply()
        {
            view.state.disabled = disabled;
        }

        public override void Clear()
        {

        }
    }
}
