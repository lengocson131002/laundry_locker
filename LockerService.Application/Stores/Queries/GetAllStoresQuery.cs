namespace LockerService.Application.Stores.Queries;

public class GetAllStoresQuery : PaginationRequest<Store>, IRequest<PaginationResponse<Store, StoreResponse>>
{
    public string? Name { get; set; }

    public StoreStatus? Status { get; set; }
    
    public string? ProvinceCode { get; set; }
    
    public string? DistrictCode { get; set; }
    
    public string? WardCode { get; set; }
    
    public override Expression<Func<Store, bool>> GetExpressions()
    {
        if (Name != null)
        {
            Expression = Expression.And(store => store.Name.ToLower().Contains(Name.ToLower()));
        }
        
        if (Status != null)
        {
            Expression = Expression.And(store => store.Status.Equals(Status));
        }

        if (!string.IsNullOrWhiteSpace(ProvinceCode))
        {
            Expression = Expression.And(store => ProvinceCode.Equals(store.Location.Province.Code));
        }

        if (!string.IsNullOrWhiteSpace(DistrictCode))
        {
            Expression = Expression.And(store => DistrictCode.Equals(store.Location.District.Code));
        }

        if (!string.IsNullOrWhiteSpace(WardCode))
        {
            Expression = Expression.And(store => WardCode.Equals(store.Location.Ward.Code));
        }
        
        
        return Expression;
    }
}