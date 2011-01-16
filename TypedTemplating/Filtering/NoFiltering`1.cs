using System.Collections.Generic;
using EPiServer.Core;

namespace TypedTemplating.Filtering
{
    public class NoFiltering<TPageData> 
        : IFilteringStrategy<TPageData> where TPageData : PageData
    {
        public IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages)
        {
            return pages;
        }
    }
}
