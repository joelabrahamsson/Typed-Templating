using EPiServer.Core;

namespace TypedTemplating.ItemClassification
{
    public interface ISelectedItemStrategy<TPageData> where TPageData : PageData
    {
        bool IsSelected(PageListPageItem<TPageData> pageItem);
    }
}
