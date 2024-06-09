using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(ObjectPool))]
    public class EngineView : MonoBehaviour
    {
        public Transform container;
        public float scale = 1;
        public int preview = 3;

        public Engine engine { get; private set; }
        public List<EntityView> entities { get; } = new();
        public Dictionary<Id, EntityView> entitybyId { get; } = new();

        public ObjectPool pool { get; private set; }

        private void Awake()
        {
            pool = GetComponent<ObjectPool>();
        }

        public void Setup(Engine engine)
        {
            Clear();

            this.engine = engine;
            engine.onEntityCreated += OnCreateEntity;
            engine.onEntityRemoved += OnRemoveEntity;

            foreach (var entity in engine.entities)
            {
                CreateEntity(entity);
            }
        }

        public void Clear()
        {
            if (engine != null)
            {
                engine.onEntityCreated -= OnCreateEntity;
                engine.onEntityRemoved -= OnRemoveEntity;
            }

            foreach (var entity in entities)
            {
                entity._Remove();
                entity.Pool();
            }

            entities.Clear();
            entitybyId.Clear();
            engine = null;
        }

        private void OnCreateEntity(Entity entity) => CreateEntity(entity);
        private void OnRemoveEntity(Entity entity) => RemoveEntity(GetEntityById(entity.id));
        private EntityView CreateEntity(Entity entity)
        {
            var prefab = entity.prefab;
            var view = pool.Spawn(prefab, container);
            entities.Add(view);
            entitybyId.Add(entity.id, view);
            view._Setup(this, entity);
            return view;
        }
        private void RemoveEntity(EntityView view)
        {
            view._Remove();
            view.Pool();
            entities.Remove(view);
            entitybyId.Remove(view.id);
        }

        public EntityView GetEntityById(Id id)
        {
            if (entitybyId.TryGetValue(id, out var view))
                return view;
            return null;
        }

        public Vector3 GetPosition(Int2 point)
        {
            var p = new Vector2(point.x, point.y);
            return container.position + container.TransformDirection(p * scale);
        }
        public Int2 GetPoint(Vector3 position)
        {
            var rel = container.InverseTransformDirection(position - container.position);
            rel /= scale;
            var x = Mathf.RoundToInt(rel.x);
            var y = Mathf.RoundToInt(rel.y);
            return new Int2(x, y);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            DrawGizmos();
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            DrawGizmos();
        }
        private void DrawGizmos()
        {
            for (int i = -preview; i <= preview; i++)
            {
                for (int j = -preview; j <= preview; j++)
                {
                    Gizmos.DrawWireSphere(GetPosition(new Int2(i, j)), scale * 0.2f);
                }
            }
        }
    }
}
