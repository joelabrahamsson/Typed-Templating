using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating.Listing
{
    public class Children<TPageData> : IListingStrategy<TPageData> where TPageData : PageData
    {
        public virtual IEnumerable<TPageData> GetPages(PageReference pageLink)
        {
            return DataFactory.Instance.GetChildren(pageLink)
                .OfType<TPageData>()
                .ToList();
        }
    }
}
