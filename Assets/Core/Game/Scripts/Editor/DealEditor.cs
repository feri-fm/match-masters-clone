using UnityEngine;
using UnityEditor;

namespace MMC.Game
{
    [CustomEditor(typeof(Deal))]
    public class DealEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var deal = target as Deal;
            if (GUILayout.Button($"Rename to [{deal.GetName()}]"))
            {
                string assetPath = AssetDatabase.GetAssetPath(deal.GetInstanceID());
                AssetDatabase.RenameAsset(assetPath, deal.GetName());
                Selection.activeObject = deal;
                // AssetDatabase.SaveAssets();
            }
        }
    }
}