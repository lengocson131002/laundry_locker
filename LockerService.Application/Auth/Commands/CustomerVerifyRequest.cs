namespace LockerService.Application.Auth.Commands;

public class CustomerVerifyRequestValidator : AbstractValidator<CustomerVerifyRequest>
{
    public CustomerVerifyRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .NotEmpty();
    }
}

public class CustomerVerifyRequest : IRequest<StatusResponse>
{
    [TrimString(true)]
    public string PhoneNumber { get; set; } = default!;
}