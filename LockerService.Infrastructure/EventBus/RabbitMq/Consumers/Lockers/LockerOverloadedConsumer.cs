using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerOverloadedConsumer : IConsumer<LockerOverloadedEvent>
{
    private readonly ILogger<LockerOverloadedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public LockerOverloadedConsumer(ILogger<LockerOverloadedConsumer> logger, IUnitOfWork unitOfWork, INotifier notifier)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<LockerOverloadedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker overloaded event: {}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository
            .Get(
                predicate: locker => locker.Id == message.LockerId,
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                    locker => locker.Store,
                })
            .FirstOrDefaultAsync();
            
        if (locker == null)
        {
            return;
        }
        
        var lockerInfoData = JsonSerializer.Serialize(locker, JsonSerializerUtils.GetGlobalJsonSerializerOptions());

        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.Overload,
            Status = locker.Status,
            Data = lockerInfoData,
            Error = message.Error,
            ErrorCode = message.ErrorCode
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();

        // Notify staffs managing this locker
        var staffs = await _unitOfWork.StaffLockerRepository.GetStaffs(locker.Id);
        foreach (var staff in staffs)
        {
            var notification = new Notification(
                account: staff,
                type: NotificationType.SystemLockerBoxOverloaded,
                entityType: EntityType.Locker,
                data: locker
            );
            
            await _notifier.NotifyAsync(notification);
        }

    }
}