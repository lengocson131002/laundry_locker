namespace LockerService.Application.Services.Queries;

public class GetAllServicesQuery : PaginationRequest<Service>, IRequest<PaginationResponse<Service, ServiceResponse>>
{
    [BindNever] 
    public int? LockerId { get; set; }
    
    public string? Name { get; set; }

    public bool? IsActive { get; set; }
    
    public override Expression<Func<Service, bool>> GetExpressions()
    {
        if (LockerId != null)
        {
            Expression = Expression.And(service => service.LockerId == LockerId);
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            Expression = Expression.And(service => service.Name.ToLower().Contains(Name.Trim().ToLower()));
        }
        
        if (IsActive != null)
        {
            Expression = Expression.And(service => IsActive == true ? service.DeletedAt == null : service.DeletedAt != null);
        }

        return Expression;
    }
}