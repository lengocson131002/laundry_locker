using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Features.Settings.Models;

public class SettingsResponse
{

    public InformationSettings InformationSettings { get; set; } = default!; 
        
    public AccountSettings AccountSettings { get; set; } = default!;

    public OrderSettings OrderSettings { get; set; } = default!;

    public ZaloAuthSettings ZaloAuthSettings { get; set; } = default!;

    public TimeSettings TimeSettings { get; set; } = default!;

    public LockerSettings LockerSettings { get; set; } = default!;

    public PaymentSettings PaymentSettings { get; set; } = default!;
}