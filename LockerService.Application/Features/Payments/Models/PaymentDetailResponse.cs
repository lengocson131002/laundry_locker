using LockerService.Application.Features.Accounts.Models;

namespace LockerService.Application.Features.Payments.Models;

public class PaymentDetailResponse : PaymentResponse
{
    public AccountResponse Customer { get; set; } = default!;
}