using EPiServer.Core;

namespace TypedTemplating
{
    public class PageListPageItemEventArgs<TPageData> : PageListItemEventArgs where TPageData : PageData
    {
        public PageListPageItemEventArgs(PageListPageItem<TPageData> item)
            : base(item)
        {
            PageItem = item;
        }

        public PageListPageItem<TPageData> PageItem { get; set; }
    }
}
