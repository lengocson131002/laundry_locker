using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Notification?> GetNotification(long accountId, long notificationId)
    {
        return _dbContext.Notifications
            .Where(notification => notification.Id == notificationId
                                   && notification.AccountId == accountId
                                   && !notification.Deleted)
            .FirstOrDefaultAsync();
    }
}