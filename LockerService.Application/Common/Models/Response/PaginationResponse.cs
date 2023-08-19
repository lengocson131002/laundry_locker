namespace LockerService.Application.Common.Models.Response;

public class PaginationResponse<TEntity, TResponse> where TEntity : class where TResponse : class
{
    public  PaginationResponse(IList<TResponse> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public PaginationResponse(IList<TEntity> items, int count, int pageNumber, int pageSize,
        Func<TEntity, TResponse> mapper)
    {
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items.Select(mapper).ToList();
    }

    public PaginationResponse(IQueryable<TEntity> source, int pageNumber, int pageSize, Func<TEntity, TResponse> mapper)
    {
        TotalCount = source.Count();
        PageSize = pageSize;
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);

        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        Items = items.Select(mapper).ToList();
    }

    public int PageNumber { get; }

    public int TotalPages { get; }

    public int PageSize { get; set; }

    public int TotalCount { get; }

    public bool HasPrevious => PageNumber > 1;

    public bool HasNext => PageNumber < TotalPages;

    public IList<TResponse> Items { get; }
}

public class PaginationResponse<TEntity> : PaginationResponse<TEntity, TEntity> where TEntity : class
{
    public PaginationResponse(IList<TEntity> items, int count, int pageNumber, int pageSize) 
        : base(items, count, pageNumber, pageSize)
    {
    }

    public PaginationResponse(IList<TEntity> items, int count, int pageNumber, int pageSize, Func<TEntity, TEntity> mapper) 
        : base(items, count, pageNumber, pageSize, mapper)
    {
    }

    public PaginationResponse(IQueryable<TEntity> source, int pageNumber, int pageSize, Func<TEntity, TEntity> mapper) 
        : base(source, pageNumber, pageSize, mapper)
    {
    }

    public PaginationResponse(IQueryable<TEntity> source, int pageNumber, int pageSize) : base(source, pageNumber, pageSize, entity => entity)
    {
    }

}