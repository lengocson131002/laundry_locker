using LockerService.Application.Settings.Models;

namespace LockerService.Application.Settings.Queries;

public class GetSettingsQuery : IRequest<SettingsResponse>
{
    public string? Search { get; set; }
    
}