using System.Collections.Generic;
using EPiServer.Core;

namespace TypedTemplating.Listing
{
    public interface IListingStrategy<TPageData> where TPageData : PageData
    {
        IEnumerable<TPageData> GetPages(PageReference pageLink);
    }
}
