namespace LockerService.Application.Bills.Models;

public class BillResponse
{
    public long Id { get; set; }
    
    public long ReferenceOrderId { get; set; }
    
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }
    
    public string? Content { get; set; }
    
    public string? ReferenceTransactionId { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }

}