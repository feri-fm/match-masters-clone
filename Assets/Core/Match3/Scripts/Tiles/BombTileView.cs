using System.Threading.Tasks;
using MMC.EngineCore;
using UnityEngine;

namespace MMC.Match3
{
    public class BombTileView : ColoredTileView<BombTile>
    {
        public TileSpriteColor target;
        public GameObject clearEffect;

        public MatchPattern area;

        public override Entity CreateEntity() => new BombTile();

        protected override void OnSetup()
        {
            base.OnSetup();
            var r = clearEffect.GetComponent<SpriteRenderer>();
            var c = target.GetColor();
            r.color = new Color(c.r, c.g, c.b, r.color.a);
            clearEffect.SetActive(false);
            tile.onHit += () =>
            {
                clearEffect.SetActive(true);
            };
        }
    }

    public class BombTile : ColoredTile<BombTileView>
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
            var offset = position - Int2.one * 2;
            foreach (var point in prefab.area.points)
            {
                var p = point + offset;
                if (game.ValidatePoint(p))
                {
                    var tile = game.GetTileAt(p);
                    if (tile != null)
                    {
                        _ = tile.Hit();
                    }
                }
            }
            await game.Wait(0.3f);
            engine.RemoveEntity(this);
        }
    }
}
