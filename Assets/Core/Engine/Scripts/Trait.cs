using System;
using UnityEngine;

namespace MMC.EngineCore
{
    public abstract class Trait : LifecycleObject
    {
        public Entity entity { get; private set; }
        public TraitView prefab { get; private set; }

        public Engine engine => entity.engine;

        public string key => prefab.key;

        public void _Setup(Entity entity, TraitView prefab)
        {
            this.entity = entity;
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
