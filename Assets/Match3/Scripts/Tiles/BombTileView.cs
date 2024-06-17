using System.Threading.Tasks;
using Core;

namespace Match3
{
    public class BombTileView : ColoredTileView<BombTile>
    {
        public MatchPattern area;

        public override Entity CreateEntity() => new BombTile();
    }

    public class BombTile : ColoredTile<BombTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            await engine.Wait(0.2f);
            var offset = position - Int2.one * 2;
            foreach (var point in prefab.area.points)
            {
                var p = point + offset;
                if (game.ValidatePoint(p))
                {
                    var tile = game.GetTileAt(p);
                    if (tile != null)
                    {
                        await tile.Hit();
                    }
                }
            }
            await engine.Wait(0.2f);
            engine.RemoveEntity(this);
        }
    }
}
