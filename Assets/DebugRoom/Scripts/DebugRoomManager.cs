using System;
using System.Collections;
using System.Collections.Generic;
using ImUI;
using UnityEngine;

namespace DebugRoom
{
    public class DebugRoomManager : MonoBehaviour
    {
        public ImUIManager imUIManager;
        public float scale = 1;
        public bool toggle;
        public int width = 7;
        public int height = 7;
        public int beads = 6;
        public int minBeads = 6;
        public int maxBeads = 10;

        private List<string> tabs = new();

        private void Start()
        {
            imUIManager.SetViewBuilder(b =>
            {
                b.Title("Debug Room");
                b.Label("This is debug room");
                Tab("Game", () =>
                {
                    b.Number("Seed", 0);
                    scale = b.Slider("Scale", scale, 0, 1);
                    toggle = b.Toggle("Toggle", toggle);
                    width = b.Slider("Width", width, 2, 16);
                    height = b.Slider("Height", height, 2, 16);
                    beads = b.Slider("Beads", beads, 2, 6);
                    minBeads = b.Slider("Min Bead", minBeads, 0, 20);
                    maxBeads = b.Slider("Max Bead", maxBeads, 0, 30);
                    toggle = b.Toggle("Toggle", toggle);
                    scale = b.Slider("Scale", scale, 0, 1);
                    b.Button("Reload");
                });
                Tab("Network", () =>
                {
                    b.Label("This is Network tab");
                });
                Tab("Programs", () =>
                {
                    Tab("Shuffle", () =>
                    {
                        b.Number("Count", 0);
                        b.Number("Max Count", 0);
                        b.Button("Shuffle");
                    });
                    Tab("Hammer", () =>
                    {
                        b.Button("Hammer");
                    });
                    Tab("Rocket", () =>
                    {
                        b.Number("Count", 0);
                        b.Number("Max Count", 0);
                        b.Button("Rocket");
                    });
                    Tab("Duck", () =>
                    {
                        b.Number("Point Y", 0);
                        b.Button("Duck");
                    });
                    Tab("Bucket", () =>
                    {
                        b.Number("Count", 0);
                        b.Number("Max Count", 0);
                        b.Button("Bucket");
                    });
                    Tab("Hat", () =>
                    {
                        b.Number("Count", 0);
                        b.Number("Max Count", 0);
                        b.Button("Hat");
                    });
                });
                Tab("Go Forth", () =>
                {
                    b.Label("Rick prime");
                });

                void Tab(string tabName, Action content)
                {
                    if (b.Button(tabName + (tabs.Contains(tabName) ? " <" : " >")))
                    {
                        if (tabs.Contains(tabName))
                            tabs.Remove(tabName);
                        else
                            tabs.Add(tabName);
                    }
                    if (tabs.Contains(tabName))
                    {
                        b.indent += 40;
                        content.Invoke();
                        b.Space(15);
                        b.indent -= 40;
                    }
                }
            });
        }
    }
}
