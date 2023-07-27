using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Common.Services;

public interface ISettingService
{
    public Task<T> GetSettings<T>(CancellationToken cancellationToken = default) where T : ISetting, new();

    public Task UpdateSettings<T>(T value, CancellationToken cancellationToken = default) where T : ISetting, new();
}