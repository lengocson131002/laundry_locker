namespace LockerService.Application.Orders.Queries;

public class GetAllOrdersQuery : PaginationRequest<Order>, IRequest<PaginationResponse<Order, OrderResponse>>
{
    private string? _query;

    public string? Query
    {
        get => _query;
        set => _query = value?.Trim().ToLower();
    }

    public int? LockerId { get; set; }

    public int? BoxOrder { get; set; }

    public OrderType? Type { get; set; }

    public OrderStatus? Status { get; set; }

    public DateTimeOffset? From { get; set; }

    public DateTimeOffset? To { get; set; }


    public override Expression<Func<Order, bool>> GetExpressions()
    {
        Expression = Expression.And(order => LockerId == null || order.LockerId == LockerId);

        Expression = Expression.And(order => Type == null || order.Type == Type);

        Expression = Expression.And(order => Status == null || Status.Equals(order.Status));

        Expression = Expression.And(order => From == null || order.CreatedAt.UtcDateTime >= From);

        Expression = Expression.And(order => To == null || order.CreatedAt.UtcDateTime <= To);

        if (!string.IsNullOrWhiteSpace(Query))
        {
            Expression<Func<Order, bool>> queryExpression = PredicateBuilder.New<Order>();
            queryExpression = queryExpression.Or(order => order.SendPhone.ToLower().Contains(Query));
            queryExpression = queryExpression.Or(order =>
                order.ReceivePhone != null && order.ReceivePhone.ToLower().Contains(Query));
            queryExpression = queryExpression.Or(order => order.Locker.Name.ToLower().Contains(Query));
            Expression = Expression.And(queryExpression);
        }

        if (BoxOrder != null)
        {
            Expression<Func<Order, bool>> boxOrderExpression = PredicateBuilder.New<Order>();
            boxOrderExpression = boxOrderExpression.Or(order => order.SendBox == BoxOrder);
            boxOrderExpression = boxOrderExpression.Or(order => order.ReceiveBox == BoxOrder);

            Expression = Expression.And(boxOrderExpression);
        }

        return Expression;
    }
}