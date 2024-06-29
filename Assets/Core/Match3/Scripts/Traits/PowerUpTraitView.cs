using MMC.EngineCore;

namespace MMC.Match3
{
    public class PowerUpTraitView : TraitView<PowerUpTrait>
    {
        public override Trait CreateTrait() => new PowerUpTrait();
    }

    public class PowerUpTrait : Trait<PowerUpTraitView>
    {

    }
}
