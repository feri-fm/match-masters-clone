using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using MMC.EngineCore;
using TMPro;
using UnityEditor.Rendering;
using System.Linq;

namespace MMC.Match3
{
    [CustomPropertyDrawer(typeof(MatchPatternRotator))]
    public class MatchPatternRotatorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var target = property.GetValue<MatchPatternRotator>();

            var patternProp = property.FindPropertyRelative("pattern");
            var patternHeight = MatchPatternDrawer.GetPropertyHeight(patternProp);
            var pattern = patternProp.GetValue<MatchPattern>();

            var rotationProp = property.FindPropertyRelative("rotation");
            var rotationHeight = MatchRotationDrawer.GetPropertyHeight(rotationProp);
            var rotation = rotationProp.GetValue<MatchRotation>();

            EditorGUI.PropertyField(rect, patternProp);
            EditorGUI.PropertyField(new Rect(rect.position + Vector2.up * patternHeight, rect.size), rotationProp);

            //TODO: add a preview of all rotated patterns
            // EditorGUI.BeginDisabledGroup(true);
            // EditorGUILayout.PropertyField(property, true);
            // EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var patternProp = property.FindPropertyRelative("pattern");
            var rotationProp = property.FindPropertyRelative("rotation");

            var patternHeight = MatchPatternDrawer.GetPropertyHeight(patternProp);
            var rotationHeight = MatchRotationDrawer.GetPropertyHeight(rotationProp);

            return patternHeight + rotationHeight + 8;
        }
    }
}
