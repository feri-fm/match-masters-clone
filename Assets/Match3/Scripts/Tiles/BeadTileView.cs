
namespace Match3
{
    public class BeadTileView : TileView<BeadTile>
    {
        public int color;

        public override Tile CreateTile() => new BeadTile();
    }

    public class BeadTile : Tile<BeadTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<PositionTraitView>();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<ColorTraitView, ColorTrait>(c => c.color = prefab.color);
        }
    }
}
