namespace LockerService.Application.Lockers.Queries;

public class GetAllLockersQuery : PaginationRequest<Locker>, IRequest<PaginationResponse<Locker, LockerResponse>>
{
    public string? Search { get; set; }
    
    public long? StoreId { get; set; }
    
    public LockerStatus? Status { get; set; }
    
    public string? ProvinceCode { get; set; }
    
    public string? DistrictCode { get; set; }
    
    public string? WardCode { get; set; }

    public IList<long>? ExcludedIds { get; set; }

    public long? ForStaffId { get; set; }
    
    public override Expression<Func<Locker, bool>> GetExpressions()
    {
        if (Search != null)
        {
            Search = Search.Trim().ToLower();
            Expression = Expression.And(locker => locker.Name.ToLower().Contains(Search) || locker.Code.ToLower().Contains(Search));
        }

        if (Status != null)
        {
            Expression = Expression.And(locker => locker.Status == Status);
        }

        if (!string.IsNullOrWhiteSpace(ProvinceCode))
        {
            Expression = Expression.And(locker => ProvinceCode == locker.Location.Province.Code);
        }

        if (!string.IsNullOrWhiteSpace(DistrictCode))
        {
            Expression = Expression.And(locker => DistrictCode == locker.Location.District.Code);
        }

        if (!string.IsNullOrWhiteSpace(WardCode))
        {
            Expression = Expression.And(locker => WardCode == locker.Location.Ward.Code);
        }

        if (StoreId != null)
        {
            Expression = Expression.And(locker => StoreId == locker.StoreId);
        }
        
        if (ExcludedIds != null)
        {
            Expression = Expression.And(locker => ExcludedIds.All(id => locker.Id != id));
        }

        if (ForStaffId != null)
        {
            Expression = Expression.And(locker => locker.StaffLockers.All(item => item.StaffId != ForStaffId));
        }
            
        return Expression;
    }
}