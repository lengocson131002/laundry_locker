using LockerService.Application.Features.Settings.Models;

namespace LockerService.Application.Features.Settings.Commands;

public class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsCommandValidator()
    {
        RuleFor(model => model.InformationSettings)
            .SetInheritanceValidator(v => v.Add(new InformationSettingsCommandValidator()))
            .When(model => model.InformationSettings != null);
        
        RuleFor(model => model.AccountSettings)
            .SetInheritanceValidator(v => v.Add(new AccountSettingsCommandValidator()))
            .When(model => model.AccountSettings != null);
        
        RuleFor(model => model.OrderSettings)
            .SetInheritanceValidator(v => v.Add(new OrderSettingsCommandValidator()))
            .When(model => model.OrderSettings != null);
        
        RuleFor(model => model.ZaloAuthSettings)
            .SetInheritanceValidator(v => v.Add(new ZaloAuthSettingCommandValidator()))
            .When(model => model.ZaloAuthSettings != null);
        
        RuleFor(model => model.TimeSettings)
            .SetInheritanceValidator(v => v.Add(new TimeSettingsCommandValidator()))
            .When(model => model.TimeSettings != null);
        
        RuleFor(model => model.LockerSettings)
            .SetInheritanceValidator(v => v.Add(new LockerSettingCommandValidator()))
            .When(model => model.LockerSettings != null);
    }
}

public class UpdateSettingsCommand : IRequest<SettingsResponse>
{
    public InformationSettingsCommand? InformationSettings { get; set; } = default!;

    public AccountSettingsCommand? AccountSettings { get; set; } = default!;

    public OrderSettingsCommand? OrderSettings { get; set; } = default!;

    public ZaloAuthSettingsCommand? ZaloAuthSettings { get; set; } = default!;

    public TimeSettingsCommand? TimeSettings { get; set; } = default!;

    public LockerSettingsCommand? LockerSettings { get; set; } = default!;
}