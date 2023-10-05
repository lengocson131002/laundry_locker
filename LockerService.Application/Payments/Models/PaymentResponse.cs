namespace LockerService.Application.Payments.Models;

public class PaymentResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }
    
    public string? Qr { get; set; }
    
    public string? Url { get; set; }
    
    public long OrderId { get; set; }

    public long CustomerId { get; set; }

    public PaymentStatus Status { get; set; }
}