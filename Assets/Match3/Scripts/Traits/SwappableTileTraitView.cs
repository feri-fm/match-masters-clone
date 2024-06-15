using Core;

namespace Match3
{
    public class SwappableTileTraitView : TraitView<SwappableTileTrait>
    {
        public override Trait CreateTrait() => new SwappableTileTrait();
    }

    public class SwappableTileTrait : Trait
    {

    }
}
