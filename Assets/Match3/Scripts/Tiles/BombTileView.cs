
namespace Match3
{
    public class BombTileView : TileView<BombTile>
    {
        public override Tile CreateTile() => new BombTile();
    }

    public class BombTile : Tile
    {

    }
}
