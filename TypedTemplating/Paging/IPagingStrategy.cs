namespace TypedTemplating.Paging
{
    public interface IPagingStrategy
    {
        PagingSpecification GetPagingSpecification(int totalItems);
    }
}
