namespace LockerService.Application.Features.Payments.Commands;

public class PaymentCallbackCommand : IRequest
{
    // Reference Id of our payment
    public string PaymentReferenceId { get; set; } = default!;
    
    // External reference transaction Id
    public string? ReferenceTransactionId { get; set; } = default!;
    
    public decimal Amount { get; set; }

    public bool IsSuccess { get; set; }
}