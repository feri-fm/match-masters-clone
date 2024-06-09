
namespace Match3
{
    public class CellEntityView : EntityView<CellEntity>
    {
        protected override void OnRender()
        {
            base.OnRender();
            transform.position = engine.GetPosition(entity.position);
        }

        public override Entity CreateEntity() => new CellEntity();
    }

    public class CellEntity : Entity
    {
        public Int2 position;
    }
}
