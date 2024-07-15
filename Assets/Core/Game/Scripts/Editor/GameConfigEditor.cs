using UnityEngine;
using UnityEditor;

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
                var config = target as GameConfig;
                config.boosters.Clear();
                config.perks.Clear();
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
                    }
                }
            }
        }
    }
}