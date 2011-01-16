using System.Collections.Generic;
using EPiServer.Core;

namespace TypedTemplating.Filtering
{
    public interface IFilteringStrategy<TPageData> where TPageData : PageData
    {
        IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages);
    }
}
