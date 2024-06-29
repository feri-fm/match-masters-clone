using MMC.EngineCore;

namespace MMC.Match3
{
    public class GravityTraitView : TraitView<GravityTrait>
    {
        public override Trait CreateTrait() => new GravityTrait();
    }

    public class GravityTrait : Trait<GravityTraitView>
    {

    }
}
