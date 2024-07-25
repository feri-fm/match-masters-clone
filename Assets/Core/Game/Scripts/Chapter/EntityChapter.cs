namespace MMC.Game
{
    public abstract class EntityChapter : Chapter
    {
        public ChapterEntityView entityPrefab;

        protected ChapterEntity chapterEntity;

        protected override void Apply(GameplayIns ins)
        {
            base.Apply(ins);
            chapterEntity = ins.gameplay.engine.CreateEntity(entityPrefab) as ChapterEntity;
        }
    }
}