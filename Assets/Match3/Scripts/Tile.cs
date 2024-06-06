
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public abstract class TileView : PoolObject
    {
        public string key;

        public List<TraitView> traits { get; } = new();

        public Tile tile { get; private set; }

        public abstract Tile CreateTile();
    }
    public abstract class TileView<T> : TileView where T : Tile
    {

    }

    public abstract class Tile : IEvaluable
    {
        public List<Trait> traits = new();

        public Engine engine { get; private set; }
        public TileView prefab { get; private set; }
        public Evaluable evaluable { get; private set; }

        public string key => prefab.key;

        public virtual void OnCreated() { }
        public virtual void OnRemoved() { }

        public void _Setup(Engine engine, TileView prefab) { evaluable = new Evaluable(engine.evaluator); }
        public void _Remove() { }

        public Trait AddTrait(TraitView trait) => AddTrait(trait.key);
        public Trait AddTrait(string key) => null;
        public void RemoveTrait(Trait trait) { }

        public Trait GetTrait(string key) => null;
        public T GetTrait<T>() where T : Trait => null;
        public bool HasTrait<T>() where T : Trait => false;
        public bool WithTrait<T>(Action<T> action) where T : Trait => false;
    }
}
