namespace LockerService.Application.Services.Queries;

public class GetAllServicesQuery : PaginationRequest<Service>, IRequest<PaginationResponse<Service, ServiceResponse>>
{
    public string? Query { get; set; }

    public ServiceStatus? Status { get; set; }
    
    public override Expression<Func<Service, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Query))
        {
            Expression = Expression.And(service => service.Name.ToLower().Contains(Query.Trim().ToLower()));
        }
        
        if (Status != null)
        {
            Expression = Expression.And(service => Status.Equals(service.Status));
        }

        return Expression;
    }
}