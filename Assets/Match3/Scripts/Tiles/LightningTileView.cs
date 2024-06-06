
namespace Match3
{
    public class LightningTileView : TileView<LightningTile>
    {
        public override Tile CreateTile() => new LightningTile();
    }

    public class LightningTile : Tile
    {

    }
}
