using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using ImUI;
using Match3;
using UnityEngine;

namespace DebugRoom
{
    public class DebugRoomManager : MonoBehaviour
    {
        public ImUIManager imUIManager;

        public EngineView engineView;
        public EngineConfig engineConfig;
        public GameOptions gameOptions;

        private List<string> tabs = new();

        private void Start()
        {
            imUIManager.SetViewBuilder(b =>
            {
                b.Title("Debug Room");

                Tab("Game", () =>
                {
                    b.BeginHorizontal(50);
                    var ind = b.indent;
                    b.indent = 0;
                    gameOptions.seed = b.Number("Seed", gameOptions.seed, new VPLayoutFlexibleWidth(100));
                    if (b.Button("Rnd", new VPLayoutMinWidth(70)))
                    {
                        gameOptions.seed = UnityEngine.Random.Range(10000, 99999);
                    }
                    b.indent = ind;
                    b.EndLayout();
                    gameOptions.width = b.Slider("Width", gameOptions.width, 2, 16);
                    gameOptions.height = b.Slider("Height", gameOptions.height, 2, 16);
                    gameOptions.beads = b.Slider("Beads", gameOptions.beads, 2, 6);
                    gameOptions.minBeads = b.Slider("Min Bead", gameOptions.minBeads, 0, 20);
                    gameOptions.maxBeads = b.Slider("Max Bead", gameOptions.maxBeads, 0, 30);
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
