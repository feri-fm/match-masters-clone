using System;
using System.Threading.Tasks;
using MMC.EngineCore;
using UnityEngine;

namespace MMC.Match3
{
    public class ClearLineTileView : ColoredTileView<ClearLineTile>
    {
        public SpriteRenderer target;
        public GameObject clearEffect;
        public ClearLineDirection direction;

        public override Entity CreateEntity() => new ClearLineTile();

        protected override void OnSetup()
        {
            base.OnSetup();
            var r = clearEffect.GetComponent<SpriteRenderer>();
            r.color = new Color(target.color.r, target.color.g, target.color.b, r.color.a); clearEffect.SetActive(false);
            tile.onHit += () =>
            {
                clearEffect.SetActive(true);
            };
        }
    }

    public class ClearLineTile : ColoredTile<ClearLineTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<AnimatorTraitView>();
            AddTrait<ScoreTraitView>();
            AddTrait<PowerUpTraitView>();
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            await game.Wait(0.2f);
            if (prefab.direction == ClearLineDirection.Horizontal)
            {
                for (int i = 0; i <= game.width * 2; i++)
                {
                    var v = i % 2 == 0 ? (position.x + i / 2) : (position.x - (i + 1) / 2);
                    var point = new Int2(v, position.y);
                    if (!game.ValidatePoint(point)) continue;
                    var tile = game.GetTileAt(point);
                    if (tile != null)
                    {
                        _ = tile.Hit();
                        // if (i % 2 == 0) await game.Wait(0.1f);
                    }
                }
            }
            else if (prefab.direction == ClearLineDirection.Vertical)
            {
                for (int i = 0; i <= game.height * 2; i++)
                {
                    var v = i % 2 == 0 ? (position.y + i / 2) : (position.y - (i + 1) / 2);
                    var point = new Int2(position.x, v);
                    if (!game.ValidatePoint(point)) continue;
                    var tile = game.GetTileAt(point);
                    if (tile != null)
                    {
                        _ = tile.Hit();
                        // if (i % 2 == 0) await game.Wait(0.1f);
                    }
                }
            }
            await game.Wait(0.3f);
            engine.RemoveEntity(this);
        }
    }

    public enum ClearLineDirection
    {
        Vertical, Horizontal
    }
}
