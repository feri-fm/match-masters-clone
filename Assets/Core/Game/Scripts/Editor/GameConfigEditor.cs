using UnityEngine;
using UnityEditor;
using ConfigFetcher;

namespace MMC.Game
{
    [CustomEditor(typeof(GameConfig))]
    public class GameConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Fetch"))
            {
                new GameConfigFetch().Fetch(target);
            }
        }
    }

    [ConfigFetch(typeof(GameConfig))]
    public class GameConfigFetch : ConfigFetch
    {
        public override void Fetch(Object target)
        {
            var config = target as GameConfig;
            config.boosters.Clear();
            config.perks.Clear();
            config.chapters.Clear();
            config.items.Clear();
            config.deals.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (asset != null)
                {
                    var booster = asset.GetComponent<Booster>();
                    if (booster != null)
                        config.boosters.Add(booster);
                    var perk = asset.GetComponent<Perk>();
                    if (perk != null)
                        config.perks.Add(perk);
                    var chapter = asset.GetComponent<Chapter>();
                    if (chapter != null)
                        config.chapters.Add(chapter);
                }
            }
            foreach (var guid in AssetDatabase.FindAssets("t:Item"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Item)) as Item;
                if (asset != null)
                    config.items.Add(asset);
            }
            foreach (var guid in AssetDatabase.FindAssets("t:Deal"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Deal)) as Deal;
                if (asset != null)
                    config.deals.Add(asset);
            }
            config.chapters.Sort((a, b) => a.trophy.CompareTo(b.trophy));
            EditorUtility.SetDirty(target);
        }
    }
}