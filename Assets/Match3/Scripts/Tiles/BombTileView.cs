using Core;

namespace Match3
{
    public class BombTileView : TileView<BombTile>
    {
        public override Entity CreateEntity() => new BombTile();
    }

    public class BombTile : Tile
    {

    }
}
