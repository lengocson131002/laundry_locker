namespace LockerService.Application.Dashboard.Queries;

public class DashboardStoreQuery : PaginationRequest<DashboardStoreItem>, IRequest<PaginationResponse<DashboardStoreItem>>
{
    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }
    
    public string? Search { get; set; }

    public override Expression<Func<DashboardStoreItem, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            Expression = Expression.And(item => item.Name.ToLower().Contains(Search));
        }
        return Expression;
    }
}