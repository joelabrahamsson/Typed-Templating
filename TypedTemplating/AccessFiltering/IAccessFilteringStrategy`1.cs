using System.Collections.Generic;
using EPiServer.Core;

namespace TypedTemplating.AccessFiltering
{
    public interface IAccessFilteringStrategy<TPageData> where TPageData : PageData
    {
        IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages);
    }
}
