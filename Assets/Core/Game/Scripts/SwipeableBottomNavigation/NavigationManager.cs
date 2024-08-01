using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SwipeableBottomNavigation
{
    public class NavigationManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public Transform linesContainer;
        public Transform buttonsContainer;
        public Transform panelsContainer;
        public RectTransform panelsContainerSize;
        public NavigationCursor cursor;
        public float catchScrollSpeed = 1;
        public float jumpScrollMultiplier = 1;
        public float smooth = 10;
        public float selectedWeight = 1.4f;

        public List<NavigationLine> lines { get; } = new();
        public List<NavigationButton> buttons { get; } = new();
        public List<NavigationPanel> panels { get; } = new();

        public int currentPanelIndex { get; set; }
        public float scroll;

        private bool isDragging;
        private float dragScroll;
        private Vector3 dragPoint;
        private float panelsWidth;
        private float catchScroll;

        private void Awake()
        {
            lines.Clear();
            buttons.Clear();
            panels.Clear();
            for (int i = 0; i < linesContainer.childCount; i++)
            {
                var line = linesContainer.GetChild(i).GetComponent<NavigationLine>();
                if (line != null)
                    lines.Add(line);
            }
            for (int i = 0; i < buttonsContainer.childCount; i++)
            {
                var button = buttonsContainer.GetChild(i).GetComponent<NavigationButton>();
                if (button != null)
                    buttons.Add(button);
            }
            for (int i = 0; i < panelsContainer.childCount; i++)
            {
                var panel = panelsContainer.GetChild(i).GetComponent<NavigationPanel>();
                if (panel != null)
                    panels.Add(panel);
            }

            for (int i = 0; i < lines.Count; i++)
                lines[i].Setup(this, i);
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Setup(this, i);
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Setup(this, i);
                panels[i].SetActive(false);
            }

            cursor.Setup(this);
        }

        public void Render()
        {
            foreach (var panel in panels)
            {
                if (panel.isActive)
                    panel.OnRender();
            }
        }

        public void SetPanel(int index)
        {
            currentPanelIndex = index;
            panels[index].OnSelected();
        }

        public void Jump()
        {
            scroll = currentPanelIndex;
            for (int i = 0; i < lines.Count; i++)
                lines[i].Jump();
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Jump();
            cursor.Jump();
        }

        private void Update()
        {
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(panelsContainerSize);
            panelsWidth = bounds.size.x;

            if (isDragging)
            {
                var delta = -(Input.mousePosition - dragPoint).x;
                delta /= Screen.width;
                scroll = dragScroll + delta;
                catchScroll = Mathf.MoveTowards(catchScroll, scroll, catchScrollSpeed * Time.deltaTime);
            }
            else
            {
                if (Mathf.Abs(scroll - currentPanelIndex) > 0.001f)
                    scroll = Mathf.Lerp(scroll, currentPanelIndex, cursor.smooth * Time.deltaTime);
                else
                    scroll = currentPanelIndex;
            }

            scroll = Mathf.Clamp(scroll, 0, panels.Count - 1);

            scroll += 1;

            scroll -= 0.5f;
            var current = (int)scroll;
            var progress = scroll - current;
            var previous = current - 1;
            var next = current + 1;
            scroll += 0.5f;

            SetupPanel(panels[current], -progress + 0.5f);
            if (previous >= 0)
                SetupPanel(panels[previous], -progress - 1 + 0.5f);
            if (next < panels.Count)
                SetupPanel(panels[next], -progress + 1 + 0.5f);

            for (int i = 0; i < panels.Count; i++)
            {
                var active = i == current || i == previous || i == next;
                if (panels[i].isActive != active)
                {
                    panels[i].SetActive(active);
                    if (active)
                    {
                        panels[i].OnOpen();
                        panels[i].OnRender();
                    }
                    else
                    {
                        panels[i].OnClose();
                    }
                }
            }
            scroll -= 1;
        }

        private void OnDisable()
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].isActive)
                {
                    panels[i].SetActive(false);
                    panels[i].OnClose();
                }
            }
        }

        private void SetupPanel(NavigationPanel panel, float offset)
        {
            panel.rect.offsetMin = offset * panelsWidth * Vector2.right;
            panel.rect.offsetMax = offset * panelsWidth * Vector2.right;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            dragScroll = scroll;
            catchScroll = scroll;
            dragPoint = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            var velocity = scroll - catchScroll;
            var jump = Mathf.Clamp(velocity * jumpScrollMultiplier, -0.5f, 0.5f);
            currentPanelIndex = Mathf.Clamp(Mathf.RoundToInt(scroll + jump), 0, panels.Count - 1);
        }
    }

    public class NavigationSegment : MonoBehaviour
    {
        public int index { get; private set; }
        public NavigationManager navigation { get; private set; }

        public bool isSelected => navigation.currentPanelIndex == index;

        public float weight => isSelected ? navigation.selectedWeight : 1;

        public void Setup(NavigationManager manager, int index)
        {
            this.navigation = manager;
            this.index = index;
            Setup();
        }

        public virtual void Setup() { }
        public virtual void OnRender() { }
    }
}