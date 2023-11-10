using LockerService.Application.Features.Payments.Models;
using LockerService.Domain;

namespace LockerService.Application.Features.Payments.Queries;

public class GetAllPaymentQuery : PaginationRequest<Payment>, IRequest<PaginationResponse<Payment, PaymentResponse>>
{
    public string? Search { get; set; }
    
    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; }
    
    public PaymentStatus? Status { get; set; }
    
    public long? OrderId { get; set; }
    
    public long? CustomerId { get; set; }
    
    public PaymentMethod? Method { get; set; }
    
    public long? StoreId { get; set; }
    
    public override Expression<Func<Payment, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            Expression = Expression.And(payment =>
                (payment.Order.PinCode != null && payment.Order.PinCode.ToLower().Contains(Search)) 
                || payment.Customer.PhoneNumber.ToLower().Contains(Search));
        }

        if (From != null)
        {
            Expression = Expression.And(payment => payment.CreatedAt >= From);
        }

        if (To != null)
        {
            Expression = Expression.And(payment => payment.CreatedAt <= To);
        }

        if (Status != null)
        {
            Expression = Expression.And(payment => payment.Status == Status);
        }

        if (OrderId != null)
        {
            Expression = Expression.And(payment => payment.OrderId == OrderId);
        }

        if (CustomerId != null)
        {
            Expression = Expression.And(payment => payment.CustomerId == CustomerId);
        }

        if (Method != null)
        {
            Expression = Expression.And(payment => payment.Method == Method);
        }

        if (StoreId != null)
        {
            Expression = Expression.And(payment => payment.Order.Locker.StoreId == StoreId);
        }
        
        return Expression;
    }
}