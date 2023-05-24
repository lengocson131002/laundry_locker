namespace LockerService.Application.Lockers.Queries;

public class GetAllLockersQuery : PaginationRequest<Locker>, IRequest<PaginationResponse<Locker, LockerResponse>>
{
    public string? Name { get; set; }

    public string? MacAddress { get; set; }
    
    public LockerStatus? Status { get; set; }
    
    public string? ProvinceCode { get; set; }
    
    public string? DistrictCode { get; set; }
    
    public string? WardCode { get; set; }
    
    public override Expression<Func<Locker, bool>> GetExpressions()
    {
        if (Name != null)
        {
            Expression = Expression.And(locker => locker.Name.ToLower().Contains(Name.ToLower()));
        }

        if (MacAddress != null)
        {
            Expression = Expression.And(locker => locker.MacAddress.ToLower().Contains(MacAddress.ToLower()));
        }
        
        if (Status != null)
        {
            Expression = Expression.And(locker => locker.Status.Equals(Status));
        }

        if (!string.IsNullOrWhiteSpace(ProvinceCode))
        {
            Expression = Expression.And(locker => ProvinceCode.Equals(locker.Location.Province.Code));
        }

        if (!string.IsNullOrWhiteSpace(DistrictCode))
        {
            Expression = Expression.And(locker => DistrictCode.Equals(locker.Location.District.Code));
        }

        if (!string.IsNullOrWhiteSpace(WardCode))
        {
            Expression = Expression.And(locker => WardCode.Equals(locker.Location.Ward.Code));
        }
        
        
        return Expression;
    }
}