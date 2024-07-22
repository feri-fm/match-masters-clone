using UnityEngine;
using MMC.EngineCore;
using System.Threading.Tasks;

namespace MMC.Match3
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
            AddTrait<ScoreTraitView>();
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            engine.RemoveEntity(this);
        }

        public override string ToString()
        {
            return base.ToString() + " | Bead " + color.ToString();
        }
    }
}
