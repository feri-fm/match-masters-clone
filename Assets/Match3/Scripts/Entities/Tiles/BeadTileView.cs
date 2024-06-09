
using UnityEngine;

namespace Match3
{
    public class BeadTileView : TileView<BeadTile>
    {
        public int color;

        public override Entity CreateEntity() => new BeadTile();
    }

    public class BeadTile : Tile<BeadTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<ColorTraitView, ColorTrait>(c => c.color = prefab.color);

            evaluable.RegisterCallback(0, Evaluate);
        }

        public void Evaluate()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                position.x += Random.Range(-1, 2);
                position.y += Random.Range(-1, 2);
                Changed();
            }
        }
    }
}
