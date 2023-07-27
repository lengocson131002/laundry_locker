namespace LockerService.Domain.Entities.Settings;

public abstract class SettingFactory<T> where T : ISetting, new()
{
    public static T Initialize()
    {
        return new T();
    }
}