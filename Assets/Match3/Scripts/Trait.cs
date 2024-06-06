using UnityEngine;

namespace Match3
{
    public abstract class TraitView : PoolObject
    {
        public string key;

        public abstract Trait CreateTrait();
    }
    public abstract class TraitView<T> : TraitView where T : Trait
    {

    }

    public abstract class Trait
    {
        public Tile tile { get; private set; }
        public TileView prefab { get; private set; }

        public string key => prefab.key;

        public virtual void OnCreated() { }
        public virtual void OnRemoved() { }

        public void _Setup(Tile tile, TileView prefab) { }
        public void _Remove() { }
    }
}
