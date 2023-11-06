
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Queries;

public class GetAllOrdersQuery : PaginationRequest<Order>, IRequest<PaginationResponse<Order, OrderResponse>>
{
    public string? Search { get; set; }
    
    public long? LockerId { get; set; }

    public OrderType? Type { get; set; }

    public OrderStatus? Status { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public long? CustomerId { get; set; }

    public IList<long>? ExcludedIds { get; set; }
    
    public long? StoreId { get; set; }
    
    public long? ServiceId { get; set; }
    
    public long? StaffId { get; set; }
    
    public bool? DeliverySupported { get; set; }
        
    public override Expression<Func<Order, bool>> GetExpressions()
    {
        Expression = Expression.And(order => LockerId == null || LockerId == order.LockerId);

        Expression = Expression.And(order => Type == null || order.Type == Type);

        Expression = Expression.And(order => Status == null || Status == order.Status);

        Expression = Expression.And(order => From == null || order.CreatedAt.UtcDateTime >= From);

        Expression = Expression.And(order => To == null || order.CreatedAt.UtcDateTime <= To);

        Expression = Expression.And(order => CustomerId == null 
                                             || (order.ReceiverId != null && CustomerId == order.ReceiverId) 
                                             || CustomerId == order.SenderId);

        Expression = Expression.And(order => StoreId == null || StoreId == order.Locker.StoreId);

        if (ServiceId != null)
        {
            Expression = Expression.And(order => order.Details.Any(item => item.ServiceId == ServiceId));
        }
            
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();
            Expression<Func<Order, bool>> queryExpression = PredicateBuilder.New<Order>();
            queryExpression = queryExpression.Or(order => order.Locker.Name.ToLower().Contains(Search));
            queryExpression = queryExpression.Or(order => order.Sender.PhoneNumber.ToLower().Contains(Search)
                                                          || (order.Receiver != null && order.Receiver.PhoneNumber
                                                              .ToLower().Contains(Search)));
            queryExpression = queryExpression.Or(order => order.PinCode != null && order.PinCode.ToLower().Contains(Search));
            
            Expression = Expression.And(queryExpression); 
        }
        
        if (ExcludedIds != null)
        {
            Expression = Expression.And(order => ExcludedIds.All(id => order.Id != id));
        }

        if (StaffId != null)
        {
            Expression = Expression.And(order => order.Timelines.Any(timeline => timeline.StaffId == StaffId));
        }

        if (DeliverySupported != null)
        {
            Expression = Expression.And(order => order.DeliverySupported == DeliverySupported);
        }
        
        return Expression;
    }
}