namespace LockerService.Domain.Entities.Settings;

public class AccountSettings
{
    public int MaxWrongLoginCount { get; set; }
    
    public int WrongLoginBlockTimeInMinutes { get; set; }
    
    public static AccountSettings Initialize()
    {
        return new AccountSettings();
    }
}