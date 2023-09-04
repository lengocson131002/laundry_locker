namespace LockerService.Application.Settings.Commands;

public class InformationSettingsCommandValidator : AbstractValidator<InformationSettingsCommand>
{
    public InformationSettingsCommandValidator()
    {
        RuleFor(model => model.CompanyName)
            .NotEmpty();

        RuleFor((model => model.ContactEmail))
            .Must(mail => mail == null || mail.IsValidEmail())
            .WithMessage("Invalid email address");
        
        RuleFor((model => model.ContactPhone))
            .Must(phone => phone == null || phone.IsValidPhoneNumber())
            .WithMessage("Invalid phone number");
        
        RuleFor((model => model.Facebook))
            .Must(facebook => facebook == null || facebook.IsValidUrl())
            .WithMessage("Invalid facebook url");
        
        RuleFor((model => model.Zalo))
            .Must(zalo => zalo == null || zalo.IsValidPhoneNumber())
            .WithMessage("Invalid zalo phone number");
    }
}

public class InformationSettingsCommand
{
    [TrimString(true)]
    public string CompanyName { get; set; } = default!;

    [NormalizePhone(true)]
    public string? ContactPhone { get; set; } = default!;

    [TrimString(true)]
    public string? ContactEmail { get; set; } = default!;

    [TrimString(true)]
    public string? Facebook { get; set; } = default!;

    [NormalizePhone(true)]
    public string? Zalo { get; set; } = default!;

    public TimeSpan? OpenedAt { get; set; }
    
    public TimeSpan? ClosedAt { get; set; }
}