using System.ComponentModel;
using System.Web.UI;
using EPiServer.Core;
using TypedTemplating.Filtering;
using TypedTemplating.ItemClassification;

namespace TypedTemplating
{
    public class TypedMenuList<TPageData> : TypedPageList<TPageData>
        where TPageData : PageData
    {
        public TypedMenuList()
        {
            SelectedItemStrategy = new BranchRootExcludingListingRoot<TPageData>();
            FilteringStrategy = new VisibleInMenus<TPageData>();
        }

        #region Control hiearchy build up
        protected override ITemplate GetItemTemplate(PageListPageItem<TPageData> pageItem)
        {
            if(!IsSelected(pageItem))
                return base.GetItemTemplate(pageItem);

            if (pageItem.IsFirstPageItem && SelectedFirstItemTemplate != null)
                return SelectedFirstItemTemplate;

            if (pageItem.IsLastPageItem && SelectedLastItemTemplate != null)
                return SelectedLastItemTemplate;

            if (SelectedItemTemplate != null)
                return SelectedItemTemplate;

            return base.GetItemTemplate(pageItem);
        }

        protected virtual bool IsSelected(PageListPageItem<TPageData> pageItem)
        {
            return SelectedItemStrategy.IsSelected(pageItem);
        }

        protected override ITemplate GetItemHeaderTemplate(PageListPageItem<TPageData> pageItem)
        {
            if (!IsSelected(pageItem))
                return base.GetItemHeaderTemplate(pageItem);

            if (pageItem.IsFirstPageItem && SelectedFirstItemHeaderTemplate != null)
                return SelectedFirstItemHeaderTemplate;

            if (pageItem.IsLastPageItem && SelectedLastItemHeaderTemplate != null)
                return SelectedLastItemHeaderTemplate;

            if (SelectedItemHeaderTemplate != null)
                return SelectedItemHeaderTemplate;

            return base.GetItemHeaderTemplate(pageItem);
        }

        protected override ITemplate GetItemFooterTemplate(PageListPageItem<TPageData> pageItem)
        {
            if (!IsSelected(pageItem))
                return base.GetItemFooterTemplate(pageItem);

            if (pageItem.IsFirstPageItem && SelectedFirstItemFooterTemplate != null)
                return SelectedFirstItemFooterTemplate;

            if (pageItem.IsLastPageItem && SelectedLastItemFooterTemplate != null)
                return SelectedLastItemFooterTemplate;

            if (SelectedItemFooterTemplate != null)
                return SelectedItemFooterTemplate;

            return base.GetItemFooterTemplate(pageItem);
        }
        #endregion

        #region Configurables
        public ISelectedItemStrategy<TPageData> SelectedItemStrategy { get; set; }
        #endregion

        #region Templates
        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedItemTemplate { get; set; }

        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedFirstItemTemplate { get; set; }

        [TemplateContainer(typeof(PageListPageItem<>))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedLastItemTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedItemHeaderTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedItemFooterTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedFirstItemHeaderTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedFirstItemFooterTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedLastItemHeaderTemplate { get; set; }

        [TemplateContainer(typeof(PageListItem))]
        [Browsable(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedLastItemFooterTemplate { get; set; }
        #endregion
    }
}
