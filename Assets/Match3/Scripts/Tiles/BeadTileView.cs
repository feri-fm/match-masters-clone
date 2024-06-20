using UnityEngine;
using Core;
using System.Threading.Tasks;

namespace Match3
{
    public class BeadTileView : ColoredTileView<BeadTile>
    {
        public override Entity CreateEntity() => new BeadTile();
    }

    public class BeadTile : ColoredTile<BeadTileView>
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
            engine.RemoveEntity(this);
        }
    }
}
