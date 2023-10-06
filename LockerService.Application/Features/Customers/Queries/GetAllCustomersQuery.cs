using LockerService.Application.Features.Customers.Models;

namespace LockerService.Application.Features.Customers.Queries;

public class GetAllCustomersQuery : PaginationRequest<Account>, IRequest<PaginationResponse<Account, CustomerResponse>>
{
    public string? Search { get; set; }
    
    public AccountStatus? Status { get; set; }
    
    public IList<long>? ExcludedIds { get; set; }

    public override Expression<Func<Account, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            var queryExpression =  PredicateBuilder.New<Account>(true);
            queryExpression.Or(cus => cus.FullName != null && cus.FullName.ToLower().Contains(Search));
            queryExpression.Or(cus => cus.Username.ToLower().Contains(Search));
            queryExpression.Or(cus => cus.PhoneNumber.ToLower().Contains(Search));
            queryExpression.Or(cus => cus.Description != null && cus.Description.ToLower().Contains(Search));
            Expression = Expression.And(queryExpression);
        }
        
        if (Status != null)
        {
            Expression = Expression.And(cus => Status.Equals(cus.Status));
        }

        if (ExcludedIds != null)
        {
            Expression = Expression.And(cus => ExcludedIds.All(id => cus.Id != id));
        }

        Expression = Expression.And(acc => Role.Customer.Equals(acc.Role));

        return Expression;
    }
}