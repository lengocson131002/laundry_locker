namespace LockerService.Application.Common.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    Task RemoveAsync<T>(string key, CancellationToken cancellationToken = default);
    
     
}