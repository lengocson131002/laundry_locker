namespace LockerService.Application.Auth.Commands;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(model => model.CurrentPassword)
            .NotEmpty();

        RuleFor(model => model.NewPassword)
            .NotEmpty();

        RuleFor(model => model.ConfirmPassword)
            .Equal(model => model.NewPassword)
            .WithMessage("Passwords do not match");
    }
}

public class UpdatePasswordCommand : IRequest<StatusResponse>
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}