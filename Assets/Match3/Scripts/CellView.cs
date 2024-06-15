using Core;

namespace Match3
{
    public class CellView : EntityView<Cell>
    {
        protected override void OnRender()
        {
            base.OnRender();
            transform.position = engine.GetPosition(entity.position);
        }

        public override Entity CreateEntity() => new Cell();
    }

    public class Cell : Entity
    {
        public Int2 position;
    }
}
