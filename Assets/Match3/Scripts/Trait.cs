using System;
using UnityEngine;

namespace Match3
{
    public abstract class Trait : LifecycleObject
    {
        public Tile tile { get; private set; }
        public TraitView prefab { get; private set; }

        public string key => prefab.key;

        public void _Setup(Tile tile, TraitView prefab)
        {
            this.tile = tile;
            this.prefab = prefab;
            __Setup();
        }
        public void _Remove()
        {
            __Remove();
        }

        public void Changed()
        {
            __Changed();
        }
    }

    public class Trait<T> : Trait where T : TraitView
    {
        public new T prefab => base.prefab as T;
    }
}
