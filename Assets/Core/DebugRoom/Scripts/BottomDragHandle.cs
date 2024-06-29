using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMC.DebugRoom
{
    public class BottomDragHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Canvas canvas;
        public RectTransform handle;
        public RectTransform panel;

        private bool isDragging;
        private Vector3 downMousePosition;
        private float startHeight;

        public float normalizedPosition;

        private void Update()
        {
            var canvasScale = canvas.transform.localScale.x;
            var height = panel.sizeDelta.y;

            if (isDragging)
            {
                height = startHeight + (Input.mousePosition - downMousePosition).y / canvasScale;
            }

            if (height < 0) height = 0;

            var handleHeight = handle.sizeDelta.y;
            var maxHeight = Screen.height / canvasScale - handleHeight;

            if (height > maxHeight) height = maxHeight;

            normalizedPosition = height / maxHeight;

            panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            downMousePosition = Input.mousePosition;
            startHeight = panel.sizeDelta.y;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }

        private void OnEnable()
        {
            isDragging = false;
        }
        private void OnDisable()
        {
            isDragging = false;
        }
    }
}
