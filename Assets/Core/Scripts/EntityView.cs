using System.Collections.Generic;

namespace Core
{
    public abstract class EntityView : LifecycleObjectView
    {
        public string key;

        public List<TraitView> traits { get; } = new();

        public EngineView engine { get; private set; }
        public Entity entity { get; private set; }

        public Id id => entity.id;

        public abstract Entity CreateEntity();

        public void _Setup(EngineView engine, Entity entity)
        {
            this.engine = engine;
            this.entity = entity;
            entity.onTraitCreated += OnCreateTrait;
            entity.onTraitRemoved += OnRemoveTrait;
            foreach (var trait in entity.traits)
            {
                CreateTrait(trait);
            }
            name = key + "_" + id;
            __Setup(entity);
        }
        public void _Remove()
        {
            foreach (var trait in traits)
            {
                trait._Remove();
                trait.Pool();
            }
            traits.Clear();
            entity.onTraitCreated -= OnCreateTrait;
            entity.onTraitRemoved -= OnRemoveTrait;
            __Remove(entity);
        }

        private void OnCreateTrait(Trait trait) => CreateTrait(trait);
        private void OnRemoveTrait(Trait trait) => RemoveTrait(GetViewByTrait(trait));
        private TraitView CreateTrait(Trait trait)
        {
            var prefab = trait.prefab;
            var view = engine.pool.Spawn(prefab, transform);
            traits.Add(view);
            view._Setup(this, trait);
            return view;
        }
        private void RemoveTrait(TraitView view)
        {
            view._Remove();
            view.Pool();
            traits.Remove(view);
        }

        public TraitView GetViewByTrait(Trait trait)
        {
            return traits.Find(e => e.trait == trait);
        }
    }
    public abstract class EntityView<T> : EntityView where T : Entity
    {
        public new T entity => base.entity as T;
    }
}
