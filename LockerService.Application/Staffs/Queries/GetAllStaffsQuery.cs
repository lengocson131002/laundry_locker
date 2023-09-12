namespace LockerService.Application.Staffs.Queries;

public class GetAllStaffsQuery : PaginationRequest<Account>, IRequest<PaginationResponse<Account, StaffResponse>>
{
    private string? _search;

    public string? Search
    {
        get => _search;
        set => _search = value?.Trim().ToLower();
    }

    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public AccountStatus? Status { get; set; }
    public string? FullName { get; set; }
    public string? Description { get; set; }
    public long? StoreId { get; set; }

    public DateTimeOffset? CreatedFrom { get; set; }

    public DateTimeOffset? CreatedTo { get; set; }
    
    public IList<long>? ExcludedIds { get; set; }

    public override Expression<Func<Account, bool>> GetExpressions()
    {
        if (Search is not null)
        {
            Expression = Expression.And(account => (account.FullName != null && account.FullName.ToLower().Contains(Search))
                                                   || account.Username.ToLower().Contains(Search)
                                                   || (account.Description != null && account.Description.ToLower().Contains(Search))
                                                       || account.PhoneNumber.ToLower().Contains(Search));
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

        if (ExcludedIds != null)
        {
            Expression = Expression.And(account => ExcludedIds.All(id => account.Id != id));
        }

        Expression = Expression.And(account => account.IsStaff);

        return Expression;
    }
}