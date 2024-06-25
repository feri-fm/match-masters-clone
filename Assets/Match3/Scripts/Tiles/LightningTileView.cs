using System.Collections.Generic;
using System.Threading.Tasks;
using MMC.Core;
using UnityEngine;

namespace MMC.Match3
{
    public class LightningTileView : ColoredTileView<LightningTile>
    {
        public int count = 10;
        public LineRenderer lineRenderer;
        public SpriteRenderer spriteRenderer;

        public override Entity CreateEntity() => new LightningTile();

        protected override void OnRender()
        {
            base.OnRender();
            lineRenderer.startColor = spriteRenderer.color;
            lineRenderer.endColor = spriteRenderer.color;
            lineRenderer.positionCount = entity.targets.Count * 2;
            for (int i = 0; i < entity.targets.Count; i++)
            {
                lineRenderer.SetPosition(i * 2, engine.GetPosition(entity.position) + Vector3.back);
                lineRenderer.SetPosition(i * 2 + 1, engine.GetPosition(entity.targets[i]) + Vector3.back);
            }
        }
    }

    public class LightningTile : ColoredTile<LightningTileView>
    {
        public List<Int2> targets = new();

        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<AnimatorTraitView>();
            AddTrait<PowerUpTraitView>();
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            targets.Clear();
            var targetTiles = new List<Tile>();
            for (int i = 0; i < prefab.count; i++)
            {
                var x = game.RandInt(game.width);
                var y = game.RandInt(game.height);
                var point = new Int2(x, y);
                var tile = game.GetTileAt(point);
                if (tile != null)
                {
                    targetTiles.Add(tile);
                    targets.Add(tile.position);
                }
            }
            Changed();
            await game.Wait(0.2f);
            for (int i = 0; i < targetTiles.Count; i++)
            {
                _ = targetTiles[i].Hit();
            }
            await game.Wait(0.5f);
            engine.RemoveEntity(this);
        }
    }
}
