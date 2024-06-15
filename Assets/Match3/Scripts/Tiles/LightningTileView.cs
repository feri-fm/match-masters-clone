using Core;

namespace Match3
{
    public class LightningTileView : TileView<LightningTile>
    {
        public override Entity CreateEntity() => new LightningTile();
    }

    public class LightningTile : Tile
    {

    }
}
