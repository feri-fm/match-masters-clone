using UnityEngine;
using UnityEngine.Events;

namespace MMC.Game
{
    public class Blocker : MonoBehaviour
    {
        public UnityEvent onClick = new();

        private void OnMouseUpAsButton()
        {
            if (!UIFilter.IsPointerClear())
            {
                onClick.Invoke();
            }
        }
    }
}