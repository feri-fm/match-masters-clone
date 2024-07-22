namespace MMC.Game
{
    public abstract class EntityChapter : Chapter
    {
        public ChapterEntityView entityPrefab;

        protected override void Apply(GameplayIns ins)
        {
            base.Apply(ins);
            ins.gameplay.engine.CreateEntity(entityPrefab);
        }
    }
}