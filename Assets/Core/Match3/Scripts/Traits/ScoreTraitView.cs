using MMC.EngineCore;

namespace MMC.Match3
{
    public class ScoreTraitView : TraitView<ScoreTrait>
    {
        public override Trait CreateTrait() => new ScoreTrait();
    }

    public class ScoreTrait : Trait<ScoreTraitView>
    {
        public int value = 1;
    }
}
