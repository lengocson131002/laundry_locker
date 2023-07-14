using LockerService.Application.Staffs.Models;

namespace LockerService.Application.Staffs.Queries;

public class GetAllStaffsQuery : PaginationRequest<Account>, IRequest<PaginationResponse<Account, StaffResponse>>
{
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public AccountStatus? Status { get; set; }
    public string? FullName { get; set; }
    public string? Description { get; set; }
    public long? StoreId { get; set; }

    public override Expression<Func<Account, bool>> GetExpressions()
    {
        if (Username != null)
        {
            Expression = Expression.And(account => account.Username.ToLower().Contains(Username.ToLower()));
        }

        if (PhoneNumber != null)
        {
            Expression = Expression.And(account => account.PhoneNumber.ToLower().Contains(PhoneNumber.ToLower()));
        }

        if (FullName != null)
        {
            Expression = Expression.And(account => account.FullName.ToLower().Contains(FullName.ToLower()));
        }

        if (Description != null)
        {
            Expression = Expression.And(account => account.Description.ToLower().Contains(Description.ToLower()));
        }

        if (StoreId != null)
        {
            Expression = Expression.And(account => StoreId == account.StoreId);
        }

        if (Status != null)
        {
            Expression = Expression.And(account => account.Status.Equals(Status));
        }

        Expression = Expression.And(account => Equals(Role.Staff, account.Role));

        return Expression;
    }
}