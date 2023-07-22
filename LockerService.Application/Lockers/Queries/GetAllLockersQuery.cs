namespace LockerService.Application.Lockers.Queries;

public class GetAllLockersQuery : PaginationRequest<Locker>, IRequest<PaginationResponse<Locker, LockerResponse>>
{
    public string? Search { get; set; }
    
    public long? StoreId { get; set; }
    
    public long? StaffId { get; set; }

    public LockerStatus? Status { get; set; }
    
    public string? ProvinceCode { get; set; }
    
    public string? DistrictCode { get; set; }
    
    public string? WardCode { get; set; }

    
    public override Expression<Func<Locker, bool>> GetExpressions()
    {
        if (Search != null)
        {
            Search = Search.Trim().ToLower();
            Expression = Expression.And(locker => locker.Name.ToLower().Contains(Search));
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

        if (StoreId != null)
        {
            Expression = Expression.And(locker => StoreId.Equals(locker.StoreId));
        }

        if (StaffId != null)
        {
            Expression = Expression.And(locker =>
                locker.Staffs.FirstOrDefault(staff => StaffId.Equals(staff.Id)) != null);
        }
        
        return Expression;
    }
}