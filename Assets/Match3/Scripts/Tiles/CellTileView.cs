
namespace Match3
{
    public class CellTileView : TileView<CellTile>
    {
        public override Tile CreateTile() => new CellTile();
    }

    public class CellTile : Tile
    {

    }
}
