using MMC.EngineCore;

namespace MMC.Game
{
    public abstract class ChapterEntityView : EntityView<ChapterEntity>
    {

    }

    public abstract class ChapterEntity : Entity<ChapterEntityView>
    {

    }

    public abstract class ChapterEntityView<T> : ChapterEntityView where T : ChapterEntity
    {
        public new T entity => base.entity as T;
    }
    public abstract class ChapterEntity<T> : ChapterEntity where T : ChapterEntityView
    {
        public new T prefab => base.prefab as T;
    }
}