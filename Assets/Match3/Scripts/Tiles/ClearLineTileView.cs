using System.Threading.Tasks;
using Core;

namespace Match3
{
    public class ClearLineTileView : ColoredTileView<ClearLineTile>
    {
        public ClearLineDirection direction;

        public override Entity CreateEntity() => new ClearLineTile();
    }

    public class ClearLineTile : ColoredTile<ClearLineTileView>
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
            await engine.Wait(0.1f);
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
                        await tile.Hit();
                        if (i % 2 == 0) await engine.Wait(0.1f);
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
                        await tile.Hit();
                        if (i % 2 == 0) await engine.Wait(0.1f);
                    }
                }
            }
            await engine.Wait(0.2f);
            engine.RemoveEntity(this);
        }
    }

    public enum ClearLineDirection
    {
        Vertical, Horizontal
    }
}
