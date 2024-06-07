using System.Collections.Generic;

namespace Match3
{
    public abstract class TileView : LifecycleObjectView
    {
        public string key;

        public List<TraitView> traits { get; } = new();

        public EngineView engine { get; private set; }
        public Tile tile { get; private set; }

        public abstract Tile CreateTile();

        public void _Setup(EngineView engine, Tile tile)
        {
            this.engine = engine;
            this.tile = tile;
            __Setup(tile);
        }
        public void _Remove()
        {
            foreach (var trait in traits)
            {
                trait._Remove();
                trait.Pool();
            }
            traits.Clear();
            __Remove(tile);
        }
    }
    public abstract class TileView<T> : TileView where T : Tile
    {
    }
}
