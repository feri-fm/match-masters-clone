namespace Core
{
    public abstract class TraitView : LifecycleObjectView
    {
        public string key;

        public EntityView entity { get; private set; }
        public Trait trait { get; private set; }

        public abstract Trait CreateTrait();

        public void _Setup(EntityView entity, Trait trait)
        {
            this.entity = entity;
            this.trait = trait;
            name = key;
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
