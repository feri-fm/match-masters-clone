using UnityEngine;

namespace Match3
{
    public class TileSpriteColor : MonoBehaviour
    {
        public ColoredTileView tile;
        public SpriteRenderer spriteRenderer;
        public Color blue;
        public Color red;
        public Color green;
        public Color yellow;
        public Color orange;
        public Color purple;

        private void OnValidate()
        {
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (spriteRenderer == null) return;
            var c = spriteRenderer.color;
            switch (tile.color.value)
            {
                case (int)TileColorSamples.Blue: c = blue; break;
                case (int)TileColorSamples.Red: c = red; break;
                case (int)TileColorSamples.Green: c = green; break;
                case (int)TileColorSamples.Yellow: c = yellow; break;
                case (int)TileColorSamples.Orange: c = orange; break;
                case (int)TileColorSamples.Purple: c = purple; break;
            }
            spriteRenderer.color = c;
        }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}