namespace TypedTemplating.Paging
{
    public class PagingSpecification
    {
        public PagingSpecification(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int PageSize { get; private set; }
        public int PageNumber { get; private set; }
    }
}
