namespace LockerService.Application.Auth.Commands;

public class CustomerVerifyRequestValidator : AbstractValidator<CustomerVerifyRequest>
{
    public CustomerVerifyRequestValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty();
    }
}

public class CustomerVerifyRequest : IRequest<StatusResponse>
{
    public string Username { get; set; } = default!;
}