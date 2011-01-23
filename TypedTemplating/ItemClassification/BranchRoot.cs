using EPiServer.Core;

namespace TypedTemplating.ItemClassification
{
    public class BranchRoot<TPageData> : ISelectedItemStrategy<TPageData>
        where TPageData : PageData
    {
        public bool IsSelected(PageListPageItem<TPageData> pageItem)
        {
            return pageItem.IsCurrentlyViewedPage || pageItem.IsAncestorOfCurrentlyViewedPage;
        }
    }
}
