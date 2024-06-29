using MMC.EngineCore;

namespace MMC.Match3
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

        public override void Save(JsonData data)
        {
            base.Save(data);
            data.W("p", position);
        }
        public override void Load(JsonData data)
        {
            base.Load(data);
            position = data.R<Int2>("p");
        }
    }
}
