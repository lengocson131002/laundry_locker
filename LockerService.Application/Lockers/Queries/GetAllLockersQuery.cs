namespace LockerService.Application.Lockers.Queries;

public class GetAllLockersQuery : PaginationRequest<Locker>, IRequest<PaginationResponse<Locker, LockerResponse>>
{
    private string? _query;

    public string? Query
    {
        get => _query;
        set => _query = value?.Trim().ToLower();
    }

    public string? Name { get; set; }

    public LockerStatus? Status { get; set; }

    public string? ProvinceCode { get; set; }

    public string? DistrictCode { get; set; }

    public string? WardCode { get; set; }

    public long? StoreId { get; set; }


    public override Expression<Func<Locker, bool>> GetExpressions()
    {
        if (Query != null)
        {
            Expression = Expression.And(locker => locker.Name.ToLower().Contains(_query)
                                                  || locker.Description.ToLower().Contains(_query)
                                                  || locker.Code.ToLower().Contains(_query));
        }

        if (Name != null)
        {
            Expression = Expression.And(locker => locker.Name.ToLower().Contains(Name.ToLower()));
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
            Expression = Expression.And(locker => Equals(StoreId, locker.StoreId));
        }


        return Expression;
    }
}