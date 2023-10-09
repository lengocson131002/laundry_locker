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
            var lockerInfoData = JsonSerializerUtils.Serialize(locker);
            
            var laundryAttendants =await _unitOfWork.AccountRepository
                .GetStaffs(
                    storeId: locker.StoreId, 
                    role: Role.LaundryAttendant,
                    isActive: true)
                .ToListAsync();
            
            foreach (var la in laundryAttendants)
            {
                var notification = new Notification(
                    account: la,
                    type: NotificationType.SystemLockerBoxWarning,
                    entityType: EntityType.Locker,
                    data: lockerInfoData
                );

                await _notifier.NotifyAsync(notification);
            }
        }
    }
}