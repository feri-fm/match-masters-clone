using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace Match3
{
    public class LightningTileView : ColoredTileView<LightningTile>
    {
        public int count = 10;

        public override Entity CreateEntity() => new LightningTile();
    }

    public class LightningTile : ColoredTile<LightningTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<AnimatorTraitView>();
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            await engine.Wait(0.2f);
            for (int i = 0; i < prefab.count; i++)
            {
                var x = game.RandInt(game.width);
                var y = game.RandInt(game.height);
                var point = new Int2(x, y);
                var tile = game.GetTileAt(point);
                if (tile != null)
                {
                    _ = tile.Hit();
                }
            }
            await engine.Wait(0.2f);
            engine.RemoveEntity(this);
        }
    }
}
