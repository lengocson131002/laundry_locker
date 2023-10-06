using LockerService.Application.Features.Settings.Models;

namespace LockerService.Application.Features.Settings.Queries;

public class GetSettingsQuery : IRequest<SettingsResponse>
{
    public string? Search { get; set; }
    
}