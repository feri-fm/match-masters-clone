using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public class Engine
    {
        public readonly List<Entity> entities = new();
        public readonly Dictionary<Id, Entity> entityById = new();

        public EngineConfig config { get; private set; }

        public readonly IdentifierGenerator identifierGenerator = new();
        public readonly Evaluator evaluator = new();

        public event Action<Entity> onEntityCreated = delegate { };
        public event Action<Entity> onEntityRemoved = delegate { };

        public Func<float, Task> waiter;

        public Engine(EngineConfig config)
        {
            this.config = config;
        }

        public void Clear()
        {
            foreach (var entity in entities)
            {
                entity._Remove();
                onEntityRemoved.Invoke(entity);
            }
            entities.Clear();
            entityById.Clear();
            identifierGenerator.Clear();
            evaluator.Clear();
        }

        public Entity CreateEntity<TView>() where TView : EntityView => CreateEntity(config.GetEntity<TView>());
        public Entity CreateEntity(string key)
        {
            var prefab = config.GetEntity(key);
            return CreateEntity(prefab);
        }
        public Entity CreateEntity(EntityView prefab)
        {
            var id = identifierGenerator.Generate();
            return CreateEntity(prefab, id);
        }
        public Entity CreateEntity(EntityView prefab, Id id)
        {
            var entity = prefab.CreateEntity();
            entities.Add(entity);
            entityById.Add(id, entity);
            entity._Setup(this, prefab, id);
            onEntityCreated.Invoke(entity);
            return entity;
        }
        public void RemoveEntity(Entity entity)
        {
            entity._Remove();
            entities.Remove(entity);
            entityById.Remove(entity.id);
            onEntityRemoved.Invoke(entity);
        }

        public void Evaluate()
        {
            evaluator.Evaluate(entities.ToArray());
        }

        public T GetEntity<T>() where T : Entity
        {
            return entities.Find(e => e is T) as T;
        }

        public List<Entity> GetEntitiesWithTrait<T>() where T : Trait
        {
            var res = new List<Entity>();
            foreach (var entity in entities)
                if (entity.HasTrait<T>())
                    res.Add(entity);
            return res;
        }
        public void ForEntityWithTrait<T>(Action<Entity> action) where T : Trait
        {
            foreach (var entity in entities)
            {
                if (entity.HasTrait<T>())
                    action.Invoke(entity);
            }
        }

        public async Task Wait(float time)
        {
            if (waiter != null)
                await waiter.Invoke(time);
        }
    }
}
