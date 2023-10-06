using System.Linq.Dynamic.Core;

namespace LockerService.Application.Common.Models.Request;

public abstract class FilterRequest<T> where T : class
{
    public string? SortColumn { get; set; }

    public SortDirection SortDir { get; set; } = SortDirection.Asc;
    
    protected Expression<Func<T, bool>> Expression = PredicateBuilder.New<T>(true);

    public abstract Expression<Func<T, bool>> GetExpressions();

    public Func<IQueryable<T>, IOrderedQueryable<T>>? GetOrder()
    {
        if (string.IsNullOrWhiteSpace(SortColumn)) return null;

        return query => query.OrderBy($"{SortColumn} {SortDir.ToString().ToLower()}");
    }
}