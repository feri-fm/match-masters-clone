namespace Match3
{
    public abstract class ColoredTileView : TileView<ColoredTile>
    {
        public TileColor color = TileColor.none;

    }
    public abstract class ColoredTile : Tile<ColoredTileView>
    {
        public TileColor color => prefab.color;
    }

    public abstract class ColoredTileView<T> : ColoredTileView where T : Tile
    {
        public new T entity => base.entity as T;
    }
    public abstract class ColoredTile<T> : ColoredTile where T : ColoredTileView
    {
        public new T prefab => base.prefab as T;
    }
}