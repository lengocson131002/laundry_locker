using LockerService.Application.Features.Accounts.Models;

namespace LockerService.Application.Features.Payments.Models;

public class PaymentResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }

    public string ReferenceId { get; set; } = default!;
    
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }
    
    public string? Qr { get; set; }
    
    public string? Url { get; set; }
    
    public string? Deeplink { get; set; }
    
    public long? OrderId { get; set; }

    public long CustomerId { get; set; }

    public PaymentStatus Status { get; set; }
    
    public string? Description { get; set; }
    
    public AccountResponse? Customer { get; set; }
    
    public PaymentType Type { get; set; }
}