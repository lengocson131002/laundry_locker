namespace LockerService.Application.Common.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}