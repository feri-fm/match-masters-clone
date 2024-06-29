using System.Collections.Generic;
using MMC.EngineCore;
using ImUI;
using MMC.Match3;
using UnityEngine;

namespace MMC.DebugRoom
{
    public class SaveSection : DebugSection
    {
        private List<string> saves = new();

        private string saveKey = "";
        private int keyCounter;

        protected override void OnUI()
        {
            ui.BeginHorizontal(50);
            ui.StashIndent(0);
            saveKey = ui.Text("Name", saveKey, new VPLayoutFlexibleWidth(100));
            ui.disabled = saveKey == "";
            if (ui.Button("X", new VPLayoutMinWidth(80)))
            {
                saveKey = "";
            }
            ui.disabled = false;
            ui.LoadIndent();
            ui.EndLayout();

            if (saveKey != "")
            {
                if (ui.Button("Save"))
                {
                    SaveGame(saveKey);
                }
            }
            else
            {
                if (ui.Button("Generate Name"))
                {
                    saveKey = "save_" + keyCounter;
                    keyCounter += 1;
                    PlayerPrefs.SetInt(nameof(keyCounter), keyCounter);
                }
            }

            ui.Tab("saves", $"Saves ({saves.Count})", () =>
            {
                if (saves.Count == 0)
                    ui.Label("No save found");
                foreach (var key in saves.ToArray())
                {
                    ui.BeginHorizontal(50);
                    ui.StashIndent(0);
                    if (ui.Button(key, new VPLayoutFlexibleWidth(100)))
                    {
                        LoadGame(key);
                    }
                    if (ui.Button("X", new VPLayoutMinWidth(70)))
                    {
                        RemoveSavedGame(key);
                    }
                    ui.LoadIndent();
                    ui.EndLayout();
                }
            });
        }

        private void Start()
        {
            keyCounter = PlayerPrefs.GetInt(nameof(keyCounter), keyCounter);
            saves = PlayerPrefs.GetString("saves", "[]").FromJson<List<string>>();
        }

        public SaveSectionData Save()
        {
            return new SaveSectionData()
            {
                engine = manager.GetSection<GameSection>().Save(),
                stats = manager.GetSection<StatsSection>().Save(),
            };
        }
        public void Load(SaveSectionData data)
        {
            manager.GetSection<GameSection>().Load(data.engine);
            manager.GetSection<StatsSection>().Load(data.stats);
        }

        public string GetKey(string key) => "save_" + key;
        public void SaveGame(string key)
        {
            var data = Save();
            PlayerPrefs.SetString(GetKey(key), data.ToJson());
            if (!saves.Contains(key))
            {
                saves.Add(key);
                PlayerPrefs.SetString("saves", saves.ToJson());
            }
        }
        public void LoadGame(string key)
        {
            var data = PlayerPrefs.GetString(GetKey(key)).FromJson<SaveSectionData>();
            if (data != null)
            {
                Load(data);
                saveKey = key;
            }
        }
        public void RemoveSavedGame(string key)
        {
            PlayerPrefs.DeleteKey(GetKey(key));
            saves.Remove(key);
            PlayerPrefs.SetString("saves", saves.ToJson());

            if (saves.Count == 0)
            {
                keyCounter = 0;
                PlayerPrefs.SetInt(nameof(keyCounter), keyCounter);
            }
        }
    }

    public class SaveSectionData
    {
        public EngineData engine;
        public StatsData stats;
    }
}