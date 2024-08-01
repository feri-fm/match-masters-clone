using UnityEngine;
using UnityEditor;

namespace MMC.Match3
{
    [CustomPropertyDrawer(typeof(MatchRotation))]
    public class MatchRotationDrawer : PropertyDrawer
    {
        private const int SPACE = 2;
        private const int WIDTH = 45;
        private const int HEIGHT = 20;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var target = property.GetValue<MatchRotation>();

            for (int i = 0; i < 4; i++)
            {
                var selected = target.rotations.Contains(i);
                var text = selected ? $"{target.rotations.IndexOf(i)}:{i * 90}" : $"{i * 90}";
                var color = selected ? Color.blue : Color.white;

                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
                if (GUI.Button(new Rect(new Vector2(i, 0) * (WIDTH + SPACE) + position.position + Vector2.up * SPACE, new Vector2(WIDTH, HEIGHT)), text))
                {
                    if (selected) target.rotations.Remove(i);
                    else target.rotations.Add(i);
                }
                GUI.backgroundColor = oldColor;
            }

            target.flip = Toggle(new Rect(new Vector2((WIDTH + SPACE) * 4, SPACE) + position.position, new Vector2(WIDTH, HEIGHT)),
                target.flip, "Flip", target.flip ? Color.green : Color.white);

            EditorGUI.EndProperty();
        }


        private bool Toggle(Rect rect, bool value, string text, Color color)
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            if (GUI.Button(rect, text))
            {
                GUI.backgroundColor = oldColor;
                return !value;
            }
            GUI.backgroundColor = oldColor;
            return value;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetPropertyHeight(property);
        }

        public static float GetPropertyHeight(SerializedProperty property)
        {
            return HEIGHT * 2 + SPACE * 4;
        }
    }
}
