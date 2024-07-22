using UnityEngine;
using UnityEditor;

namespace MMC.EngineCore
{
    [CustomEditor(typeof(EngineConfig))]
    public class EngineConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Fetch"))
            {
                var config = target as EngineConfig;
                config.entities.Clear();
                config.traits.Clear();
                foreach (var guid in AssetDatabase.FindAssets("t:prefab"))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    if (asset != null)
                    {
                        var entity = asset.GetComponent<EntityView>();
                        if (entity != null)
                            config.entities.Add(entity);
                        var trait = asset.GetComponent<TraitView>();
                        if (trait != null)
                            config.traits.Add(trait);
                    }
                }
                EditorUtility.SetDirty(target);
            }
        }
    }
}
