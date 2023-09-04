using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerDisconnectedConsumer : IConsumer<LockerDisconnectedEvent>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public LockerDisconnectedConsumer(ILogger<LockerDisconnectedConsumer> logger, IUnitOfWork unitOfWork, INotifier notifier)
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
                predicate: locker => locker.Code == message.LockerCode,
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
            Event = LockerEvent.Disconnect,
            Status = LockerStatus.Disconnected,
            PreviousStatus = locker.Status,
            Data = lockerInfoData,
        };
        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        
        locker.Status = LockerStatus.Disconnected;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);

        await _unitOfWork.SaveChangesAsync();
        
        // Push notification to admins
        var admins = await _unitOfWork.AccountRepository.GetAdmins().ToListAsync();
        foreach (var admin in admins)
        {
            var notification = new Notification()
            {
                Account = admin,
                Type = NotificationType.LockerDisconnected,
                Content = NotificationType.LockerDisconnected.GetDescription(),
                EntityType = EntityType.Locker,
                Data = lockerInfoData,
                ReferenceId = locker.Id.ToString()
            };

            await _notifier.NotifyAsync(notification);
        }
    }
}