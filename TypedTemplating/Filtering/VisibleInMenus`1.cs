using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;

namespace TypedTemplating.Filtering
{
    public class VisibleInMenus<TPageData>
        : IFilteringStrategy<TPageData> where TPageData : PageData
    {
        public IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages)
        {
            return pages.Where(page => page.VisibleInMenu);
        }
    }
}
