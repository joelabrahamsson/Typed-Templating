using System.ComponentModel;
using System.Web.UI;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating
{
    [ToolboxItem(false)]
    public class PageListPageItem<TPageData> 
        : PageListItem 
        where TPageData : PageData
    {
        protected PageReference ListingRoot;

        public PageListPageItem(
            int itemIndex,
            TPageData page,
            int dataItemIndex, 
            int totalNumberOfPagesToRender,
            PageReference listingRoot)
            : base(itemIndex)
        {
            DataItem = page;
            DataItemIndex = dataItemIndex;
            TotalNumberOfPagesToRender = totalNumberOfPagesToRender;
            ListingRoot = listingRoot;
        }

        public virtual TPageData DataItem { get; private set; }

        #region DataItem positions
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
        #endregion

        #region Branch positions
        int? pageLevelBelowRoot;
        public virtual int PageLevelBelowRoot
        {
            get
            {
                if (pageLevelBelowRoot.HasValue)
                    return pageLevelBelowRoot.Value;

                pageLevelBelowRoot = GetPageLevelBelowRoot();

                return pageLevelBelowRoot.Value;
            }
        }

        protected virtual int GetPageLevelBelowRoot()
        {
            if(PageReference.IsNullOrEmpty(ListingRoot))
                return 0;

            if (IsListingRoot(DataItem.PageLink))
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

        protected virtual bool IsListingRoot(PageReference pageLink)
        {
            return pageLink.CompareToIgnoreWorkID(ListingRoot);
        }

        public virtual bool IsCurrentlyViewedPage
        {
            get
            {
                return DataItemTargetPageLink.CompareToIgnoreWorkID(CurrentlyViewedPage.PageLink);
            }
        }

        public virtual PageReference DataItemTargetPageLink
        {
            get { return DataItem.PageLink; }
        }

        bool? isAncestorOfCurrentlyViewedPage;
        public virtual bool IsAncestorOfCurrentlyViewedPage
        {
            get 
            {
                if (isAncestorOfCurrentlyViewedPage.HasValue)
                    return isAncestorOfCurrentlyViewedPage.Value;

                isAncestorOfCurrentlyViewedPage = DetermineIfAncestorOfCurrentlyViewedPage();

                return isAncestorOfCurrentlyViewedPage.Value;
            }
        }

        protected virtual bool DetermineIfAncestorOfCurrentlyViewedPage()
        {
            if (CurrentlyViewedPage.PageLink.CompareToIgnoreWorkID(DataItemTargetPageLink))
                return false;

            PageReference parentNode = CurrentlyViewedPage.ParentLink;
            while (PageReference.IsValue(parentNode))
            {
                if (parentNode.CompareToIgnoreWorkID(DataItemTargetPageLink))
                    return true;

                parentNode = DataFactory.Instance.GetPage(parentNode).ParentLink;
            }

            return false;
        }

        bool? isDescendantOfCurrentlyViewedPage;
        public virtual bool IsDescendantOfCurrentlyViewedPage
        {
            get 
            {
                if (isDescendantOfCurrentlyViewedPage.HasValue)
                    return isDescendantOfCurrentlyViewedPage.Value;

                isDescendantOfCurrentlyViewedPage = DetermineIfDescendantOfCurrentlyViewedPage();

                return isDescendantOfCurrentlyViewedPage.Value;
            }
        }

        protected virtual bool DetermineIfDescendantOfCurrentlyViewedPage()
        {
            if (DataItemTargetPageLink.CompareToIgnoreWorkID(CurrentlyViewedPage.PageLink))
                return false;

            PageReference parentNode = DataItem.ParentLink;
            while (PageReference.IsValue(parentNode))
            {
                if (parentNode.CompareToIgnoreWorkID(CurrentlyViewedPage.PageLink))
                    return true;

                parentNode = DataFactory.Instance.GetPage(parentNode).ParentLink;
            }

            return false;
        }

        protected virtual PageData CurrentlyViewedPage
        {
            get
            {
                return PageBase.CurrentPage;
            }
        }
        #endregion

        #region Surroundings
        Control itemHeader;
        public virtual Control ItemHeader
        {
            get
            {
                return itemHeader;
            }

            set
            {
                if (itemHeader != null)
                    Controls.Remove(itemHeader);

                itemHeader = value;
                Controls.Add(itemHeader);
            }
        }

        Control itemFooter;
        public virtual Control ItemFooter
        {
            get
            {
                return itemFooter;
            }

            set
            {
                if (itemFooter != null)
                    Controls.Remove(itemFooter);

                itemFooter = value;
                Controls.Add(itemFooter);
            }
        }
        #endregion

        #region IPageSource
        public override PageData CurrentPage
        {
            get
            {
                return DataItem;
            }
        }
        #endregion
    }
}
