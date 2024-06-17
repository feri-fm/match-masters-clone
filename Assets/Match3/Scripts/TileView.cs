
using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace Match3
{
    public abstract class TileView : EntityView<Tile>
    {
        public float smooth = 10;

        private bool started;

        public Tile tile => entity;
        public Game game => tile.game;

        protected override void OnSetup()
        {
            base.OnSetup();
            started = false;
            transform.position = new Vector3(1000, 1000, 0);
        }

        protected override void OnRender()
        {
            base.OnRender();
            if (!started)
            {
                started = true;
                Jump();
            }
            // transform.position = engine.GetPosition(entity.position);
        }

        protected override void Update()
        {
            base.Update();
            transform.position = Vector3.Lerp(transform.position, engine.GetPosition(entity.position), smooth * Time.deltaTime);
        }

        public void Jump()
        {
            transform.position = engine.GetPosition(entity.position + Int2.up);
        }
    }

    public abstract class Tile : Entity
    {
        public Int2 position;

        public Game game { get; private set; }

        public bool isHit { get; private set; }

        public void SetupGame(Game game)
        {
            this.game = game;
        }

        public async Task Hit()
        {
            if (!isHit)
            {
                isHit = true;
                await OnHit();
            }
        }

        protected virtual Task OnHit()
        {
            return Task.CompletedTask;
        }
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
