using System.Collections.Generic;
using EPiServer.Core;

namespace TypedTemplating.AccessFilter
{
    public interface IAccessFilteringStrategy<TPageData> where TPageData : PageData
    {
        IEnumerable<TPageData> Filter(IEnumerable<TPageData> pages);
    }
}
