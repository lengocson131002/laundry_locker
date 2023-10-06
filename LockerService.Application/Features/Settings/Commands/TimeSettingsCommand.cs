using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Settings.Commands;

public class TimeSettingsCommandValidator : AbstractValidator<TimeSettingsCommand>
{
    public TimeSettingsCommandValidator()
    {
        RuleFor(model => model.TimeZone)
            .NotEmpty()
            .Must(timezone => timezone.IsValidTimeZone())
            .WithMessage("Invalid timezone");
    }
}

public class TimeSettingsCommand
{
    public string TimeZone { get; set; } = default!;
}