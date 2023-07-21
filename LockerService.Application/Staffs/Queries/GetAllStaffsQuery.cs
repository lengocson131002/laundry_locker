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

    public override Expression<Func<Account, bool>> GetExpressions()
    {
        if (Query is not null)
        {
            Expression = Expression.And(account => account.FullName.ToLower().Contains(_query)
                                                   || account.Username.ToLower().Contains(_query)
                                                   || account.Description.ToLower().Contains(_query)
                                                   || account.PhoneNumber.ToLower().Contains(_query));
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
            Expression = Expression.And(account => account.FullName.ToLower().Contains(FullName.ToLower()));
        }

        if (Description is not null)
        {
            Expression = Expression.And(account => account.Description.ToLower().Contains(Description.ToLower()));
        }

        if (StoreId is not null)
        {
            Expression = Expression.And(account => StoreId == account.StoreId);
        }

        if (Status is not null)
        {
            Expression = Expression.And(account => account.Status.Equals(Status));
        }

        if (CreatedFrom is not null)
        {
            Expression = Expression.And(account => CreatedFrom <= account.CreatedAt);
        }

        if (CreatedTo is not null)
        {
            Expression = Expression.And(account => CreatedTo >= account.CreatedAt);
        }

        Expression = Expression.And(account => Equals(Role.Staff, account.Role));

        return Expression;
    }
}