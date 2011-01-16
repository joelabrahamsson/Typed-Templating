using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using EPiServer.Core;
using TypedTemplating.AccessFilter;
using TypedTemplating.Listing;
using TypedTemplating.Paging;

namespace TypedTemplating
{
    [ParseChildren(true)]
    [PersistChildren(true)]
    public class PageList<TPageData> : Control, INamingContainer where TPageData : PageData
    {
        bool isDataBound;

        public PageList()
        {
            ListingStrategy = new Children<TPageData>();
            AccessFilteringStratey = new FilterForVisitor<TPageData>();
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
                var pageItemContext = 
                    new PageItemRenderingContext<TPageData>(
                        page, dataItemIndex, numberOfPagesToRender);
                
                var itemTemplateHeader = GetItemTemplateHeader(dataItemIndex);
                if (itemTemplateHeader != null)
                {
                    AddItemSurrounding(itemTemplateHeader, itemIndex);
                    itemIndex++;
                }

                AddPageItem(itemIndex, pageItemContext);
                itemIndex++;

                var itemTemplateFooter = GetItemTemplateFooter(dataItemIndex);
                if (itemTemplateFooter != null)
                {
                    AddItemSurrounding(itemTemplateFooter, itemIndex);
                    itemIndex++;
                }

                if (IsNotLastItem(dataItemIndex, numberOfPagesToRender) && SeparatorTemplate != null)
                {
                    AddSeparator(itemIndex);
                    itemIndex++;
                }

                dataItemIndex++;
            }
        }

        void AddItemSurrounding(ITemplate itemTemplate, int itemIndex)
        {
            var container = new NonPageItem(itemIndex);
            itemTemplate.InstantiateIn(container);
            Controls.Add(container);
            container.DataBind();
            OnItemDataBound(container);
        }

        void AddPageItem(int itemIndex, PageItemRenderingContext<TPageData> pageItemContext)
        {
            PageListPageItem<TPageData> container = CreatePageItemContainer(itemIndex, pageItemContext);
            var template = GetItemTemplate(pageItemContext);
            template.InstantiateIn(container);
            Controls.Add(container);
            container.DataBind();
            OnPageItemDataBound(container);
        }
        
        ITemplate GetItemTemplateHeader(int dataItemIndex)
        {
            if (IsAlternatingItem(dataItemIndex) && AlternatingItemTemplateHeader != null)
            {
                return AlternatingItemTemplateHeader;
            }
            
            if (ItemTemplateHeader != null)
            {
                return ItemTemplateHeader;
            }

            return null;
        }

        ITemplate GetItemTemplateFooter(int dataItemIndex)
        {
            if (IsAlternatingItem(dataItemIndex) && AlternatingItemTemplateFooter != null)
            {
                return AlternatingItemTemplateFooter;
            }
            
            if (ItemTemplateFooter != null)
            {
                return ItemTemplateFooter;
            }

            return null;
        }

        PageListPageItem<TPageData> CreatePageItemContainer(int itemIndex, PageItemRenderingContext<TPageData> renderingContext)
        {
            return new PageListPageItem<TPageData>(
                itemIndex,
                renderingContext,
                pageLink);
        }

        protected virtual ITemplate GetItemTemplate(PageItemRenderingContext<TPageData> renderingContext)
        {
            if (renderingContext.DataItemIndex == 0 && FirstItemTemplate != null)
                return FirstItemTemplate;

            if (renderingContext.IsLast && LastItemTemplate != null)
                return LastItemTemplate;

            if (AlternatingItemTemplate == null)
                return ItemTemplate;

            return IsAlternatingItem(renderingContext.DataItemIndex) ? ItemTemplate : AlternatingItemTemplate;
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
            if (!PageReference.IsNullOrEmpty(pageLink))
                return ListingStrategy.GetPages(pageLink);

            if(dataSource != null)
                return dataSource;

            return new List<TPageData>();
        }

        protected virtual IEnumerable<TPageData> GetPagesToList()
        {
            return AccessFilteringStratey.Filter(GetAllPagesFromSource());
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
        public IAccessFilteringStrategy<TPageData> AccessFilteringStratey { get; set; }

        public IListingStrategy<TPageData> ListingStrategy { get; set; }

        public IPagingStrategy PagingStrategy { get; set; }

        public bool AutoBind { get; set; }

        IEnumerable<TPageData> dataSource;
        public IEnumerable<TPageData> DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        PageReference pageLink;
        public virtual PageReference ListingRoot
        {
            get
            {
                return PageReference.IsNullOrEmpty(pageLink) 
                    ? PageReference.EmptyReference
                    : pageLink;
            }
            set
            {
                pageLink = value;
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

        [TemplateContainer(typeof(NonPageItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ItemTemplateHeader { get; set; }

        [TemplateContainer(typeof(NonPageItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ItemTemplateFooter { get; set; }

        [TemplateContainer(typeof(NonPageItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate AlternatingItemTemplateHeader { get; set; }

        [TemplateContainer(typeof(NonPageItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate AlternatingItemTemplateFooter { get; set; }
        #endregion
    }
}
