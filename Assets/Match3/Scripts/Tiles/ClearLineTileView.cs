using Core;

namespace Match3
{
    public class ClearLineTileView : TileView<ClearLineTile>
    {
        public override Entity CreateEntity() => new ClearLineTile();
    }

    public class ClearLineTile : Tile
    {

    }
}
