using System.Web.UI;
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
