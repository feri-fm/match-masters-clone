
namespace Match3
{
    public class BeadTileView : TileView<BeadTile>
    {
        public override Tile CreateTile() => new BeadTile();
    }

    public class BeadTile : Tile
    {

    }
}
