namespace LockerService.Application.Services.Queries;

public class GetAllServicesQuery : PaginationRequest<Service>, IRequest<PaginationResponse<Service, ServiceResponse>>
{
    [JsonIgnore]
    [BindNever]
    public long? StoreId { get; set; }
    
    [JsonIgnore]
    [BindNever]
    public long? LockerId { get; set; }

    public string? Search { get; set; }

    public ServiceStatus? Status { get; set; }
    
    public IList<long>? ExcludedIds { get; set; }
    

    public override Expression<Func<Service, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            Expression = Expression.And(service => service.Name.ToLower().Contains(Search));
        }
        
        if (Status != null)
        {
            Expression = Expression.And(service => Status.Equals(service.Status));
        }
        
        if (ExcludedIds != null)
        {
            Expression = Expression.And(service => ExcludedIds.All(id => service.Id != id));
        }

        if (StoreId != null)
        {
            Expression = Expression.And(service => Equals(service.StoreId, StoreId));
        }

        return Expression;
    }
}