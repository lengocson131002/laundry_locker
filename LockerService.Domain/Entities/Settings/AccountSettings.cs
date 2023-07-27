namespace LockerService.Domain.Entities.Settings;

public class AccountSettings : ISetting
{
    public int MaxWrongLoginCount { get; set; }
    
    public int WrongLoginBlockTimeInMinutes { get; set; }

    public AccountSettings()
    {
        
    }
}