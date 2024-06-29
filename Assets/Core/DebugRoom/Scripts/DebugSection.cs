using ImUI;
using UnityEngine;

namespace MMC.DebugRoom
{
    public abstract class DebugSection : MonoBehaviour
    {
        public string title;
        public GameObject overlayObject;

        public DebugRoomManager manager { get; private set; }
        public ImUIManager imUI => manager.currentPage.imUI;

        protected bool overlay;

        protected ImUIBuilder ui { get; private set; }

        public void Setup(DebugRoomManager manager)
        {
            this.manager = manager;
            if (overlayObject != null)
            {
                overlayObject.SetActive(overlay);
                MemberBinder.Bind(overlayObject);
            }
            Setup();
        }

        public void BuildView(ImUIBuilder ui)
        {
            this.ui = ui;
            ui.Tab(title, 20, () =>
            {
                OnUI();
            });
            if (overlayObject != null) overlayObject.SetActive(overlay);
        }

        protected void UIOverlayToggle()
        {
            overlay = ui.Toggle("Overlay", overlay);
        }

        protected virtual void Setup() { }
        protected abstract void OnUI();
    }
}