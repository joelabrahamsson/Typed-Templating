using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating.Listing
{
    public class RootAndChildren<TPageData> : IListingStrategy<TPageData> where TPageData : PageData
    {
        public virtual IEnumerable<TPageData> GetPages(PageReference pageLink)
        {
            var result = new List<TPageData>();
            
            var rootPage = DataFactory.Instance.GetPage(pageLink) as TPageData;
            if(rootPage != null)
                result.Add(rootPage);
            
            var childrenOfRoot = DataFactory.Instance.GetChildren(pageLink)
                .OfType<TPageData>()
                .ToList();
            result.AddRange(childrenOfRoot);

            return result;
        }
    }
}
