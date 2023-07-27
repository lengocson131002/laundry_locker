namespace LockerService.Application.Staffs.Queries;

public class GetAllStaffsQuery : PaginationRequest<Account>, IRequest<PaginationResponse<Account, StaffResponse>>
{
    private string? _query;

    public string? Query
    {
        get => _query;
        set => _query = value?.Trim().ToLower();
    }

    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public AccountStatus? Status { get; set; }
    public string? FullName { get; set; }
    public string? Description { get; set; }
    public long? StoreId { get; set; }

    public DateTimeOffset? CreatedFrom { get; set; }

    public DateTimeOffset? CreatedTo { get; set; }
    
    public long? LockerId { get; set; }

    public IList<long>? ExcludedIds { get; set; }
    
    public override Expression<Func<Account, bool>> GetExpressions()
    {
        if (Query is not null)
        {
            Expression = Expression.And(account => (account.FullName != null && account.FullName.ToLower().Contains(Query))
                                                   || account.Username.ToLower().Contains(Query)
                                                   || (account.Description != null && account.Description.ToLower().Contains(Query))
                                                       || account.PhoneNumber.ToLower().Contains(Query));
        }

        if (Username is not null)
        {
            Expression = Expression.And(account => account.Username.ToLower().Contains(Username.ToLower()));
        }

        if (PhoneNumber is not null)
        {
            Expression = Expression.And(account => account.PhoneNumber.ToLower().Contains(PhoneNumber.ToLower()));
        }

        if (FullName is not null)
        {
            Expression = Expression.And(account => account.FullName != null && account.FullName.ToLower().Contains(FullName.ToLower()));
        }

        if (Description is not null)
        {
            Expression = Expression.And(account => account.Description != null && account.Description.ToLower().Contains(Description.ToLower()));
        }

        if (StoreId is not null)
        {
            Expression = Expression.And(account => StoreId == account.StoreId);
        }

        if (Status is not null)
        {
            Expression = Expression.And(account => account.Status == Status);
        }

        if (CreatedFrom is not null)
        {
            Expression = Expression.And(account => CreatedFrom <= account.CreatedAt);
        }

        if (CreatedTo is not null)
        {
            Expression = Expression.And(account => CreatedTo >= account.CreatedAt);
        }

        if (LockerId != null)
        {
            Expression = Expression.And(account => account.StaffLockers.FirstOrDefault(lo => lo.LockerId == LockerId) != null);
        }

        if (ExcludedIds != null)
        {
            Expression = Expression.And(account => ExcludedIds.All(id => account.Id != id));
        }
        
        Expression = Expression.And(account => Equals(Role.Staff, account.Role));

        return Expression;
    }
}