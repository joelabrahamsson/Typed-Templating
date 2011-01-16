namespace TypedTemplating.Paging
{
    public class NoPaging : IPagingStrategy
    {
        public PagingSpecification GetPagingSpecification(int totalItems)
        {
            return new PagingSpecification(int.MaxValue - 1, 1);
        }
    }
}
