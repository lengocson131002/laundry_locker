namespace LockerService.Application.Settings.Models;

public class SettingResponse
{
    public long Id { get; set; }
    
    public SettingGroup? Group { get; set; }

    public string? Key { get; set; }

    public string? Value { get; set; }

    public string? Name { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
}