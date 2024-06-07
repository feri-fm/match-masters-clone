namespace Match3
{
    public class GameTileView : TileView<GameTile>
    {
        public override Tile CreateTile() => new GameTile();
    }

    public class GameTile : Tile<GameTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            evaluable.RegisterCallback(0, Evaluate);
        }

        public void Evaluate()
        {
            var bead = engine.CreateTile("bead_red");
            bead.WithTrait<PositionTrait>(p => p.position = new Int2(0, 0));
            var position = bead.WithTrait<PositionTrait, Int2>(e => e.position, Int2.zero);

            // i was here: put position as a field for Tile, not a separate Trait

            engine.ForTileWithTrait<GravityTrait>(t =>
            {

            });
        }
    }
}
