
using Core;
using UnityEngine;

namespace Match3
{
    public abstract class TileView : EntityView<Tile>
    {
        protected override void OnRender()
        {
            base.OnRender();
            transform.position = engine.GetPosition(entity.position);
        }
    }

    public abstract class Tile : Entity
    {
        public Int2 position;

        public ColorTrait colorTrait => _colorTrait ??= GetTrait<ColorTrait>(); ColorTrait _colorTrait;

        public virtual void Hit(float time) { }
    }

    public abstract class TileView<T> : TileView where T : Tile
    {
        public new T entity => base.entity as T;
    }
    public abstract class Tile<T> : Tile where T : TileView
    {
        public new T prefab => base.prefab as T;
    }
}
