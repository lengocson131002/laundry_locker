using LockerService.Application.Settings.Models;

namespace LockerService.Application.Settings.Commands;

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
    }
}

public class UpdateSettingsCommand : IRequest<SettingsResponse>
{
    public InformationSettingsCommand? InformationSettings { get; set; } = default!;

    public AccountSettingsCommand? AccountSettings { get; set; } = default!;

    public OrderSettingsCommand? OrderSettings { get; set; } = default!;

    public ZaloAuthSettingCommand? ZaloAuthSettings { get; set; } = default!;
}