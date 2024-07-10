using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MMC.EngineCore
{
    public abstract class Entity : LifecycleObject, IEvaluable
    {
        public List<Trait> traits = new();

        public Id id { get; private set; }
        public Engine engine { get; private set; }
        public EntityView prefab { get; private set; }
        public Evaluable evaluable { get; private set; }

        public event Action<Trait> onTraitCreated = delegate { };
        public event Action<Trait> onTraitRemoved = delegate { };

        public EngineConfig engineConfig => engine.config;

        public string key => prefab.key;

        public void _Setup(Engine engine, EntityView prefab, Id id)
        {
            this.engine = engine;
            this.prefab = prefab;
            this.id = id;
            evaluable = new Evaluable(engine.evaluator);
            __Setup();
        }
        public void _Remove()
        {
            foreach (var trait in traits)
            {
                trait._Remove();
                onTraitRemoved.Invoke(trait);
            }
            traits.Clear();
            evaluable.Clear();
            __Remove();
        }
        public void Changed()
        {
            __Changed();
        }

        public TTrait AddTrait<TView, TTrait>(Action<TTrait> action) where TView : TraitView where TTrait : Trait
        {
            var trait = AddTrait<TView, TTrait>();
            action.Invoke(trait);
            return trait;
        }
        public TTrait AddTrait<TView, TTrait>() where TView : TraitView where TTrait : Trait => AddTrait<TView>() as TTrait;
        public Trait AddTrait<TView>() where TView : TraitView => AddTrait(engineConfig.GetTrait<TView>());
        public Trait AddTrait(TraitView trait) => AddTrait(trait.key);
        public Trait AddTrait(string key)
        {
            var prefab = engineConfig.GetTrait(key);
            var trait = prefab.CreateTrait();
            traits.Add(trait);
            trait._Setup(this, prefab);
            onTraitCreated.Invoke(trait);
            return trait;
        }
        public void RemoveTrait(Trait trait)
        {
            trait._Remove();
            traits.Remove(trait);
            onTraitRemoved.Invoke(trait);
        }

        public Trait GetTrait(string key)
        {
            return traits.Find(e => e.key == key);
        }
        public T GetTrait<T>() where T : Trait
        {
            return traits.Find(e => e is T) as T;
        }
        public bool HasTrait<T>() where T : Trait
        {
            return traits.Any(e => e is T);
        }
        public bool HasTrait(string key)
        {
            return traits.Any(e => e.key == key);
        }
        public bool WithTrait<T>(Action<T> action) where T : Trait
        {
            var trait = GetTrait<T>();
            if (trait != null)
            {
                action.Invoke(trait);
                return true;
            }
            return false;
        }
        public bool WithTrait(string key, Action<Trait> action)
        {
            var trait = GetTrait(key);
            if (trait != null)
            {
                action.Invoke(trait);
                return true;
            }
            return false;
        }
        public R WithTrait<T, R>(Func<T, R> action, R def = default) where T : Trait
        {
            var trait = GetTrait<T>();
            if (trait != null)
                return action.Invoke(trait);
            return def;
        }
        public R WithTrait<R>(string key, Func<Trait, R> action, R def = default)
        {
            var trait = GetTrait(key);
            if (trait != null)
                return action.Invoke(trait);
            return def;
        }

        public virtual void Save(JsonData data) { }
        public virtual void Load(JsonData data) { }
        public virtual void PostLoad(JsonData data) { }

        public virtual string GetDataKey() => key;

        public EntityData Save()
        {
            var json = new JsonData();
            Save(json);
            return new EntityData()
            {
                id = id,
                key = GetDataKey(),
                data = json.json,
            };
        }
        public void Load(EntityData data)
        {
            Load(new JsonData(data.data));
        }
        public void PostLoad(EntityData data)
        {
            PostLoad(new JsonData(data.data));
        }
    }

    public class Entity<TView> : Entity
        where TView : EntityView
    {
        public new TView prefab => base.prefab as TView;
    }

    public class EntityData
    {
        public Id id;
        public string key;
        public JObject data;
    }
}
