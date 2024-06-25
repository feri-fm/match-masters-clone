
using System;
using System.Threading.Tasks;
using MMC.Core;
using UnityEngine;

namespace MMC.Match3
{
    public abstract class TileView : EntityView<Tile>
    {
        public Tile tile => entity;
        public Game game => tile.game;

        public Transform root;

        protected override void OnRender()
        {
            base.OnRender();
            transform.position = engine.GetPosition(entity.position);
        }
    }

    public abstract class Tile : Entity
    {
        public Int2 position;

        public Game game { get; private set; }

        public bool canHit { get; set; }
        public bool isHit { get; private set; }

        public event Action onHit = delegate { };

        public void SetupGame(Game game)
        {
            this.game = game;
        }

        public async Task Hit()
        {
            if (!isHit && canHit)
            {
                isHit = true;
                game.hittings += 1;
                game.OnTileHit(this);
                onHit.Invoke();
                await OnHit();
                game.hittings -= 1;
            }
        }

        protected virtual Task OnHit()
        {
            return Task.CompletedTask;
        }

        public override void Save(JsonData data)
        {
            base.Save(data);
            data.W("p", position);
        }
        public override void Load(JsonData data)
        {
            base.Load(data);
            position = data.R<Int2>("p");
        }
    }

    public abstract class TileView<T> : TileView where T : Tile
    {
        public new T entity => base.entity as T;
        public new T tile => base.tile as T;
    }
    public abstract class Tile<T> : Tile where T : TileView
    {
        public new T prefab => base.prefab as T;
    }
}
