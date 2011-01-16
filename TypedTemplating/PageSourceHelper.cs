using System.Web.UI;
using EPiServer;
using EPiServer.Core;

namespace TypedTemplating
{
    public class PageSourceHelper
    {
        public static IPageSource GetPageSourceForControl(Control control)
        {
            IPageSource pageSource;
            for (Control parent = control.Parent; (parent != control.Page) && (parent != null); parent = parent.Parent)
            {
                pageSource = parent as IPageSource;
                if (pageSource != null)
                {
                    return pageSource;
                }
            }
            pageSource = control.Page as IPageSource;
            if (pageSource != null)
                return pageSource;

            return DataFactory.Instance;
        }
    }
}
