namespace Match3
{
    public abstract class TraitView : LifecycleObjectView
    {
        public string key;

        public TileView tile { get; private set; }
        public Trait trait { get; private set; }

        public abstract Trait CreateTrait();

        public void _Setup(TileView tile, Trait trait)
        {
            this.tile = tile;
            this.trait = trait;
            __Setup(trait);
        }
        public void _Remove()
        {
            __Remove(trait);
        }
    }
    public abstract class TraitView<T> : TraitView where T : Trait
    {
    }
}
