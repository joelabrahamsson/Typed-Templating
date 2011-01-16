﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using EPiServer.Core;
using TypedTemplating.AccessFiltering;
using TypedTemplating.Filtering;
using TypedTemplating.Listing;
using TypedTemplating.Paging;

namespace TypedTemplating
{
    [ParseChildren(true)]
    [PersistChildren(true)]
    public class PageList<TPageData> : Control, INamingContainer, IPageSource where TPageData : PageData
    {
        bool isDataBound;

        public PageList()
        {
            ListingStrategy = new Children<TPageData>();
            AccessFilteringStrategy = new FilterForVisitor<TPageData>();
            FilteringStrategy = new NoFiltering<TPageData>();
            PagingStrategy = new NoPaging();
        }

        #region Event handling
        public event EventHandler<PageListPageItemEventArgs<TPageData>> PageItemDataBound;

        public event EventHandler<PageListItemEventArgs> ItemDataBound;

        protected virtual void OnPageItemDataBound(PageListPageItem<TPageData> item)
        {
            OnItemDataBound(item);

            if (PageItemDataBound == null)
                return;

            var eventArgs = new PageListPageItemEventArgs<TPageData>(item);
            PageItemDataBound(this, eventArgs);
        }

        protected virtual void OnItemDataBound(PageListItem item)
        {
            if (ItemDataBound == null)
                return;

            var eventArgs = new PageListItemEventArgs(item);
            ItemDataBound(this, eventArgs);
        }
        #endregion

        #region Control hiearchy build up
        protected override void CreateChildControls()
        {
            Controls.Clear();
        }

        void CreateControlHiearchy()
        {
            int itemIndex = 0;
            int dataItemIndex = 0;
            var pages = GetPagesToRender();
            int numberOfPagesToRender = pages.Count();
            
            foreach (var page in pages)
            {
                AddPageItem(itemIndex, page, dataItemIndex, numberOfPagesToRender);
                itemIndex++;

                if (IsNotLastItem(dataItemIndex, numberOfPagesToRender) && SeparatorTemplate != null)
                {
                    AddSeparator(itemIndex);
                    itemIndex++;
                }

                dataItemIndex++;
            }
        }

        void AddPageItem(int itemIndex, TPageData page, int dataItemIndex, int numberOfPagesToRender)
        {
            PageListPageItem<TPageData> itemContainer =
                new PageListPageItem<TPageData>(
                    itemIndex,
                    page,
                    dataItemIndex,
                    numberOfPagesToRender,
                    listingRoot);

            var itemTemplateHeader = GetItemTemplateHeader(itemContainer);
            if (itemTemplateHeader != null)
            {
                var headerContainer = new Control();
                itemTemplateHeader.InstantiateIn(headerContainer);
                itemContainer.ItemHeader = headerContainer;
            }

            var template = GetItemTemplate(itemContainer);
            if (template != null)
            {
                template.InstantiateIn(itemContainer);
            }

            var itemTemplateFooter = GetItemTemplateFooter(itemContainer);
            if (itemTemplateFooter != null)
            {
                var footerContainer = new Control();
                itemTemplateFooter.InstantiateIn(footerContainer);
                itemContainer.ItemFooter = footerContainer;
            }

            Controls.Add(itemContainer);

            itemContainer.DataBind();
            OnPageItemDataBound(itemContainer);
        }

        ITemplate GetItemTemplateHeader(PageListPageItem<TPageData> pageItem)
        {
            if (IsAlternatingItem(pageItem.DataItemIndex) && AlternatingItemTemplateHeader != null)
            {
                return AlternatingItemTemplateHeader;
            }
            
            if (ItemTemplateHeader != null)
            {
                return ItemTemplateHeader;
            }

            return null;
        }

        ITemplate GetItemTemplateFooter(PageListPageItem<TPageData> pageItem)
        {
            if (IsAlternatingItem(pageItem.DataItemIndex) && AlternatingItemTemplateFooter != null)
            {
                return AlternatingItemTemplateFooter;
            }
            
            if (ItemTemplateFooter != null)
            {
                return ItemTemplateFooter;
            }

            return null;
        }

        protected virtual ITemplate GetItemTemplate(PageListPageItem<TPageData> pageItem)
        {
            if (pageItem.DataItemIndex == 0 && FirstItemTemplate != null)
                return FirstItemTemplate;

            if (pageItem.IsLastPageItem && LastItemTemplate != null)
                return LastItemTemplate;

            if (AlternatingItemTemplate == null)
                return ItemTemplate;

            return IsAlternatingItem(pageItem.DataItemIndex) ? ItemTemplate : AlternatingItemTemplate;
        }

