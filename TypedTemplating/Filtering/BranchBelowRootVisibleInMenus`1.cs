using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating.Filtering
{
    public class BranchBelowRootVisibleInMenus<TPageData>
        : IFilteringStrategy<TPageData> where TPageData : PageData
    {
        PageReference listingRoot;

        public BranchBelowRootVisibleInMenus(PageReference listingRoot)
        {
            this.listingRoot = listingRoot;    
        }

        public IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages)
        {
            return pages.Where(page => AllPagesInBranchVisibleInMenus(page));
        }

        bool AllPagesInBranchVisibleInMenus(PageData page)
        {
            if (page.PageLink.CompareToIgnoreWorkID(listingRoot))
                return true;

            if (!page.VisibleInMenu)
                return false;

            if (PageReference.IsNullOrEmpty(page.ParentLink))
                return true;
            
            var parent = DataFactory.Instance.GetPage(page.ParentLink);
            
            return AllPagesInBranchVisibleInMenus(parent);
        }
    }
}
