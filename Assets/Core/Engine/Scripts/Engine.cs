using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMC.EngineCore
{
    public class Engine
    {
        public readonly List<Entity> entities = new();
        public readonly Dictionary<Id, Entity> entityById = new();

        public EngineConfig config { get; private set; }

        public readonly IdentifierGenerator identifierGenerator = new();
        public readonly Evaluator evaluator = new();

        public EventManger events = new();

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
            return CreateEntity(prefab, id, entities.Count);
        }
        public Entity CreateEntity(EntityView prefab, Id id, int index)
        {
            var entity = prefab.CreateEntity();
            entities.Insert(index, entity);
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

        public Entity GetEntity(Id id)
        {
            return entities.Find(e => e.id == id);
        }
        public T GetEntity<T>(Id id) where T : Entity
        {
            return entities.Find(e => e.id == id) as T;
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

        public EngineData Save()
        {
            return new EngineData()
            {
                identifier = identifierGenerator.Save(),
                entities = entities.Select(e => e.Save()).ToArray(),
            };
        }
        public void Load(EngineData data)
        {
            Clear();
            identifierGenerator.Load(data.identifier);
            var _entities = new Entity[data.entities.Length];
            for (int i = 0; i < data.entities.Length; i++)
            {
                var entityData = data.entities[i];
                var prefab = config.GetEntity(entityData.key);
                var entity = CreateEntity(prefab, entityData.id);
                _entities[i] = entity;
            }
            for (int i = 0; i < _entities.Length; i++)
            {
                _entities[i].Load(data.entities[i]);
            }
            for (int i = 0; i < _entities.Length; i++)
            {
                _entities[i].PostLoad(data.entities[i]);
            }
        }

        public Engine CopyEngine()
        {
            var engine = new Engine(config);
            var data = Save();
            engine.Load(data);
            return engine;
        }
    }

    public class EngineData
    {
        public IdentifierGeneratorData identifier;
        public EntityData[] entities;
    }
}
