using LockerService.Application.Common.Services;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Infrastructure.Services;

public class SettingService : ISettingService
{
    private const string SettingRedisPrefix = "Settings-";
        
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public SettingService(ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<T> GetSettings<T>(CancellationToken cancellationToken = default) where T : ISetting, new()
    {
        var key = $"{SettingRedisPrefix}{typeof(T).Name}";
        return await _cacheService.GetAsync<T>(key, () => _unitOfWork.SettingRepository.GetSettings<T>(), cancellationToken);
    }

    public async Task UpdateSettings<T>(T value, CancellationToken cancellationToken = default) where T : ISetting, new()
    {
        var key = $"{SettingRedisPrefix}{typeof(T).Name}";
        await _unitOfWork.SettingRepository.UpdateSettings(value);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetAsync(key, value, cancellationToken);
    }
}