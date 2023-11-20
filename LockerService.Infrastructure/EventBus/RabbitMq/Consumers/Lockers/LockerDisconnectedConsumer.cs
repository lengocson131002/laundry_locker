using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerDisconnectedConsumer : IConsumer<LockerDisconnectedEvent>
{
    private readonly ILogger _logger;
    private readonly INotifier _notifier;
    private readonly IUnitOfWork _unitOfWork;

    public LockerDisconnectedConsumer(ILogger<LockerDisconnectedConsumer> logger, IUnitOfWork unitOfWork,
        INotifier notifier)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<LockerDisconnectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Receive locker disconnected message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository
            .Get(
                locker => locker.Code == message.LockerCode,
                includes: new List<Expression<Func<Locker, object>>>
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                    locker => locker.Store
                })
            .FirstOrDefaultAsync();

        if (locker == null) return;

        var lockerInfoData = JsonSerializer.Serialize(locker, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        var @event = new LockerTimeline
        {
            LockerId = locker.Id,
            Event = LockerEvent.Disconnect,
            Status = LockerStatus.Disconnected,
            PreviousStatus = locker.Status,
            Data = lockerInfoData
        };
        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);

        locker.Status = LockerStatus.Disconnected;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);

        await _unitOfWork.SaveChangesAsync();

        // Push notification store's staffs
        var staffs = await _unitOfWork.AccountRepository
            .GetStaffs(locker.StoreId)
            .ToListAsync();

        foreach (var staff in staffs)
        {
            var notification = new Notification(
                staff,
                NotificationType.SystemLockerDisconnected,
                EntityType.Locker,
                locker,
                true
            );
            await _notifier.NotifyAsync(notification);
        }

        // Push notification admins
        var admins = await _unitOfWork.AccountRepository
            .GetStaffs(roles: new List<Role> { Role.Admin })
            .ToListAsync();

        foreach (var admin in admins)
        {
            var notification = new Notification(
                admin,
                NotificationType.SystemLockerDisconnected,
                EntityType.Locker,
                locker
            );
            await _notifier.NotifyAsync(notification);
        }
    }
}