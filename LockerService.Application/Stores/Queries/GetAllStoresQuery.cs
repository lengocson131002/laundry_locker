namespace LockerService.Application.Stores.Queries;

public class GetAllStoresQuery : PaginationRequest<Store>, IRequest<PaginationResponse<Store, StoreResponse>>
{
    private string? _query;

    public string? Query
    {
        get => _query;
        set => _query = value?.Trim().ToLower();
    }

    public string? Name { get; set; }

    public StoreStatus? Status { get; set; }

    public string? Address { get; set; }
    public string? ProvinceCode { get; set; }

    public string? DistrictCode { get; set; }

    public string? WardCode { get; set; }

    public DateTimeOffset? CreatedFrom { get; set; }

    public DateTimeOffset? CreatedTo { get; set; }

    public override Expression<Func<Store, bool>> GetExpressions()
    {
        if (Query is not null)
        {
            Expression = Expression.And(store => (store.Location.Address != null && store.Location.Address.ToLower().Contains(Query)) ||
                                                 store.Name.ToLower().Contains(Query));
        }

        if (Address is not null)
        {
            Expression = Expression.And(store => store.Location.Address != null && store.Location.Address.ToLower().Contains(Address.ToLower().Trim()));
        }

        if (Name is not null)
        {
            Expression = Expression.And(store => store.Name.ToLower().Contains(Name.ToLower().Trim()));
        }

        if (Status is not null)
        {
            Expression = Expression.And(store => Equals(Status, store.Status));
        }

        if (!string.IsNullOrWhiteSpace(ProvinceCode))
        {
            Expression = Expression.And(store => Equals(ProvinceCode, store.Location.Province.Code));
        }

        if (!string.IsNullOrWhiteSpace(DistrictCode))
        {
            Expression = Expression.And(store => Equals(DistrictCode, store.Location.District.Code));
        }

        if (!string.IsNullOrWhiteSpace(WardCode))
        {
            Expression = Expression.And(store => Equals(WardCode, store.Location.Ward.Code));
        }

        if (CreatedFrom is not null)
        {
            Expression = Expression.And(store => store.CreatedAt >= CreatedFrom);
        }

        if (CreatedTo is not null)
        {
            Expression = Expression.And(store => store.CreatedAt <= CreatedTo);
        }

        return Expression;
    }
}