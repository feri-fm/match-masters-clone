using MMC.EngineCore;

namespace MMC.Match3
{
    public class MechanicTraitView : TraitView<MechanicTrait>
    {
        public override Trait CreateTrait() => new MechanicTrait();
    }

    public class MechanicTrait : Trait<MechanicTraitView>
    {

    }
}
