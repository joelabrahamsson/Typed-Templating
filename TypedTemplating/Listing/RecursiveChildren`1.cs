using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating.Listing
{
    public class RecursiveChildren<TPageData> : IListingStrategy<TPageData> where TPageData : PageData
    {
        int levels;
        public RecursiveChildren(int recurseLevelsBelowRoot)
        {
            levels = recurseLevelsBelowRoot;
        }

        public virtual IEnumerable<TPageData> GetPages(PageReference pageLink)
        {
            return GetDescendantsFromLevel(pageLink, 1);
        }

        protected virtual IEnumerable<TPageData> GetDescendantsFromLevel(PageReference pageLink, int recursionLevel)
        {
            var pages = new List<TPageData>();

            foreach (var child in DataFactory.Instance.GetChildren(pageLink))
            {
                if (child is TPageData)
                    pages.Add((TPageData)child);

                if(recursionLevel < levels)
                    pages.AddRange(GetDescendantsFromLevel(child.PageLink, recursionLevel + 1));
            }

            return pages;
        }
    }
}
