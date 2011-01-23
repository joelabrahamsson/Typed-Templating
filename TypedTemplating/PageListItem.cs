using System.ComponentModel;
using System.Web.UI;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating
{
    public class PageListItem : Control, INamingContainer, IPageSource
    {
        public PageListItem(int itemIndex)
        {
            ItemIndex = itemIndex;
        }

        public int ItemIndex { get; private set; }

        PageBase page;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PageBase PageBase
        {
            get
            {
                if (page == null)
                {
                    page = Page as PageBase;
                    if (page != null)
                    {
                        return page;
                    }
                    page = this.Context.Handler as PageBase;
                    if (page == null)
                    {
                        throw new EPiServerException("This user control must be placed on an ASPX-page that inherits from EPiServer.PageBase");
                    }
                }
                return page;
            }
        }
 


        #region IPageSource
        public virtual PageData CurrentPage
        {
            get
            {
                return PageSource.CurrentPage;
            }
        }

        PageDataCollection IPageSource.GetChildren(PageReference pageLink)
        {
            return PageSource.GetChildren(pageLink);
        }

        PageData IPageSource.GetPage(PageReference pageLink)
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
