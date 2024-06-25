using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using MMC.Core;
using TMPro;
using UnityEditor.Rendering;

namespace MMC.Match3
{
    [CustomPropertyDrawer(typeof(MatchPattern))]
    public class GameConfigMatchItemDrawer : PropertyDrawer
    {
        private Cell[,] cells;
        private const int SPACE = 22;
        private const int SIZE = 20;
        private const int MARGIN = 12;

        private EditMode editMode;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var target = property.GetValue<MatchPattern>();

            var sp = new Vector2(position.x + 6 * SIZE, position.y + MARGIN);

            if (GUI.Button(new Rect(sp.x, sp.y, 100, 20), editMode.ToString()))
            {
                editMode = editMode == EditMode.Point ? EditMode.Reward : EditMode.Point;
            }

            target.width = Mathf.Clamp(EditorGUI.IntField(new Rect(sp.x, sp.y + 23, 45, 20), target.width), 1, 5);
            target.height = Mathf.Clamp(EditorGUI.IntField(new Rect(sp.x + 54, sp.y + 23, 45, 20), target.height), 1, 5);
            EditorGUI.PropertyField(new Rect(sp.x, sp.y + 46, 100, 20), property.FindPropertyRelative(nameof(target.reward)), GUIContent.none);

            var width = target.width;
            var height = target.height;

            if (cells == null || cells.GetLength(0) != width || cells.GetLength(1) != height)
            {
                cells = new Cell[width, height];
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        cells[i, j] = new Cell()
                        {
                            x = i,
                            y = j,
                        };
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j].point = false;
                    cells[i, j].reward = false;
                }
            }

            foreach (var point in target.points)
            {
                if (point.x < width && point.y < height)
                    cells[point.x, point.y].point = true;
            }
            foreach (var point in target.rewardPoints)
            {
                if (point.x < width && point.y < height)
                    cells[point.x, point.y].reward = true;
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (editMode == EditMode.Point)
                        cells[i, j].point = Toggle(i, j, cells[i, j].point, cells[i, j]);
                    else if (editMode == EditMode.Reward)
                        cells[i, j].reward = Toggle(i, j, cells[i, j].reward, cells[i, j]);
                }
            }

            var newPoints = new List<Int2>();
            var newRewards = new List<Int2>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (cells[i, j].point)
                    {
                        newPoints.Add(new Int2(i, j));
                    }
                    if (cells[i, j].reward)
                    {
                        newRewards.Add(new Int2(i, j));
                    }
                }
            }
            target.points = newPoints.ToArray();
            target.rewardPoints = newRewards.ToArray();

            bool Toggle(int i, int j, bool value, Cell cell)
            {
                var text = cell.reward ? "X" : " ";
                var color = cell.point ? Color.red : Color.white;

                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
                if (GUI.Button(new Rect(new Vector2(i, j) * SPACE + position.position + Vector2.up * MARGIN, Vector2.one * SIZE), text))
                {
                    GUI.backgroundColor = oldColor;
                    return !value;
                }
                GUI.backgroundColor = oldColor;
                return value;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var target = property.GetValue<MatchPattern>();
            var height = target.height;
            return Mathf.Max(height, 3) * SPACE + MARGIN * 2;
        }

        protected class Cell
        {
            public int x;
            public int y;
            public bool point;
            public bool reward;
        }

        protected enum EditMode
        {
            Point, Reward
        }
    }
}
