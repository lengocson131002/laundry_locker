
namespace LockerService.Application.Orders.Queries;

public class GetAllOrdersQuery : PaginationRequest<Order>, IRequest<PaginationResponse<Order, OrderResponse>>
{
    public string? Query { get; set; }
    
    public long? LockerId { get; set; }

    public OrderType? Type { get; set; }

    public OrderStatus? Status { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public long? StaffId { get; set; }
    
    public long? CustomerId { get; set; }

    public IList<long>? ExcludedIds { get; set; }
    
    public long? StoreId { get; set; }
        
    public override Expression<Func<Order, bool>> GetExpressions()
    {
        Expression = Expression.And(order => LockerId == null || LockerId == order.LockerId);

        Expression = Expression.And(order => Type == null || order.Type == Type);

        Expression = Expression.And(order => Status == null || Status == order.Status);

        Expression = Expression.And(order => From == null || order.CreatedAt.UtcDateTime >= From);

        Expression = Expression.And(order => To == null || order.CreatedAt.UtcDateTime <= To);

        Expression = Expression.And(order => StaffId == null || StaffId == order.StaffId.Value);

        Expression = Expression.And(order => CustomerId == null 
                                             || CustomerId == order.ReceiverId.Value 
                                             || CustomerId == order.SenderId);

        Expression = Expression.And(order => StoreId == null || StoreId == order.Locker.StoreId);
            
        if (!string.IsNullOrWhiteSpace(Query))
        {
            Query = Query.Trim().ToLower();
            Expression<Func<Order, bool>> queryExpression = PredicateBuilder.New<Order>();
            queryExpression = queryExpression.Or(order => order.Locker.Name.ToLower().Contains(Query));
            queryExpression = queryExpression.Or(order => order.Staff != null &&
                                                          (order.Staff.PhoneNumber.ToLower().Contains(Query)
                                                           || order.Staff.FullName.ToLower().Contains(Query)));

            queryExpression = queryExpression.Or(order => order.Sender.PhoneNumber.ToLower().Contains(Query)
                                                          || (order.Receiver != null && order.Receiver.PhoneNumber
                                                              .ToLower().Contains(Query)));
            
            Expression = Expression.And(queryExpression); 
        }
        
        if (ExcludedIds != null)
        {
            Expression = Expression.And(order => ExcludedIds.All(id => order.Id != id));
        }

        return Expression;
    }
}