namespace LockerService.Application.Settings.Commands;

public class InformationSettingsCommandValidator : AbstractValidator<InformationSettingsCommand>
{
    public InformationSettingsCommandValidator()
    {
        RuleFor(model => model.CompanyName)
            .NotEmpty();

        RuleFor((model => model.ContactPhone))
            .Must(phone => phone == null || phone.IsValidPhoneNumber())
            .WithMessage("Invalid phone number");
    }
}

public class InformationSettingsCommand
{
    [TrimString(true)]
    public string CompanyName { get; set; } = default!;

    [TrimString(true)]
    public string? ContactPhone { get; set; } = default!;

    [TrimString(true)]
    public string? ContactEmail { get; set; } = default!;

    [TrimString(true)]
    public string? Facebook { get; set; } = default!;

    [TrimString(true)]
    public string? Zalo { get; set; } = default!;

    public TimeSpan? OpenedAt { get; set; }
    
    public TimeSpan? ClosedAt { get; set; }
}