
namespace Match3
{
    public class PositionTraitView : TraitView<PositionTrait>
    {
        public override Trait CreateTrait() => new PositionTrait();
    }

    public class PositionTrait : Trait
    {
        public Int2 position;
    }
}
