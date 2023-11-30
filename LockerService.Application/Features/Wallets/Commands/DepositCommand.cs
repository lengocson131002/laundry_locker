using LockerService.Application.Features.Payments.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Wallets.Commands;

public class DepositCommandValidator: AbstractValidator<DepositCommand> 
{
    public DepositCommandValidator()
    {
        RuleFor(req => req.PhoneNumber)
            .Must(x => x == null || x.IsValidPhoneNumber())
            .WithMessage("Invalid phone number");
    }
}
public class DepositCommand : IRequest<PaymentResponse>
{
    [NormalizePhone(true)]
    public string PhoneNumber { get; set; } = default!;
    
    public decimal Amount { get; set; }
    
    public PaymentMethod Method { get; set; }
}