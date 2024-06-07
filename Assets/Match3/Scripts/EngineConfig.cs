using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(fileName = "EngineConfig", menuName = "EngineConfig")]
    public class EngineConfig : ScriptableObject
    {
        public List<TileView> tiles;
        public List<TraitView> traits;

        public TileView GetTile(string key)
        {
            return tiles.Find(e => e.key == key);
        }
        public T GetTile<T>() where T : TileView
        {
            return tiles.Find(e => e is T) as T;
        }
        public TraitView GetTrait(string key)
        {
            return traits.Find(e => e.key == key);
        }
        public T GetTrait<T>() where T : TraitView
        {
            return traits.Find(e => e is T) as T;
        }
    }
}
