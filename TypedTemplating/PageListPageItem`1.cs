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
        PageItemRenderingContext<TPageData> renderingContext;
        
        public PageListPageItem(int itemIndex, PageItemRenderingContext<TPageData> renderingContext, 
            PageReference listingRootPageLink)
            : base(itemIndex)
        {
            this.renderingContext = renderingContext;
            this.listingRootPageLink = listingRootPageLink;
        }

        PageReference listingRootPageLink;

        public TPageData DataItem
        {
            get { return renderingContext.Page; }
        }

        public virtual int DataItemIndex
        {
            get { return renderingContext.DataItemIndex; }
        }

        public virtual bool IsFirstPageItem
        {
            get { return renderingContext.IsFirst; }
        }

        public virtual bool IsLastPageItem
        {
            get { return renderingContext.IsLast; }
        }

        public virtual int PageLevelBelowRoot
        {
            get
            {
                if(PageReference.IsNullOrEmpty(listingRootPageLink))
                    return 0;

                if (IsListingRoot(listingRootPageLink))
                    return 0;

                int levels = 1;
                PageReference parentNode = renderingContext.Page.ParentLink;
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

        
    }
}
