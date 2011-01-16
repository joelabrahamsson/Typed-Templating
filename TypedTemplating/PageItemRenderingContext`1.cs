using EPiServer.Core;

namespace TypedTemplating
{
    public class PageItemRenderingContext<TPageData> where TPageData : PageData
    {
        public PageItemRenderingContext(
            TPageData page, 
            int dataItemIndex, 
            int totalNumberOfPagesToRender)
        {
            Page = page;
            DataItemIndex = dataItemIndex;
            TotalNumberOfPagesToRender = totalNumberOfPagesToRender;
        }

        public TPageData Page { get; private set;}
        
        public int DataItemIndex { get; private set; }

        public int TotalNumberOfPagesToRender { get; private set; }

        public bool IsFirst
        {
            get
            {
                return DataItemIndex == 0;
            }
        }

        public bool IsLast
        {
            get
            {
                return DataItemIndex == TotalNumberOfPagesToRender - 1;
            }
        }
    }
}
