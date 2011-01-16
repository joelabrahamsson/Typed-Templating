using System.ComponentModel;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating
{
    [ToolboxItem(false)]
    public class PageListPageItem<TPageData> 
        : PageListItem 
        where TPageData : PageData
    {
        public PageListPageItem(int itemIndex,
            TPageData page,
            int dataItemIndex, 
            int totalNumberOfPagesToRender,
            PageReference listingRootPageLink)
            : base(itemIndex)
        {
            DataItem = page;
            DataItemIndex = dataItemIndex;
            TotalNumberOfPagesToRender = totalNumberOfPagesToRender;
            this.listingRootPageLink = listingRootPageLink;
        }

        PageReference listingRootPageLink;

        public virtual TPageData DataItem { get; private set; }

        public virtual int DataItemIndex { get; private set; }

        public virtual bool IsFirstPageItem
        {
            get
            {
                return DataItemIndex == 0;
            }
        }

        public virtual bool IsLastPageItem
        {
            get
            {
                return DataItemIndex == TotalNumberOfPagesToRender - 1;
            }
        }

        protected virtual int TotalNumberOfPagesToRender { get; private set; }

        public virtual int PageLevelBelowRoot
        {
            get
            {
                if(PageReference.IsNullOrEmpty(listingRootPageLink))
                    return 0;

                if (IsListingRoot(listingRootPageLink))
                    return 0;

                int levels = 1;
                PageReference parentNode = DataItem.ParentLink;
                while (PageReference.IsValue(parentNode) && !IsListingRoot(parentNode))
                {
                    levels++;
                    parentNode = DataFactory.Instance.GetPage(parentNode).ParentLink;
                }

                return levels;
            }
        }

        protected bool IsListingRoot(PageReference pageLink)
        {
            return pageLink.CompareToIgnoreWorkID(listingRootPageLink);
        }

        public override PageData CurrentPage
        {
            get
            {
                return DataItem;
            }
        }
    }
}