        bool IsNotLastItem(int dataItemIndex, int pageCount)
        {
            return dataItemIndex + 1 < pageCount;
        }

        void AddSeparator(int itemIndex)
        {
            var separatorContainer = new NonPageItem(itemIndex);
            SeparatorTemplate.InstantiateIn(separatorContainer);
            Controls.Add(separatorContainer);
            separatorContainer.DataBind();
            OnItemDataBound(separatorContainer);
        }

        bool IsAlternatingItem(int dataItemIndex)
        {
            return dataItemIndex % 2 != 0;
        }

        #endregion

        #region Data binding
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!isDataBound && AutoBind)
            {
                DataBind();
            }
        }

        public override void DataBind()
        {
            base.DataBind();

            EnsureChildControls();

            CreateControlHiearchy();

            isDataBound = true;
        }
        #endregion

        #region Page retrieval and filtering
        protected virtual IEnumerable<TPageData> GetAllPagesFromSource()
        {
            if (!PageReference.IsNullOrEmpty(listingRoot))
                return ListingStrategy.GetPages(listingRoot);

            if(dataSource != null)
                return dataSource;

            return new List<TPageData>();
        }

        protected virtual IEnumerable<TPageData> GetPagesToList()
        {
            var accessiblePages = AccessFilteringStrategy.Filter(GetAllPagesFromSource());
            return FilteringStrategy.Filter(accessiblePages);
        }

        protected virtual IEnumerable<TPageData> GetPagesToRender()
        {
            int skipCount = (PagingPage - 1)*PagingPageSize;
            
            return GetPagesToList()
                .Skip(skipCount)
                .Take(PagingPageSize);
        }
        #endregion

        #region Paging
        public int TotalNumberOfPages
        {
            get
            {
                return GetPagesToList().Count();
            }
        }

        int? pagingPage;
        public int PagingPage
        {
            get
            {
                if (pagingPage.HasValue)
                    return pagingPage.Value;

                return PagingStrategy
                    .GetPagingSpecification(TotalNumberOfPages)
                    .PageNumber;
            }

            set
            {
                pagingPage = value;
            }
        }

        int? pagingPageSize;
        public int PagingPageSize 
        {
            get
            {
                if (pagingPageSize.HasValue)
                    return pagingPageSize.Value;

                return PagingStrategy
                    .GetPagingSpecification(TotalNumberOfPages)
                    .PageSize;
            }

            set
            {
                pagingPageSize = value;
            }
        }
        #endregion

        #region Configurables
        public IAccessFilteringStrategy<TPageData> AccessFilteringStrategy { get; set; }

        public IFilteringStrategy<TPageData> FilteringStrategy { get; set; }

        public IListingStrategy<TPageData> ListingStrategy { get; set; }

        public IPagingStrategy PagingStrategy { get; set; }

        public bool AutoBind { get; set; }

        IEnumerable<TPageData> dataSource;
        public IEnumerable<TPageData> DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        PageReference listingRoot;
        public virtual PageReference ListingRoot
        {
            get
            {
                return PageReference.IsNullOrEmpty(listingRoot) 
                    ? PageReference.EmptyReference
                    : listingRoot;
            }
            set
            {
                listingRoot = value;
            }
        }
        #endregion

        #region Templates
        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ItemTemplate { get; set; }

        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate AlternatingItemTemplate { get; set; }

        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate FirstItemTemplate { get; set; }

        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate LastItemTemplate { get; set; }

        [TemplateContainer(typeof(NonPageItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SeparatorTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ItemTemplateHeader { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ItemTemplateFooter { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate AlternatingItemTemplateHeader { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate AlternatingItemTemplateFooter { get; set; }
        #endregion

        #region IPageSource
        public virtual PageData CurrentPage
        {
            get
            {
                return PageSource.CurrentPage;
            }
        }

        public PageDataCollection GetChildren(PageReference pageLink)
        {
            return PageSource.GetChildren(pageLink);
        }

        public PageData GetPage(PageReference pageLink)
        {
            return PageSource.GetPage(pageLink);
        }

        private IPageSource pageSource;
        protected virtual IPageSource PageSource
        {
            get
            {
                if (pageSource != null)
                    return pageSource;

                pageSource = PageSourceHelper.GetPageSourceForControl(this);

                return pageSource;
            }
        }
        #endregion
    }
}
