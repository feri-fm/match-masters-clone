using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMC.Core;
using ImUI;
using MMC.Match3;
using UnityEngine;

namespace MMC.DebugRoom
{
    [RequireComponent(typeof(ObjectPool))]
    public class DebugRoomManager : MonoBehaviour
    {
        public BottomDragHandle bottomDragHandle;
        public DebugPage pagePrefab;
        public ListLoader pagesListLoader;
        public List<DebugPage> pages = new();
        public List<DebugSection> sections = new();

        public DebugPage currentPage;

        public int pageCounter;

        private ObjectPool pool;

        private bool dirty;

        private void Awake()
        {
            pool = GetComponent<ObjectPool>();
            pagePrefab.SetActive(false);

            foreach (var section in sections)
            {
                section.Setup(this);
            }
        }

        private void Start()
        {
            var page = AddPage();
            page.title = "main";
        }

        private void LateUpdate()
        {
            if (dirty)
            {
                dirty = false;
                Render();
            }
        }

        public void MarkDirty() => dirty = true;

        private void Render()
        {
            pagesListLoader.Setup(pages);
            foreach (var page in pages)
            {
                page.SetPage(page == currentPage);
            }
        }

        [Member]
        public DebugPage AddPage()
        {
            var page = pool.Spawn(pagePrefab, pagePrefab.transform.parent);
            page.Setup(this);
            pages.Add(page);
            SetPage(page);
            MarkDirty();
            return page;
        }

        public void SetPage(DebugPage page)
        {
            currentPage = page;
            MarkDirty();
        }

        public void RemovePage(DebugPage page)
        {
            if (pages.Count <= 1) return;
            if (currentPage == page)
            {
                var index = pages.IndexOf(page);
                var nextIndex = index + 1;
                var preIndex = index - 1;
                if (preIndex >= 0) SetPage(pages[preIndex]);
                else if (nextIndex < pages.Count) SetPage(pages[nextIndex]);
                else SetPage(null);
            }
            page.Pool();
            pages.Remove(page);
            MarkDirty();
        }

        public T GetSection<T>() where T : DebugSection
        {
            return sections.Find(e => e is T) as T;
        }
    }
}
