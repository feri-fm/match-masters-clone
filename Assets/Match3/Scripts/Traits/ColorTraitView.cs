
namespace Match3
{
    public class ColorTraitView : TraitView<ColorTrait>
    {
        public override Trait CreateTrait() => new ColorTrait();
    }

    public class ColorTrait : Trait<ColorTraitView>
    {
        public int color;
    }
}
