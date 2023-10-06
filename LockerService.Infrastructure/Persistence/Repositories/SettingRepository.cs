using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities.Settings;
using LockerService.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class SettingRepository: BaseRepository<Setting>, ISettingRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public SettingRepository(ApplicationDbContext dbContext, IServiceScopeFactory scopeFactory) : base(dbContext)
    {
        _dbContext = dbContext;
        _scopeFactory = scopeFactory;
    }

    public async Task<Setting> GetSettingsEntity<T>() where T : ISetting, new()
    {
        using var scope = _scopeFactory.CreateScope();
        var scopeDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var key = typeof(T).Name;
        var setting = await scopeDbContext.Settings.FirstOrDefaultAsync(setting => Equals(setting.Key, key));
        if (setting == null)
        {
            var settingValue = SettingFactory<T>.Initialize();
            var stringValue = JsonSerializer.Serialize(settingValue);
            setting = new Setting(key, stringValue);
            await scopeDbContext.Settings.AddAsync(setting);
            await scopeDbContext.SaveChangesAsync();
        }

        return setting;
    } 
        
    public async Task<T> GetSettings<T>() where T : ISetting, new()
    {
        var settings = await GetSettingsEntity<T>();
        return JsonSerializer.Deserialize<T>(settings.Value) ?? throw new InvalidOperationException("Setting was not created");
    }

    public async Task UpdateSettings<T>(T value) where T : ISetting, new()
    {
        var settings = await GetSettingsEntity<T>();
        settings.Value = JsonSerializer.Serialize(value);
        _dbContext.Settings.Update(settings);
    }
}