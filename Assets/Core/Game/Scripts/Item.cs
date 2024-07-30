using UnityEngine;

namespace MMC.Game
{
    public class Item : ScriptableObject
    {
        public string key => name;
        public Sprite icon;
    }
}