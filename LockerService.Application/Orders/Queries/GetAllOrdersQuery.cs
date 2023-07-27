
namespace LockerService.Application.Orders.Queries;

public class GetAllOrdersQuery : PaginationRequest<Order>, IRequest<PaginationResponse<Order, OrderResponse>>
{
    public string? Query { get; set; }
    
    public IList<long>? LockerIds { get; set; }

    public OrderType? Type { get; set; }

    public OrderStatus? Status { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }

    public IList<long>? StaffIds { get; set; }
    
    public IList<long>? CustomerIds { get; set; }

    public IList<long>? ExcludedIds { get; set; }
    
    public IList<long>? StoreIds { get; set; }
        
    public override Expression<Func<Order, bool>> GetExpressions()
    {
        Expression = Expression.And(order => LockerIds == null || LockerIds.Any(lId => lId == order.LockerId));

        Expression = Expression.And(order => Type == null || order.Type == Type);

        Expression = Expression.And(order => Status == null || Status == order.Status);

        Expression = Expression.And(order => From == null || order.CreatedAt.UtcDateTime >= From);

        Expression = Expression.And(order => To == null || order.CreatedAt.UtcDateTime <= To);

        Expression = Expression.And(order => StaffIds == null || StaffIds.Any(sId => sId == order.StaffId.Value));

        Expression = Expression.And(order => CustomerIds == null 
                                             || CustomerIds.Any(cId => cId == order.ReceiverId.Value) 
                                             || CustomerIds.Any(cId => cId == order.SenderId));

        Expression = Expression.And(order => StoreIds == null || StoreIds.Any(sId => sId == order.Locker.StoreId));
            
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