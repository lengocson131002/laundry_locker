using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Services.Queries;

public class GetAllServicesQuery : PaginationRequest<Service>, IRequest<PaginationResponse<Service, ServiceResponse>>
{
    public long? StoreId { get; set; }
    
    public long? ExcludedStoreId { get; set; }
    
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

        // If storeId passed, query all configured service for this store
        if (StoreId != null)
        {
            Expression = Expression.And(service => service.StoreServices.Any(item => item.StoreId == StoreId));
        }
        
        // else get all global services
        else
        {
            Expression = Expression.And(service => Equals(service.StoreId, null));
        }

        // Get all service that a store not configured
        if (ExcludedStoreId != null)
        {
            Expression = Expression.And(service => service.StoreServices.All(item => item.StoreId != ExcludedStoreId));
        }
        
        return Expression;
    }
}