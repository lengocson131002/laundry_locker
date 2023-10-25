using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.Services;

public class LockersService : ILockersService
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly ISettingService _settingService;

    private readonly INotifier _notifier;
    
    public LockersService(IUnitOfWork unitOfWork, ISettingService settingService, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _settingService = settingService;
        _notifier = notifier;
    }

    public async Task CheckLockerBoxAvailability(Locker locker)
    {
        var lockerSettings = await _settingService.GetSettings<LockerSettings>();
        
        var availableBoxes = await _unitOfWork.BoxRepository.FindAvailableBoxes(locker.Id);

        if (availableBoxes.Count <= lockerSettings.AvailableBoxCountWarning)
        {
            var staffs =await _unitOfWork.AccountRepository
                .GetStaffs(storeId: locker.StoreId)
                .ToListAsync();
            
            foreach (var staff in staffs)
            {
                var notification = new Notification(
                    account: staff,
                    type: NotificationType.SystemLockerBoxWarning,
                    entityType: EntityType.Locker,
                    data: locker
                );

                await _notifier.NotifyAsync(notification);
            }
        }
    }
}