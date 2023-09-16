using LockerService.Application.Notifications.Models;

namespace LockerService.Application.Notifications.Queries;

public class GetAllNotificationsQuery : PaginationRequest<Notification>, IRequest<PaginationResponse<Notification, NotificationModel>>
{
    public string? Search { get; set; }
    
    public NotificationType? Type { get; set; }
    
    public EntityType? EntityType { get; set; }
    
    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; }
    
    public bool? IsRead { get; set; }
    
    public NotificationLevel? Level { get; set; }
    
    [BindNever]
    [JsonIgnore]
    public long? AccountId { get; set; }
    
    public override Expression<Func<Notification, bool>> GetExpressions()
    {
        if (Search != null)
        {
            Search = Search.Trim().ToLower();
        }

        Expression = Expression.And(notification => Search == null || notification.Content.ToLower().Contains(Search));

        Expression = Expression.And(notification => Type == null || Type.Equals(notification.Type));

        Expression = Expression.And(notification => EntityType == null || EntityType.Equals(notification.EntityType));

        Expression = Expression.And(notification => From == null || notification.CreatedAt >= From);

        Expression = Expression.And(notification => To == null || notification.CreatedAt <= To);

        Expression = Expression.And(notification => IsRead == null || notification.IsRead == IsRead);

        Expression = Expression.And(notification => AccountId == null || notification.AccountId == AccountId);

        Expression = Expression.And(notification => Level == null || notification.Level == Level);
        return Expression;
    }
}