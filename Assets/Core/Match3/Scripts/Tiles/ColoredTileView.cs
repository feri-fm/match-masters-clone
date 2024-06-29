namespace MMC.Match3
{
    public abstract class ColoredTileView : TileView<ColoredTile>
    {
        public TileColor color = TileColor.none;

        protected override void OnSetup()
        {
            base.OnSetup();
            tile.onHit += () =>
            {
                var effect = game.config.GetEffect(color);
                var ins = engine.pool.Spawn(effect, engine.container);
                ins.transform.position = engine.GetPosition(tile.position);
            };
        }
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