using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMC.EngineCore
{
    [RequireComponent(typeof(ObjectPool))]
    public class EngineView : MonoBehaviour
    {
        public Transform container;
        public float scale = 1;
        public int preview = 3;

        public Engine engine { get; private set; }
        public List<EntityView> views { get; } = new();
        public Dictionary<Id, EntityView> viewById { get; } = new();
        public Dictionary<Entity, EntityView> viewByEntity { get; } = new();

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
                CreateView(entity);
            }
        }

        public void Clear()
        {
            if (engine != null)
            {
                engine.onEntityCreated -= OnCreateEntity;
                engine.onEntityRemoved -= OnRemoveEntity;
            }

            foreach (var view in views)
            {
                view._Remove();
                view.Pool();
            }

            views.Clear();
            viewById.Clear();
            viewByEntity.Clear();
            engine = null;
        }

        private void OnCreateEntity(Entity entity) => CreateView(entity);
        private void OnRemoveEntity(Entity entity) => RemoveView(GetViewById(entity.id));
        public EntityView CreateView(Entity entity)
        {
            var prefab = entity.prefab;
            var view = pool.Spawn(prefab, container);
            views.Add(view);
            viewById.Add(entity.id, view);
            viewByEntity.Add(entity, view);
            view._Setup(this, entity);
            return view;
        }
        public void RemoveView(EntityView view)
        {
            view._Remove();
            view.Pool();
            views.Remove(view);
            viewById.Remove(view.id);
            viewByEntity.Remove(view.entity);
        }

        public EntityView GetViewById(Id id)
        {
            if (viewById.TryGetValue(id, out var view))
                return view;
            return null;
        }
        public EntityView GetView(Entity entity)
        {
            if (viewByEntity.TryGetValue(entity, out var view))
                return view;
            return null;
        }

        public Vector3 GetPosition(Vector2 point)
        {
            var p = new Vector2(point.x, point.y);
            return container.position + container.TransformDirection(p * scale);
        }
        public Vector3 GetPosition(Int2 point) => GetPosition((Vector2)point);

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
            for (int i = 0; i < preview; i++)
            {
                for (int j = 0; j < preview; j++)
                {
                    Gizmos.DrawWireSphere(GetPosition(new Int2(i, j)), scale * 0.2f);
                }
            }
        }
    }
}
