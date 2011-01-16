using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Filters;

namespace TypedTemplating.AccessFilter
{
    public class FilterForVisitor<TPageData> : IAccessFilteringStrategy<TPageData> where TPageData : PageData
    {
        public virtual IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages)
        {
            var pageCollection = new PageDataCollection(pages);
            FilterForVisitor.Filter(pageCollection);
            return pageCollection.Cast<TPageData>();
        }
    }
}
