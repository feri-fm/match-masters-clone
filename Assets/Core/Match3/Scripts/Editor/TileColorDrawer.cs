using UnityEngine;
using UnityEditor;
using System;

namespace MMC.Match3
{
    [CustomPropertyDrawer(typeof(TileColor))]
    public class TileColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect p, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(p, label, property);

            if (Selection.objects.Length == 1)
            {
                p = EditorGUI.PrefixLabel(p, GUIUtility.GetControlID(FocusType.Passive), label);

                var value = property.FindPropertyRelative("value");

                TileColorSamples sampleColor = Enum.IsDefined(typeof(TileColorSamples), value.intValue) ? (TileColorSamples)value.intValue : TileColorSamples.None;

                var newColor = (TileColorSamples)EditorGUI.EnumPopup(new Rect(p.x, p.y, p.width - 50, p.height), sampleColor);

                if (newColor != TileColorSamples.None)
                {
                    value.intValue = (int)newColor;
                }
                else
                {
                    value.intValue = -1;
                }

                var newValue = EditorGUI.IntField(new Rect(p.x + p.width - 50 + 2, p.y, 48, 20), value.intValue);
                if (newValue != value.intValue)
                {
                    value.intValue = newValue;
                }
            }
            else
            {
                p = EditorGUI.PrefixLabel(p, GUIUtility.GetControlID(FocusType.Passive), label);
                EditorGUI.Popup(p, 0, new string[] { "multi selection not supported" });
            }

            EditorGUI.EndProperty();
        }
    }
}