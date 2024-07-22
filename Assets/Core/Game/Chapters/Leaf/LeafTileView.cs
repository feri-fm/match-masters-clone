using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;

namespace MMC.Game.Chapters.Leaf
{
    public class LeafTileView : ColoredTileView<LeafTile>
    {
        public override Entity CreateEntity() => new LeafTile();
    }

    public class LeafTile : ColoredTile<LeafTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<AnimatorTraitView>();
            AddTrait<ScoreTraitView, ScoreTrait>().value = 3;
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            engine.RemoveEntity(this);
        }

        public override string ToString()
        {
            return "Leaf " + color.ToString();
        }
    }
}