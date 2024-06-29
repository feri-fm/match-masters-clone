using System.Collections.Generic;
using UnityEngine;

namespace MMC.EngineCore
{
    [CreateAssetMenu(fileName = "EngineConfig", menuName = "EngineConfig")]
    public class EngineConfig : ScriptableObject
    {
        public List<EntityView> entities;
        public List<TraitView> traits;

        public EntityView GetEntity(string key)
        {
            return entities.Find(e => e.key == key);
        }
        public T GetEntity<T>() where T : EntityView
        {
            return entities.Find(e => e is T) as T;
        }
        public TraitView GetTrait(string key)
        {
            return traits.Find(e => e.key == key);
        }
        public T GetTrait<T>() where T : TraitView
        {
            return traits.Find(e => e is T) as T;
        }
    }
}
