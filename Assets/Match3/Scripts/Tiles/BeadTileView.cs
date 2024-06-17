using UnityEngine;
using Core;
using System.Threading.Tasks;

namespace Match3
{
    public class BeadTileView : ColoredTileView<BeadTile>
    {
        public override Entity CreateEntity() => new BeadTile();
    }

    public class BeadTile : ColoredTile<BeadTileView>
    {
        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();

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

        protected override async Task OnHit()
        {
            await base.OnHit();
            engine.RemoveEntity(this);
        }
    }
}
