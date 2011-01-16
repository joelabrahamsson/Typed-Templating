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
        
        public virtual PageData CurrentPage
        {
            get
            {
                return pageSource.CurrentPage;
            }
        }

        public PageDataCollection GetChildren(PageReference pageLink)
        {
            return pageSource.GetChildren(pageLink);
        }

        public PageData GetPage(PageReference pageLink)
        {
            return PageSource.GetPage(pageLink);
        }

        private IPageSource pageSource;
        private IPageSource PageSource
        {
            get
            {
                if (pageSource != null)
                    return pageSource;

                for (Control control = Parent; (control != Page) && (control != null); control = control.Parent)
                {
                    pageSource = control as IPageSource;
                    if (pageSource != null)
                    {
                        break;
                    }
                }
                if (pageSource == null)
                {
                    pageSource = Page as IPageSource;
                }
                if (pageSource == null)
                {
                    pageSource = DataFactory.Instance;
                }

                return pageSource;
            }
        }
    }
}
