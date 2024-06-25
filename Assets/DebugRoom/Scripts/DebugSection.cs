using ImUI;
using UnityEngine;

namespace DebugRoom
{
    public abstract class DebugSection : MonoBehaviour
    {
        public string title;

        public DebugRoomManager manager { get; private set; }
        public ImUIManager imUI => manager.currentPage.imUI;

        protected ImUIBuilder ui { get; private set; }

        public void Setup(DebugRoomManager manager)
        {
            this.manager = manager;
            Setup();
        }

        public void BuildView(ImUIBuilder ui)
        {
            this.ui = ui;
            ui.Tab(title, 20, () =>
            {
                OnUI();
            });
        }

        protected virtual void Setup() { }
        protected abstract void OnUI();
    }
}