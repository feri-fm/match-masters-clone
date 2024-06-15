using UnityEngine;
using UnityEditor;
using System;

namespace Match3
{
    [CustomPropertyDrawer(typeof(TileColor))]
    public class TileColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect p, SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("value");

            TileColorSamples sampleColor = Enum.IsDefined(typeof(TileColorSamples), value.intValue) ? (TileColorSamples)value.intValue : TileColorSamples.None;

            var newColor = (TileColorSamples)EditorGUI.EnumPopup(new Rect(p.x, p.y, 100, p.height), sampleColor);

            if (newColor != TileColorSamples.None)
            {
                value.intValue = (int)newColor;
            }

            var newValue = EditorGUI.IntField(new Rect(p.x + 110, p.y, 50, 20), value.intValue);
            if (newValue != value.intValue)
            {
                value.intValue = newValue;
            }
        }
    }
}