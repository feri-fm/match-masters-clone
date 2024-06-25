using ImUI;
using UnityEngine;

namespace MMC.DebugRoom
{
    public class DebugPage : PoolObject
    {
        public ImUIManager imUI;

        public string title;

        private DebugRoomManager manager;

        public void Setup(DebugRoomManager manager)
        {
            this.manager = manager;
            title = "p" + manager.pageCounter++;

            imUI.SetViewBuilder(ui =>
            {
                ui.Title(title);

                ui.Row(() =>
                {
                    title = ui.Text("Title", title, new VPLayoutFlexibleWidth(100));
                    if (manager.pages.Count > 1)
                    {
                        if (ui.Button("X", new VPLayoutMinWidth(60)))
                        {
                            manager.RemovePage(this);
                        }
                    }
                });

                foreach (var section in manager.sections)
                {
                    section.BuildView(ui);
                }

                manager.MarkDirty();
            });
        }

        public void SetPage(bool value)
        {
            if (gameObject.activeSelf != value)
            {
                gameObject.SetActive(value);
                if (value) imUI.Changed();
            }
        }

        public void Select()
        {
            manager.SetPage(this);
        }
    }
}