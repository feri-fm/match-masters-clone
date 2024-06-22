using UnityEngine;
using UnityEngine.UI;

namespace ImUI
{

    public class VPLayoutElement : ViewParam
    {
        public LayoutElement layoutElement { get; private set; }

        public override void Apply()
        {
            layoutElement = view.gameObject.AddComponent<LayoutElement>();
        }

        public override void Clear()
        {
            Object.Destroy(layoutElement);
        }
    }
    public abstract class VPLayoutElementValue : VPLayoutElement
    {
        protected float value;

        public VPLayoutElementValue(float value)
        {
            this.value = value;
        }
    }
    public class VPLayoutFlexibleWidth : VPLayoutElementValue
    {
        public VPLayoutFlexibleWidth(float value) : base(value) { }
        public override void Apply()
        {
            base.Apply();
            layoutElement.flexibleWidth = value;
        }
    }
    public class VPLayoutFlexibleHeight : VPLayoutElementValue
    {
        public VPLayoutFlexibleHeight(float value) : base(value) { }
        public override void Apply()
        {
            base.Apply();
            layoutElement.flexibleHeight = value;
        }
    }
    public class VPLayoutMinWidth : VPLayoutElementValue
    {
        public VPLayoutMinWidth(float value) : base(value) { }
        public override void Apply()
        {
            base.Apply();
            layoutElement.minWidth = value;
        }
    }
    public class VPLayoutMinHeight : VPLayoutElementValue
    {
        public VPLayoutMinHeight(float value) : base(value) { }
        public override void Apply()
        {
            base.Apply();
            layoutElement.minHeight = value;
        }
    }
}
