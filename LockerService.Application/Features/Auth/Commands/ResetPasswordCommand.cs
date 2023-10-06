namespace LockerService.Application.Features.Auth.Commands;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(model => model.Token)
            .NotEmpty();

        RuleFor(model => model.Username)
            .NotEmpty();    
        
        RuleFor(model => model.Password)
            .NotEmpty();

        RuleFor(model => model.ConfirmPassword)
            .NotEmpty()
            .Equal(model => model.Password)
            .WithMessage("Confirm password not match");
    }
}

public class ResetPasswordCommand : IRequest<StatusResponse>
{
    [TrimString(true)] 
    public string Token { get; set; } = default!;
    
    [TrimString(true)] 
    public string Username { get; set; } = default!;

    [TrimString(true)] 
    public string Password { get; set; } = default!;

    [TrimString(true)] 
    public string ConfirmPassword { get; set; } = default!;
}