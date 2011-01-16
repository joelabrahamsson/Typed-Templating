using System.Web;

namespace TypedTemplating.Paging
{
    public class QueryStringPaging : IPagingStrategy
    {
        const string DefaultQueryStringKey = "q";
        const int DefaultPageSize = 10;
        readonly HttpContextBase httpContext;
        
        public QueryStringPaging(HttpContextBase httpContext)
        {
            QueryStringKey = DefaultQueryStringKey;
            PageSize = DefaultPageSize;
            this.httpContext = httpContext;
        }

        public QueryStringPaging()
            : this(new HttpContextWrapper(HttpContext.Current))
        { }

        public PagingSpecification GetPagingSpecification(int totalItems)
        {
            int pageNumber = 1;
            string queryStringValue = httpContext.Request.QueryString[QueryStringKey];
            if (queryStringValue != null)
            {
                int parsedPageNumber;
                if (int.TryParse(queryStringValue, out parsedPageNumber))
                {
                    pageNumber = parsedPageNumber;
                }
            }

            return new PagingSpecification(PageSize, pageNumber);
        }

        public string QueryStringKey { get; set; }

        public int PageSize { get; set; }
    }
}
