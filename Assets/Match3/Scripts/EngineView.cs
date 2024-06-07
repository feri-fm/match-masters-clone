using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class EngineView : MonoBehaviour
    {
        public Engine engine { get; private set; }
        public List<TileView> tiles { get; } = new();

        public void Setup(Engine engine)
        {
            Clear();
            this.engine = engine;
        }

        public void Clear()
        {
            foreach (var tile in tiles)
            {
                tile._Remove();
                tile.Pool();
            }
            tiles.Clear();
        }
    }
}
