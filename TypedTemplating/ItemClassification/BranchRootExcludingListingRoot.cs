using EPiServer.Core;

namespace TypedTemplating.ItemClassification
{
    public class BranchRootExcludingListingRoot<TPageData> : ISelectedItemStrategy<TPageData>
        where TPageData : PageData
    {
        public bool IsSelected(PageListPageItem<TPageData> pageItem)
        {
            if (pageItem.PageLevelBelowRoot == 0 && !pageItem.IsCurrentlyViewedPage)
                return false;

            return pageItem.IsCurrentlyViewedPage || pageItem.IsAncestorOfCurrentlyViewedPage;
        }
    }
}
